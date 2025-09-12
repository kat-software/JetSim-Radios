
using TMPro;
using UdonSharp;
using UnityEngine;

namespace KatSoftware.JetSim.Radios.Runtime
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    [AddComponentMenu("KatSoftware/JetSim/Radios/Radio Menu")]
    public class RadioMenu : RadioEventReceiver
    {
        [SerializeField, HideInInspector] private RadioManager radioManager;

        [SerializeField] private TextMeshProUGUI channelText;
        [SerializeField] private TextMeshProUGUI radioOnText;


        private void Start() => UpdateUI();
        public override void OnRadioSettingsUpdated() => UpdateUI();
        

        #region API

        public void _ToggleRadio() => radioManager._ToggleRadioEnabled();
        public void _NextChannel() => radioManager.IncreaseChannel();
        public void _PreviousChannel() => radioManager.DecreaseChannel();

        #endregion // API
        
        private void UpdateUI()
        {
            channelText.text = (radioManager.SelectedChannel + 1).ToString();
            radioOnText.text = radioManager.GetRadioEnabled ? "ON" : "OFF";
        }
    }
}
