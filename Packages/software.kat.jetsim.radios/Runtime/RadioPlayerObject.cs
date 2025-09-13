
using UnityEngine;
using UdonSharp;
using VRC.SDKBase;

using Cyan.PlayerObjectPool;

using KatSoftware.JetSim.Common.Runtime;

namespace KatSoftware.JetSim.Radios.Runtime
{
    [AddComponentMenu("")]
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class RadioPlayerObject : CyanPlayerObjectPoolObject
    {
        #region CONSTANTS
        
        private const int MaxChannel = 15;
        
        private const float BoostedNear = BoostedFar - 1;
        private const float BoostedFar = 200000;
        private const float BoostedGain = 0.05f;

        private const float DefaultNear = 0;
        private const float DefaultFar = 25;
        private const float DefaultGain = 15;
        
        #endregion // CONSTANTS
        
        [SerializeField, HideInInspector] private RadioManager radioManager;


        private VRCPlayerApi _owner;
        private bool _localPlayerIsOwner;

        [UdonSynced, System.NonSerialized] private byte _channel;

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
        
        public void _IncreaseChannel()
        {
            var newChannel = _channel + 1;
            if (newChannel > MaxChannel)
                newChannel = 0;
            
            SetChannel(newChannel);
        }
        public void _DecreaseChannel()
        {
            var newChannel = _channel - 1;
            if (newChannel < 0)
                newChannel = MaxChannel;
            
            SetChannel(newChannel);
        }
        public void SetChannel(int newChannel)
        {
            newChannel = Mathf.Clamp(newChannel, 0, MaxChannel);
            _channel = (byte)newChannel;
            
            RequestSerialization();
            
            JS_Debug.Log("Channel set to: " + _channel, this);
        }
        
        public byte GetChannel => _channel;

        public void _SetVoiceBoosted()
        {
            JS_Debug.Log("Set voice boosted", this);
            if (!VRC.SDKBase.Utilities.IsValid(_owner)) return;
            
            _owner.SetVoiceDistanceNear(BoostedNear);
            _owner.SetVoiceDistanceFar(BoostedFar);
            _owner.SetVoiceGain(BoostedGain);
            
            JS_Debug.LogSuccess("Set boosted success!", this);
        }
        public void _SetVoiceDefault()
        {
            JS_Debug.Log("Set voice default", this);
            if (!VRC.SDKBase.Utilities.IsValid(_owner)) return;
            
            _owner.SetVoiceDistanceNear(DefaultNear);
            _owner.SetVoiceDistanceFar(DefaultFar);
            _owner.SetVoiceGain(DefaultGain);
            
            JS_Debug.LogSuccess("Set default success!", this);
        }
        
        #endregion // API

        #region CPOP
        
        public override void _OnOwnerSet()
        {
            _owner = Networking.GetOwner(gameObject);
            _localPlayerIsOwner = _owner.isLocal;
            
            JS_Debug.Log("Owner set. Remote: " + _owner.isLocal, this);
            
            if (!_localPlayerIsOwner) return;
            
            radioManager.Register(this);
            
            _channel = 0;
            RadioEnabled = false;
            
            RequestSerialization();
        }

        public override void _OnCleanup() => radioManager._Unsubscribe(this);
        
        #endregion // CPOP
    }
}
