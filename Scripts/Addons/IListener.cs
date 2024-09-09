using UnityEngine.Events;

namespace DialogGraphWithXnodeAndLocalization.Addons
{
    public interface IListener
    {
        public Speaker Speaker { get; set; }
        public UnityEvent Events { get; set; }
        public void Register();
        public void Unregister();
    }
}