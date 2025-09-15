
using UnityEngine;

using UdonSharp;
using Cyan.PlayerObjectPool;

using KatSoftware.JetSim.Common.Runtime;

namespace KatSoftware.JetSim.Radios.Runtime
{
    [AddComponentMenu("")]
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    internal partial class RadioPlayerObject : CyanPlayerObjectPoolObject
    {
        [SerializeField, HideInInspector] private RadioManager radioManager;
        
        #region SYNCHRONIZATION

        [UdonSynced] private byte _syncedData;
        
        private const byte _ZERO = 0b00000000;
        private const byte _CHANNEL_MASK = 0b01111111;
        private const byte _TRANSMIT_MASK = 0b10000000;
        
        public override void OnPreSerialization()
        {
            _syncedData = _radioEnabled ? _TRANSMIT_MASK : _ZERO;
            _syncedData |= (byte)(_channel & _CHANNEL_MASK);
        }

        public override void OnDeserialization()
        {
            _radioEnabled = (_syncedData & _TRANSMIT_MASK) > 0;
            _channel = _syncedData & _CHANNEL_MASK;

            OnLocalRadioSettingsUpdated();
        }
        
        #endregion // SYNCHRONIZATION
        
        internal void OnLocalRadioSettingsUpdated()
        {
            if (ShouldBoostVoice())
                SetVoiceBoosted(radioManager.Volume);
            else
                SetVoiceDefault();
        }

        private bool ShouldBoostVoice()
        {
            if (!_radioEnabled) return false;
            if (!radioManager.RadioEnabled) return false;
            return radioManager.Channel == _channel;
        }
        
        #region VOICE STUFF
        
        private const float _BOOSTED_NEAR = _BOOSTED_FAR - 1f;
        private const float _BOOSTED_FAR = 1000000f;
        private const float _BOOSTED_GAIN = 0.05f;

        private const float _DEFAULT_NEAR = 0f;
        private const float _DEFAULT_FAR = 25f;
        private const float _DEFAULT_GAIN = 15f;
        
        internal void SetVoiceBoosted(float volume = 1f) // TODO Does the player voice api have voice volume yet? https://feedback.vrchat.com/udon/p/setvoicegain-does-not-set-voice-gain
        {
            if (!VRC.SDKBase.Utilities.IsValid(Owner)) return;
            
            Owner.SetVoiceDistanceNear(_BOOSTED_NEAR);
            Owner.SetVoiceDistanceFar(_BOOSTED_FAR);
            Owner.SetVoiceGain(_BOOSTED_GAIN);
            
            JS_Debug.LogSuccess($"Boosted {Owner.displayName}'s voice at volume {volume}. THE VOLUME DOES NOT WORK YET SO YOU SHOULD UPVOTE THIS CANNY: https://feedback.vrchat.com/udon/p/setvoicegain-does-not-set-voice-gain", this);
        }
        internal void SetVoiceDefault()
        {
            if (!VRC.SDKBase.Utilities.IsValid(Owner)) return;
            
            Owner.SetVoiceDistanceNear(_DEFAULT_NEAR);
            Owner.SetVoiceDistanceFar(_DEFAULT_FAR);
            Owner.SetVoiceGain(_DEFAULT_GAIN);
            
            JS_Debug.LogSuccess($"Defaulted {Owner.displayName}'s voice.", this);
        }
        
        #endregion // VOICE STUFF
        
        #region CPOP
        
        public override void _OnOwnerSet()
        {
            JS_Debug.Log($"OnOwnerSet: {(Owner.isLocal ? "Local" : "Remote")}.", this);

            if (Owner.isLocal)
                radioManager.RegisterLocalRadio(this);
            else
                radioManager.RegisterRemoteRadio(this);
        }

        public override void _OnCleanup() => radioManager.UnregisterRemoteRadio(this);

        #endregion // CPOP
    }
}
