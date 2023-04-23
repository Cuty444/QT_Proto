using System.Collections;
using System.Collections.Generic;
using System;
using QT.UI;
using UnityEngine;

namespace QT.Core
{
    //시스템 관리 매니저입니다.
    public class SystemManager : MonoSingleton<SystemManager>
    {
        public ResourceManager ResourceManager { get; } = new ();
        public GameDataManager DataManager { get; } = new ();
        public PlayerManager PlayerManager { get; } = new();
        public ProjectileManager ProjectileManager { get; } = new();

        public LoadingManager LoadingManager { get; } = new();
        
        [field:SerializeField]public UIManager UIManager { get; private set; }
        
        private readonly Dictionary<Type, SystemBase> _systems = new ();

        private void Awake()
        {
            base.Awake();
            
            ResourceManager.Initialize();
            DataManager.Initialize();
            
            InitializeSystems();

            UIManager?.Initialize();
            
            LoadingManager.Initialize();
            
            _PostInitializeSystems();
            
            UIManager?.PostSystemInitialize();
        }

        private void InitializeSystems()
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

