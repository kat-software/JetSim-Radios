
using UdonSharp;

namespace KatSoftware.JetSim.Radios.Runtime
{
    public abstract class RadioEventReceiver : UdonSharpBehaviour
    {
        public abstract void OnRadioSettingsUpdated();
    }
}
