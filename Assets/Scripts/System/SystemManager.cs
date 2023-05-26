using System.Collections;
using System.Collections.Generic;
using System;
using QT.InGame;
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

        public SoundManager SoundManager { get; } = new();
        
        [field:SerializeField]public UIManager UIManager { get; private set; }
        [SerializeField] private GameObject _debugConsole;
        
        private readonly Dictionary<Type, SystemBase> _systems = new ();

        private void Awake()
        {
            base.Awake();
            
            ResourceManager.Initialize();
            DataManager.Initialize();
            
            InitializeSystems();

            UIManager?.Initialize();
            
            LoadingManager.Initialize();
            
            SoundManager.Initialize(GetComponent<AudioSource>());
            
            _PostInitializeSystems();
            
            UIManager?.PostSystemInitialize();
            
            _debugConsole.SetActive(Debug.isDebugBuild);
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


#if Testing
        
        bool isStatShow = false;
        private void OnGUI()
        {
            PlayerStat();
        }

        private void PlayerStat()
        {
            if (GUI.Button(new Rect(10, Screen.height - 25, 160, 20), "플레이어 스탯 표시"))
            {
                isStatShow = !isStatShow;
            }

            if (!isStatShow || PlayerManager.Player == null)
            {
                return;
            }
            
            float startY = 40;
            float cellHeight = 20;

            var height = startY + cellHeight * (int) PlayerStats.AtkDmgPer + 45;
            
            GUI.Box(new Rect(10,Screen.height - height,210,height), "플레이어 스탯");

            for (int i = 0; i <= (int) PlayerStats.AtkDmgPer; i++)
            {
                var statKey = (PlayerStats) i;
                var stat = PlayerManager.Player.GetStat(statKey);

                var y = (Screen.height - height + startY) + cellHeight * i;

                var style = new GUIStyle {fontStyle = FontStyle.Bold};
                style.normal.textColor = Color.white;
                
                GUI.Label(new Rect(10, y, 160, 20), statKey.ToString(), style);

                style.fontStyle = FontStyle.Normal;
                GUI.Label(new Rect(160, y, 20, 20), $"{stat.BaseValue}", style);

                var plusStat = stat.Value - stat.BaseValue;

                //if (plusStat != 0)
                {
                    style.normal.textColor = plusStat < 0 ? Color.red : Color.cyan;
                    GUI.Label(new Rect(190, y, 20, 20), $"{stat.Value - stat.BaseValue}", style);
                }
            }
        }
#endif
        
    }
}

