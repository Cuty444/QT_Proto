using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace QT.Core
{
    //시스템 관리 매니저입니다.
    public class SystemManager : MonoSingleton<SystemManager>
    {
        Dictionary<Type, SystemBase> _systems = new Dictionary<Type, SystemBase>();

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

