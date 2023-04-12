using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEditor;

namespace QT.Core
{
    public class GameDataValidator : EditorWindow
    {
        private class DataInfo
        {
            public bool IsOpen;
            public string Name;
            
            public List<string> GameDataFields;
            public List<string> JsonFields;

            public bool JsonNotFound;
        }
        
        private static string JsonDataPath = $"{Directory.GetCurrentDirectory()}/Assets/Data/GameData";
        
        [MenuItem("GameData/GameData Validator", false, 0)]
        public static void OpenGameDataValidator()
        {
            GetWindow(typeof(GameDataValidator));
        }

        private List<DataInfo> _infos = null;

        private void OnGUI()
        {
            if (GUILayout.Button("새로고침"))
            {
                SetDataInfos();
            }

            if (_infos != null)
            {
                foreach (var info in _infos)
                {
                    ShowInfo(info);
                }
            }
        }

        private void ShowInfo(DataInfo info)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.Height(55));

            GUIStyle style = new GUIStyle(EditorStyles.foldout);
            
            style.normal.textColor = Color.red;
            
            //info.IsOpen = EditorGUILayout.Foldout(info.IsOpen, info.Name,style);

            info.IsOpen = EditorGUILayout.Foldout(info.IsOpen, info.Name, style);

            if (info.IsOpen)
            {
                foreach (var fieldName in info.GameDataFields)
                {
                    Debug.Log(fieldName);
                    GUILayout.Label($"{fieldName}", EditorStyles.largeLabel, GUILayout.ExpandWidth(false));
                }
            }
            
            // EditorGUILayout.BeginHorizontal();
            // GUILayout.Label($"GLOBAL : ", EditorStyles.whiteLargeLabel, GUILayout.Width(100));
            // GUILayout.Label($"{globalStateName}", EditorStyles.largeLabel, GUILayout.ExpandWidth(false));
            // EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.EndVertical();
        }
    
        private void SetDataInfos()
        {
            var dataBaseTypes = Assembly.GetAssembly(typeof(IGameDataBase)).GetTypes().Where(t => typeof(IGameDataBase) != t && typeof(IGameDataBase).IsAssignableFrom(t));
    
            _infos = new List<DataInfo>(dataBaseTypes.Count());
            
            foreach (var dataBaseType in dataBaseTypes)
            {
                var attribute = dataBaseType.GetCustomAttribute<GameDataBaseAttribute>();
                var json = JArray.Parse(File.ReadAllText($"{JsonDataPath}/{attribute.JsonFileName}.json"));
                    
                var dataInfo = new DataInfo();
                
                dataInfo.Name = dataBaseType.Name;
                
                var propertyInfos = attribute.GameDataType.GetProperties(BindingFlags.GetField);
                dataInfo.GameDataFields = new(propertyInfos.Count());
                
                foreach (var property in propertyInfos)
                {
                    dataInfo.GameDataFields.Add(property.Name);
                }
                
                dataInfo.JsonNotFound = json == null;
                if(!dataInfo.JsonNotFound)
                {
                    dataInfo.JsonFields = new(json[0].Count());
                    foreach (var data in json[0])
                    {
                        dataInfo.JsonFields.Add(data.ToString());
                    }
                }
                    
                _infos.Add(dataInfo);
            }
                
        }
    }

}
