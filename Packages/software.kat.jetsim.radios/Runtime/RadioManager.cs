
using UdonSharp;
using UnityEngine;

using JetBrains.Annotations;

using VRRefAssist;
using KatSoftware.JetSim.Common.Runtime;

namespace KatSoftware.JetSim.Radios.Runtime
{
    [Singleton]
    [AddComponentMenu("")]
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class RadioManager : UdonSharpBehaviour
    {
        [SerializeField, HideInInspector, FindObjectsOfType] private RadioEventReceiver[] radioEventReceivers;
        
        
        private RadioPlayerObject _localPlayerRadioObject;
        
        private bool _radioPowered;
        private bool _radioReceiving;
        private bool _radioTransmitting;


        #region API
        
        /// <summary>
        /// Called by RadioPlayerObject when assigned to the local player.
        /// </summary>
        internal void RegisterLocalRadio(RadioPlayerObject radioPlayerObject)
        {
            _localPlayerRadioObject = radioPlayerObject;
            
            _localPlayerRadioObject.SetTransmitting(_radioTransmitting && _radioPowered && _radioReceiving);
            _localPlayerRadioObject.SetChannel(Channel);
            
            NotifyRadioSettingsUpdated();
        }

        [PublicAPI]
        public void SetChannel(int newChannel)
        {
            if (!_localPlayerRadioObject) return;
            
            _localPlayerRadioObject.SetChannel(Channel);
            Channel = _localPlayerRadioObject.Channel;
            
            NotifyRadioSettingsUpdated();
        }
        
        [PublicAPI]
        public int Channel { get; private set; }
        [PublicAPI]
        public const int MAX_CHANNEL = RadioPlayerObject.MAX_CHANNEL;
        
        [PublicAPI]
        public void IncreaseChannel()
        {
            if (!_localPlayerRadioObject) return;
            
            _localPlayerRadioObject.NextChannel();
            Channel = _localPlayerRadioObject.Channel;
            
            NotifyRadioSettingsUpdated();
        }
        [PublicAPI]
        public void DecreaseChannel()
        {
            if (!_localPlayerRadioObject) return;
            
            _localPlayerRadioObject.PreviousChannel();
            Channel = _localPlayerRadioObject.Channel;
            
            NotifyRadioSettingsUpdated();
        }

        [PublicAPI]
        public void SetRadioPowered(bool state)
        {
            _transmitting = state;
            _SetRadioPowered(RadioPowered);
            
            NotifyRadioSettingsUpdated();
        }

        [PublicAPI]
        public void _ToggleRadioPowered() => _SetRadioPowered(!RadioPowered);
        [PublicAPI]
        public void _SetRadioPowered(bool state)
        {
            RadioPowered = state;
            
            if (_localPlayerRadioObject)
                _localPlayerRadioObject.SetPowered(RadioPowered);
            
            NotifyRadioSettingsUpdated();
        }

        public bool RadioPowered { get; private set; }


        public void _SetAllVoicesDefault()
        {
            /*
            int activeRadiosCount = _activeRadios.Length;
            for (int i = 0; i < activeRadiosCount; i++)
                _activeRadios[i]._SetVoiceDefault();
            */
        }
        
        #endregion // API
        
        private void Start()
        {
            SendCustomEventDelayedSeconds(nameof(_RadioVoiceVolumesLoop), 5);
        }
        public void _RadioVoiceVolumesLoop()
        {
            SendCustomEventDelayedFrames(nameof(_RadioVoiceVolumesLoop), 5);
            
            if (!_localPlayerRadioObject) return;
            if (!(RadioPowered && _transmitting)) return;

            var myCurrentChannel = _localPlayerRadioObject.Channel;
            
            int activeRadiosCount = _activeRadios.Length;
            for (int i = 0; i < activeRadiosCount; i++) // TODO: Make it only update this if anything changed. When receiving sync only update that player's voice, when changing the local player's settings, loop through all radios
            {
                RadioPlayerObject playerRadio = _activeRadios[i];
                if (!playerRadio) { JS_Debug.LogError($"Radio object was null! Index: {i}.", this); continue; }

                bool inSameChannel = myCurrentChannel == playerRadio.Channel;
                
                //if (inSameChannel) playerRadio._SetVoiceBoosted();
                //else playerRadio._SetVoiceDefault();
            }
        }


        private RadioPlayerObject[] _activeRadios = System.Array.Empty<RadioPlayerObject>();
        public void _Subscribe(RadioPlayerObject playerObject)
        {
            JS_Debug.Log("Radio object added", this);
            _activeRadios = _activeRadios.AddUnique(playerObject);
        }
        public void _Unsubscribe(RadioPlayerObject playerObject)
        {
            JS_Debug.Log("Radio object removed", this);
            _activeRadios = _activeRadios.Remove(playerObject);
            //playerObject._SetVoiceDefault();
        }

        private void NotifyRadioSettingsUpdated()
        {
            foreach (RadioEventReceiver eventReceiver in radioEventReceivers)
                eventReceiver.OnRadioSettingsUpdated();
        }
    }
    
