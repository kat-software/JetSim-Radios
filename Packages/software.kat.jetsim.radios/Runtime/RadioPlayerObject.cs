
using UnityEngine;
using UdonSharp;

using Cyan.PlayerObjectPool;

using KatSoftware.JetSim.Common.Runtime;

namespace KatSoftware.JetSim.Radios.Runtime
{
    [AddComponentMenu("")]
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class RadioPlayerObject : CyanPlayerObjectPoolObject
    {
        [SerializeField, HideInInspector] private RadioManager radioManager;

        private bool _localPlayerIsOwner;
        private bool _transmitting;
        
        
        #region INTERNAL API
        
        internal void SetTransmitting(bool state)
        {
            _transmitting = state;
            
            RequestSerialization();
            OnSettingsUpdated();
        }

        internal const int MAX_CHANNEL = _CHANNEL_MASK;
        internal int Channel { get; private set; }

        internal void NextChannel(bool wrap = true)
        {
            var newChannel = Channel + 1;
            
            if (wrap)
                if (newChannel > MAX_CHANNEL) newChannel = 0;
            
            SetChannel(newChannel);
        }
        internal void PreviousChannel(bool wrap = true)
        {
            var newChannel = Channel - 1;
            
            if (wrap) 
                if (newChannel < 0) newChannel = MAX_CHANNEL;
            
            SetChannel(newChannel);
        }
        /// <summary>
        /// Sets the channel for this player's radio and syncs it.
        /// </summary>
        /// <param name="newChannel">Clamped between 0 and <see cref="MAX_CHANNEL"/>.</param>
        internal void SetChannel(int newChannel)
        {
            Channel = Mathf.Clamp(newChannel, 0, MAX_CHANNEL);
            JS_Debug.Log("Channel set to: " + Channel, this);
            
            RequestSerialization();
            OnSettingsUpdated();
        }
        
        #endregion // INTERNAL API

        #region SYNC

        [UdonSynced] private byte _syncedData;
        
        private const byte _ZERO = 0b00000000; // The ternary op requires this to be a const variable for some reason.
        private const byte _CHANNEL_MASK = 0b01111111;
        private const byte _TRANSMIT_MASK = 0b10000000;
        
        public override void OnPreSerialization()
        {
            _syncedData = _transmitting ? _TRANSMIT_MASK : _ZERO;
            _syncedData |= (byte)(Channel & _CHANNEL_MASK);
        }

        public override void OnDeserialization()
        {
            _transmitting = (_syncedData & _TRANSMIT_MASK) > 0;
            Channel = _syncedData & _CHANNEL_MASK;

            OnSettingsUpdated();
        }
        
        #endregion // SYNC
        
        private void OnSettingsUpdated()
        {
            if (_localPlayerIsOwner) return;

            //if (RadioConnection()) SetVoiceBoosted(); else SetVoiceDefault();
        }
        
        #region VOICE STUFF
        
        private const float _BOOSTED_NEAR = _BOOSTED_FAR - 1f;
        private const float _BOOSTED_FAR = 200000f;
        private const float _BOOSTED_GAIN = 0.05f;

        private const float _DEFAULT_NEAR = 0f;
        private const float _DEFAULT_FAR = 25f;
        private const float _DEFAULT_GAIN = 15f;
        
        private void SetVoiceBoosted()
        {
            JS_Debug.Log("Set voice boosted", this);
            if (!VRC.SDKBase.Utilities.IsValid(Owner)) return;
            
            Owner.SetVoiceDistanceNear(_BOOSTED_NEAR);
            Owner.SetVoiceDistanceFar(_BOOSTED_FAR);
            Owner.SetVoiceGain(_BOOSTED_GAIN);
            
            JS_Debug.LogSuccess("Set boosted success!", this);
        }
        private void SetVoiceDefault()
        {
            JS_Debug.Log("Set voice default", this);
            if (!VRC.SDKBase.Utilities.IsValid(Owner)) return;
            
            Owner.SetVoiceDistanceNear(_DEFAULT_NEAR);
            Owner.SetVoiceDistanceFar(_DEFAULT_FAR);
            Owner.SetVoiceGain(_DEFAULT_GAIN);
            
            JS_Debug.LogSuccess("Set default success!", this);
        }
        
        #endregion // VOICE STUFF
        
        #region CPOP
        
        public override void _OnOwnerSet()
        {
            _localPlayerIsOwner = Owner.isLocal;
            
            JS_Debug.Log($"OnOwnerSet: {(_localPlayerIsOwner ? "Local" : "Remote")}.", this);
            
            if (!_localPlayerIsOwner) return;
            
            radioManager.RegisterLocalRadio(this);
            
            _radioPowered = false;
            _wantsToTransmit = false;
            
            RequestSerialization();
        }

        public override void _OnCleanup() => radioManager._Unsubscribe(this);
        
        #endregion // CPOP
    }
}
