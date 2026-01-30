using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Core
{
    public class ServiceLocator : MonoBehaviour
    {
        private static ServiceLocator _instance;
        public static ServiceLocator Instance
        {
            get
            {
                if (_instance == null)
                {
                    Debug.LogError("[ServiceLocator] Instance not found! Make sure ServiceLocator exists in Bootstrap scene.");
                }
                return _instance;
            }
        }

        private Dictionary<Type, IService> _services = new Dictionary<Type, IService>();

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Debug.LogWarning("[ServiceLocator] Instance already exists. Destroying duplicate.");
                Destroy(gameObject);
                return;
            }

            _instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("[ServiceLocator] Initialized and set to DontDestroyOnLoad.");
        }
        public void Register<T>(T service) where T : IService
        {
            Type type = typeof(T);

            if (_services.ContainsKey(type))
            {
                Debug.LogWarning($"[ServiceLocator] Service {type.Name} is already registered. Skipping.");
                return;
            }

            _services[type] = service;
            service.Initialize();
            Debug.Log($"[ServiceLocator] ✓ Registered and initialized service: {type.Name}");
        }
        public T Get<T>() where T : IService
        {
            Type type = typeof(T);

            if (_services.TryGetValue(type, out IService service))
            {
                return (T)service;
            }

            Debug.LogError($"[ServiceLocator] Service {type.Name} not found! Make sure it's registered.");
            return default;
        }
        public bool IsRegistered<T>() where T : IService
        {
            return _services.ContainsKey(typeof(T));
        }
        public void Unregister<T>() where T : IService
        {
            Type type = typeof(T);
            if (_services.Remove(type))
            {
                Debug.Log($"[ServiceLocator] Unregistered service: {type.Name}");
            }
        }

        private void OnDestroy()
        {
            if (_instance == this)
            {
                _instance = null;
                _services.Clear();
                Debug.Log("[ServiceLocator] Destroyed and cleared all services.");
            }
        }
    }
}