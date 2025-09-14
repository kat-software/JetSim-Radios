
using UdonSharp;
using UnityEngine;

namespace KatSoftware.JetSim.Radios.Runtime
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    [AddComponentMenu("KatSoftware/JetSim/Radios/Radio Activator")]
    public class RadioActivator : UdonSharpBehaviour
    {
        [SerializeField, HideInInspector] private RadioManager radioManager;

        private void OnEnable() => radioManager.SetRadioPowered(true);
        private void OnDisable() => radioManager.SetRadioPowered(false);
    }
}