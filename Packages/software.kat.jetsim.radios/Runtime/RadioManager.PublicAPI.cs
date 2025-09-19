
using UnityEngine;
using JetBrains.Annotations;

namespace KatSoftware.JetSim.Radios.Runtime
{
    public partial class RadioManager
    {
        /// <summary>
        /// If the Radio system is both powered and enabled. This is the requirement for the player being able to transmit and receive.
        /// </summary>
        [PublicAPI] public bool RadioEnabled => RadioSystemEnabled && RadioPowered;

        #region VOLUME

        [PublicAPI] public float Volume { get; private set; }
        
        /// <summary>
        /// Sets the volume of all remote voices on the radio.
        /// </summary>
        /// <param name="volume">0 (off) to 1 (max).</param>
        [PublicAPI]
        public void SetVolume(float volume)
        {
            Volume = Mathf.Clamp(volume, 0f, 1f);
            
            NotifyLocalRadioSettingsUpdated();
        }

        #endregion // VOLUME
        
        #region CHANNEL

        [PublicAPI] public int Channel { get; private set; }
        [PublicAPI] public const int MAX_CHANNEL = RadioPlayerObject.MAX_CHANNEL;

        [PublicAPI]
        public void IncreaseChannel()
        {
            if (!_localPlayerRadioObject) return;

            Channel = _localPlayerRadioObject.NextChannel();

            NotifyLocalRadioSettingsUpdated();
        }

        [PublicAPI]
        public void DecreaseChannel()
        {
            if (!_localPlayerRadioObject) return;

            Channel = _localPlayerRadioObject.PreviousChannel();

            NotifyLocalRadioSettingsUpdated();
        }

        [PublicAPI]
        public void SetChannel(int newChannel)
        {
            Channel = newChannel;

            if (!_localPlayerRadioObject) return;

            Channel = _localPlayerRadioObject.SetChannel(newChannel);

            NotifyLocalRadioSettingsUpdated();
        }

        #endregion // CHANNEL

        #region POWERED

        /// <summary>
        /// For example if the player is in a RadioZone, a RadioActivator is enabled, or whatever you as the creator decide.
        /// </summary>
        [PublicAPI] public bool RadioPowered { get; private set; }

        /// <inheritdoc cref="RadioPowered" />
        [PublicAPI]
        public void ToggleRadioPowered() => SetRadioPowered(!RadioPowered);

        /// <inheritdoc cref="RadioPowered" />
        [PublicAPI]
        public void SetRadioPowered(bool state)
        {
            RadioPowered = state;

            if (!_localPlayerRadioObject) return;

            _localPlayerRadioObject.SetRadioEnabled(RadioEnabled);

            NotifyLocalRadioSettingsUpdated();
        }

        #endregion // POWERED

        #region SYSTEM ENABLED

        /// <summary>
        /// The player's preference for if they want to use the radio system.
        /// </summary>
        [PublicAPI] public bool RadioSystemEnabled { get; private set; }

        /// <inheritdoc cref="RadioSystemEnabled" />
        [PublicAPI]
        public void ToggleRadioSystemEnabled() => SetRadioSystemEnabled(!RadioSystemEnabled);
        
        /// <inheritdoc cref="RadioSystemEnabled" />
        [PublicAPI]
        public void SetRadioSystemEnabled(bool state)
        {
            RadioSystemEnabled = state;

            if (!_localPlayerRadioObject) return;

            _localPlayerRadioObject.SetRadioEnabled(RadioEnabled);

            NotifyLocalRadioSettingsUpdated();
        }

        #endregion // SYSTEM ENABLED
    }
}
