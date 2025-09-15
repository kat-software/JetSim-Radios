
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using JetBrains.Annotations;

using UdonSharp;

namespace KatSoftware.JetSim.Radios.Runtime
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    [AddComponentMenu("KatSoftware/JetSim/Radios/Radio Menu")]
    public class RadioMenu : RadioEventReceiver
    {
        [SerializeField, HideInInspector] private RadioManager radioManager;

        [SerializeField] private Slider volumeSlider;
        [SerializeField] private TextMeshProUGUI volumeText;
        [SerializeField] private TextMeshProUGUI channelText;
        [SerializeField] private TextMeshProUGUI radioOnText;
        
        public override void OnLocalRadioSettingsUpdated() => UpdateUI();
        

        #region PUBLIC API
        
        [PublicAPI] public void _OnVolumeSliderValueChanged() => radioManager.SetVolume(volumeSlider.value);
        [PublicAPI] public void _ToggleRadio() => radioManager.ToggleRadioPowered();
        [PublicAPI] public void _NextChannel() => radioManager.IncreaseChannel();
        [PublicAPI] public void _PreviousChannel() => radioManager.DecreaseChannel();

        #endregion // PUBLIC API
        
        private void UpdateUI()
        {
            volumeSlider.SetValueWithoutNotify(radioManager.Volume);
            volumeText.text = (radioManager.Volume * 100f).ToString("0.0") + "%";
            channelText.text = $"{radioManager.Channel + 1} / {RadioManager.MAX_CHANNEL + 1}";
            radioOnText.text = radioManager.RadioSystemEnabled ? "ON" : "OFF";
        }
    }
}
