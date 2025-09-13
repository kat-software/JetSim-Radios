
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

        [UdonSynced] private byte _syncedData; // 4 LSBit = channel, 4 MSBits are used for
        
        private int _channel;

        [UdonSynced, FieldChangeCallback(nameof(RadioEnabled))] private bool _radioEnabled;
        public bool RadioEnabled
        {
            set
            {
                _radioEnabled = value;

                if (_localPlayerIsOwner)
                {
                    if (!_radioEnabled) radioManager._SetAllVoicesDefault();
                    RequestSerialization();
                    
                    JS_Debug.Log("RadioEnabled Local: " + _radioEnabled, this);
                }
                else // Remote player
                {
                    if (_radioEnabled) radioManager._Subscribe(this);
                    else radioManager._Unsubscribe(this);
                    
                    JS_Debug.Log("RadioEnabled Remote: " + _radioEnabled, this);
                }
            }
            get => _radioEnabled;
        }
        
        #region API
        
        private const int _MAX_CHANNEL = 15;
        
        public void _IncreaseChannel()
        {
            var newChannel = _channel + 1;
            if (newChannel > _MAX_CHANNEL)
                newChannel = 0;
            
            SetChannel(newChannel);
        }
        public void _DecreaseChannel()
        {
            var newChannel = _channel - 1;
            if (newChannel < 0)
                newChannel = _MAX_CHANNEL;
            
            SetChannel(newChannel);
        }
        public void SetChannel(int newChannel)
        {
            _channel = Mathf.Clamp(newChannel, 0, _MAX_CHANNEL);
            
            RequestSerialization();
            
            JS_Debug.Log("Channel set to: " + _channel, this);
        }
        
        #endregion // API

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
            
            JS_Debug.Log("Owner set. Remote: " + Owner.isLocal, this);
            
            if (!_localPlayerIsOwner) return;
            
            radioManager.Register(this);
            
            _syncedData = 0;
            RadioEnabled = false;
            
            RequestSerialization();
        }

        public override void _OnCleanup() => radioManager._Unsubscribe(this);
        
        #endregion // CPOP
    }
}
