using System;
using UnityEngine;
using UnityEngine.Events;

namespace DialogGraphWithXnodeAndLocalization.Addons
{
    public class Listener : MonoBehaviour, IListener
    {
        public Speaker Speaker 
        { 
            get => _speaker; 
            set => _speaker = value;
        }

        public UnityEvent Events 
        { 
            get => _events;
            set => _events = value;
        }

        [SerializeField] private Speaker _speaker;
        [SerializeField] private UnityEvent _events;

        private void OnEnable()
        {
            Register();
        }

        private void OnDisable()
        {
            Unregister();
        }

        public void Register() => Speaker.Listeners.Add(this);

        public void Unregister() => Speaker.Listeners.Remove(this);
    }
}