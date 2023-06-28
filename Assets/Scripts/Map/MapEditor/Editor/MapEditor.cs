using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.Tilemaps;

namespace QT.Map
{
    public class MapEditor : EditorWindow
    {
        private const string LevelToolScenePath = "Assets/Scenes/MapEditScene.unity";
        private const string MapCellPrefabPath = "Assets/Scenes/MapEditScene.unity";
        private const string MapCellParentName = "MapCell";

        private MapCellData _target;

        private Transform _mapCellParent;

        [MenuItem("�� ������/�� ������ ����", false, 0)]
        public static void OpenMapEditor()
        {
            GetWindow(typeof(MapEditor));
        }

        private void Awake()
        {
            OpenLevelToolScene();
        }

        private void OnGUI()
        {
            if (EditorSceneManager.GetActiveScene().path != LevelToolScenePath)
            {
                if (GUILayout.Button("���� �� �� ����"))
                {
                    OpenLevelToolScene();
                }

                return;
            }

            if (_target == null)
            {
                EditorGUILayout.HelpBox("Ÿ���� �����ϴ�.", MessageType.Warning);
                GetTarget();

                GUILayout.Space(20);
                
                if (GUILayout.Button("���ο� MapCell �����"))
                {
                    
                }
                
                return;
            }

            
            if (GUILayout.Button("Ÿ�� �ȷ�Ʈ ����"))
            {
                EditorApplication.ExecuteMenuItem("Window/2D/Tile Palette");
            }
        }

        private void OpenLevelToolScene()
        {
            EditorSceneManager.OpenScene(LevelToolScenePath);

            _mapCellParent = GameObject.Find(MapCellParentName)?.transform;

            if (_mapCellParent == null)
            {
                _mapCellParent = new GameObject(MapCellParentName).transform;
            }
        }
        
        private void GetTarget()
        {
            var prefabStage = PrefabStageUtility.GetCurrentPrefabStage();

            if (prefabStage != null)
            {
                _target = prefabStage.FindComponentOfType<MapCellData>();
            }
            else
            {
                _target = FindObjectOfType<MapCellData>();
            }
        }

        private void ShowMapCells()
        {
        }
    }
}