
using UnityEngine;
using VRC.SDKBase;
using UdonSharp;

using KatSoftware.JetSim.Common.Runtime;

namespace KatSoftware.JetSim.Radios.Runtime
{
    [AddComponentMenu("")]
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class RadioPlayerObject : UdonSharpBehaviour
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


        private void Start()
        {
            _owner = Networking.GetOwner(gameObject);
            _localPlayerIsOwner = _owner.isLocal;
            
            if (!_localPlayerIsOwner) return;
            
            radioManager.Register(this);
            
            _channel = 0;
            RadioEnabled = false;
            
            RequestSerialization();
        }
        
        private void OnDestroy() => radioManager._Unsubscribe(this);
        
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
            if (!VRC.SDKBase.Utilities.IsValid(_owner)) return;
            
            _owner.SetVoiceDistanceNear(BoostedNear);
            _owner.SetVoiceDistanceFar(BoostedFar);
            _owner.SetVoiceGain(BoostedGain);
        }
        public void _SetVoiceDefault()
        {
            if (!VRC.SDKBase.Utilities.IsValid(_owner)) return;
            
            _owner.SetVoiceDistanceNear(DefaultNear);
            _owner.SetVoiceDistanceFar(DefaultFar);
            _owner.SetVoiceGain(DefaultGain);
        }
        
        #endregion // API
    }
}