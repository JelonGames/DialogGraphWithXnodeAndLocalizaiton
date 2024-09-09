using System.Collections.Generic;
using UnityEngine;

namespace DialogGraphWithXnodeAndLocalization.Addons
{
    [CreateAssetMenu(fileName="New Speaker", menuName = "Tools/Dialog/Speaker")]
    public class Speaker : ScriptableObject
    {
        public List<IListener> Listeners = new List<IListener>();

        public void Invoke()
        {
            foreach (var listener in Listeners)
            {
                listener.Events?.Invoke();
            }
        }
    }
}