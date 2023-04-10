using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace QT.Core
{
    public class GameManager : MonoSingleton<GameManager>
    {
        private readonly Dictionary<Type, SystemBase> _systems = new ();

        private void Awake()
        {
            _InitializeSystems();

            UI.UIManager.Instance.Initialize();

            _PostInitializeSystems();

            UI.UIManager.Instance.PostSystemInitialize();
        }

        private void _InitializeSystems()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                SystemBase childSystem = transform.GetChild(i).GetComponent<SystemBase>();

                if (childSystem == null)
                {
                    continue;
                }

                childSystem.OnInitialized();
                _systems.Add(childSystem.GetType(), childSystem);
            }
        }

        private void _PostInitializeSystems()
        {
            foreach (var system in _systems)
            {
                system.Value.OnPostInitialized();
            }
        }

        public T GetSystem<T>() where T : SystemBase
        {
            if (_systems.TryGetValue(typeof(T), out var system))
            {
                return (T)system;
            }

            return null;
        }

        public static T GetSystemInInstance<T>() where T : SystemBase
        {
            return Instance.GetSystem<T>();
        }
    }
}
