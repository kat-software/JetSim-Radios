
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

using KatSoftware.JetSim.Common.Runtime;

namespace KatSoftware.JetSim.Radios.Runtime
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    [AddComponentMenu("KatSoftware/JetSim/Radios/Radio Zone")]
    public class RadioZone : CoroutineBehaviour
    {
        [SerializeField] private Collider zoneCollider;

        private void OnValidate()
        {
            if (!zoneCollider)
                zoneCollider = GetComponent<Collider>();
        }

        [SerializeField, HideInInspector] private RadioManager radioManager;

        
        private VRCPlayerApi _localPlayer;
        private Bounds _zone;
        
        
        private bool _inZone;

        private void Start() => Init();
        
        private bool _init;
        private void Init()
        {
            if (_init) return;
            _init = true;
            
            _localPlayer = Networking.LocalPlayer;
            _zone = zoneCollider.bounds;
            _zone.Expand(1f);
        }


        public override void OnPlayerTriggerEnter(VRCPlayerApi player)
        {
            if (!Utilities.IsValid(player)) return;
            if (!player.isLocal) return;
            if (!_init) Init();
            
            _inZone = true;
            radioManager.SetRadioPowered(_inZone);

            StartCoroutine(nameof(_InternalZoneCheckLoop), 1f);
        }

        public override void OnPlayerTriggerExit(VRCPlayerApi player)
        {
            if (!Utilities.IsValid(player)) return;
            if (!player.isLocal) return;
            if (!_init) Init();
            
            _inZone = false;
            radioManager.SetRadioPowered(_inZone);
            
            StopCoroutines();
        }

        /// <summary>
        /// Only public due to SendCustomEventDelayed. Do not call this.
        /// </summary>
        public void _InternalZoneCheckLoop()
        {
            if (LocalPlayerWithinZoneBounds) return;
            
            _inZone = false;
            radioManager.SetRadioPowered(_inZone);
            
            StopCoroutines();
        }

        private bool LocalPlayerWithinZoneBounds => _zone.Contains(_localPlayer.GetPosition());
    }
}
