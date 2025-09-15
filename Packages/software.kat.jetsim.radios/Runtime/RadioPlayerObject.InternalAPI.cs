
using UnityEngine;
using KatSoftware.JetSim.Common.Runtime;

namespace KatSoftware.JetSim.Radios.Runtime
{
    internal partial class RadioPlayerObject
    {
        private bool _radioEnabled;
        /// <summary>
        /// Causes a network serialization.
        /// </summary>
        /// <param name="state">The value to set.</param>
        internal void SetRadioEnabled(bool state)
        {
            _radioEnabled = state;
            
            RequestSerialization();
        }

        internal const int MAX_CHANNEL = _CHANNEL_MASK;
        private int _channel;

        /// <inheritdoc cref="SetChannel(int, bool)"/>
        internal int NextChannel(bool wrap = true) => SetChannel(_channel + 1, wrap);
        
        /// <inheritdoc cref="SetChannel(int, bool)"/>
        internal int PreviousChannel(bool wrap = true) => SetChannel(_channel - 1, wrap);

        /// <summary>
        /// Causes a network serialization.
        /// </summary>
        /// <param name="newChannel">Clamped between 0 and <see cref="MAX_CHANNEL"/>.</param>
        /// <param name="wrap">If the channel value should wrap instead of clamp.</param>
        /// <returns>The value that was set.</returns>
        internal int SetChannel(int newChannel, bool wrap = false)
        {
            if (wrap) _channel = newChannel % (MAX_CHANNEL + 1);
            else _channel = Mathf.Clamp(newChannel, 0, MAX_CHANNEL);
            
            JS_Debug.Log("Channel set to: " + _channel, this);
            
            RequestSerialization();
            
            return _channel;
        }
    }
}