    #region ARRAY EXTENSIONS

    /*
    MIT License

    Copyright (c) 2022 Varneon

    Permission is hereby granted, free of charge, to any person obtaining a copy
    of this software and associated documentation files (the "Software"), to deal
    in the Software without restriction, including without limitation the rights
    to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    copies of the Software, and to permit persons to whom the Software is
    furnished to do so, subject to the following conditions:

    The above copyright notice and this permission notice shall be included in all
    copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    SOFTWARE.
    */
    
    internal static class ArrayExtensions
    {
        /// <summary>
        /// Adds an object to the end of the array
        /// <para>
        /// Based on: <see href="https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1.add?view=net-6.0">List&lt;T&gt;.Add(T)</see>
        /// </para>
        /// </summary>
        /// <returns>Modified T[]</returns>
        /// <typeparam name="T">The type of elements in the array.</typeparam>
        /// <param name="array">Source T[] to modify.</param>
        /// <param name="item">The object to be added to the end of the T[].</param>
        public static T[] Add<T>(this T[] array, T item)
        {
            int length = array.Length;

            T[] newArray = new T[length + 1];

            array.CopyTo(newArray, 0);

            newArray.SetValue(item, length);

            return newArray;
        }
        
        /// <summary>
        /// Removes the first occurrence of a specific object from the array
        /// <para>
        /// Based on: <see href="https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1.remove?view=net-6.0">List&lt;T&gt;.Remove(T)</see>
        /// </para>
        /// </summary>
        /// <returns>Modified T[]</returns>
        /// <typeparam name="T">The type of elements in the array.</typeparam>
        /// <param name="array">Source T[] to modify.</param>
        /// <param name="item">The object to remove from the T[].</param>
        public static T[] Remove<T>(this T[] array, T item)
        {
            int index = System.Array.IndexOf(array, item);

            if (index == -1) { return array; }

            return array.RemoveAt(index);
        }
        
        /// <summary>
        /// Removes the element at the specified index of the array
        /// <para>
        /// Based on: <see href="https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1.removeat?view=net-6.0">List&lt;T&gt;.RemoveAt(Int32)</see>
        /// </para>
        /// </summary>
        /// <returns>Modified T[]</returns>
        /// <typeparam name="T">The type of elements in the array.</typeparam>
        /// <param name="array">Source T[] to modify.</param>
        /// <param name="index">The zero-based index of the element to remove.</param>
        public static T[] RemoveAt<T>(this T[] array, int index)
        {
            int length = array.Length;

            if(index >= length || index < 0) { return array; }

            int maxIndex = length - 1;

            T[] newArray = new T[maxIndex];

            if (index == 0)
            {
                System.Array.Copy(array, 1, newArray, 0, maxIndex);
            }
            else if(index == maxIndex)
            {
                System.Array.Copy(array, 0, newArray, 0, maxIndex);
            }
            else
            {
                System.Array.Copy(array, 0, newArray, 0, index);
                System.Array.Copy(array, index + 1, newArray, index, maxIndex - index);
            }

            return newArray;
        }
        
        /// <summary>
        /// Adds an object to the end of the array while ensuring that duplicates are not added
        /// </summary>
        /// <returns>Modified T[]</returns>
        /// <typeparam name="T">The type of elements in the array.</typeparam>
        /// <param name="array">Source T[] to modify.</param>
        /// <param name="item">The object to be added to the end of the T[].</param>
        public static T[] AddUnique<T>(this T[] array, T item)
        {
            if (System.Array.IndexOf(array, item) >= 0) { return array; }

            return array.Add(item);
        }
    }

    #endregion // ARRAY EXTENSIONS
}