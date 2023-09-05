#if UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace QT.Core
{
    public class EditorSystemManager
    {
        private static EditorSystemManager _instance = null;
        public static EditorSystemManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new();
                }
                return _instance;
            }
        }
        
        
        public GameDataManager DataManager { get; private set; } = new ();

        public string[] EnemyIds;
        public string[] EnemyNames;
        
        EditorSystemManager()
        {
            DataManager.Initialize();
        }
        
        
        [MenuItem("GameData/EditorGameDataReset", false, 9999)]
        public static void ResetEditorGameData()
        {
            Instance.DataManager = new GameDataManager();
            Instance.DataManager.Initialize();
        }
    }
}
#endif
