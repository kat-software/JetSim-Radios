
using UnityEngine;

using UdonSharp;
using VRRefAssist;

using KatSoftware.JetSim.Common.Runtime;
using KatSoftware.JetSim.Common.Runtime.Extensions;

#if UNITY_EDITOR && !COMPILER_UDONSHARP
using System.Linq;
#endif

namespace KatSoftware.JetSim.Radios.Runtime
{
    [Singleton]
    [AddComponentMenu("")]
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public partial class RadioManager : UdonSharpBehaviour
    {
        [SerializeField, HideInInspector, FindObjectsOfType] private RadioEventReceiver[] eventReceivers;

        #if UNITY_EDITOR && !COMPILER_UDONSHARP
        [RunOnBuild(1001)] // After field automation.
        private void RunOnBuild()
        {
            eventReceivers = eventReceivers.Where(x => x != null).ToArray();
        }
        #endif

        private RadioPlayerObject _localPlayerRadioObject;
        
        internal void RegisterLocalRadio(RadioPlayerObject radioPlayerObject)
        {
            _localPlayerRadioObject = radioPlayerObject;
            
            SetVolume(1f);
            RadioSystemEnabled = true;
            Channel = _localPlayerRadioObject.SetChannel(Channel);
            _localPlayerRadioObject.SetRadioEnabled(RadioEnabled);
            
            NotifyLocalRadioSettingsUpdated();
        }


        private RadioPlayerObject[] _activeRadios = System.Array.Empty<RadioPlayerObject>();
        internal void RegisterRemoteRadio(RadioPlayerObject playerObject)
        {
            JS_Debug.Log("Adding remote radio.", this);
            _activeRadios = _activeRadios.AddUnique(playerObject);
        }
        internal void UnregisterRemoteRadio(RadioPlayerObject playerObject)
        {
            JS_Debug.Log("Removing remote radio.", this);
            _activeRadios = _activeRadios.Remove(playerObject);
        }

        private void NotifyLocalRadioSettingsUpdated()
        {
            // I really wish we had interfaces!
            
            foreach (RadioEventReceiver eventReceiver in eventReceivers)
                eventReceiver.OnLocalRadioSettingsUpdated();
            
            foreach (RadioPlayerObject playerRadio in _activeRadios)
                playerRadio.OnLocalRadioSettingsUpdated();
        }
    }
}
