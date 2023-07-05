using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Tilemaps;

using UnityEditor;
using UnityEditor.Tilemaps;
using UnityEditor.EditorTools;
using UnityEditor.SceneManagement;

namespace QT.Map
{
    public class MapEditor : EditorWindow
    {
        private const string LevelToolScenePath = "Assets/Scenes/RDScene/MapEditScene.unity";
        private const string MapCellPrefabPath = "Assets/Scenes/MapEditScene.unity";

        private MapCellData _target;

        private MapEditorSceneManager _sceneManager;
        private PrefabStage _prefabStage;

        private bool _isTileEditing = false;
        

        [MenuItem("맵 에디터/맵 에디터 열기", false, 0)]
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
            if (Application.isPlaying)
            {
                EditorGUILayout.HelpBox("플레이 모드에서는 맵 에디터 기능을 사용할 수 없습니다!", MessageType.Warning);
                return;
            }
            
            SetEvents();
            
            if (!CheckScene())
            {
                return;
            }
            
            if (!CheckMapCell())
            {
                return;
            }
            
            if (GUILayout.Button("타일 팔레트 열기"))
            {
                EditorApplication.ExecuteMenuItem("Window/2D/Tile Palette");
            }
            
            GUILayout.Space(5);
            GUILayout.BeginHorizontal( );
            
            if (_prefabStage == null)
            { 
                if (GUILayout.Button("저장", GUILayout.Width(100),GUILayout.ExpandWidth(true)))
                {
                }

                if (GUILayout.Button("다른 이름으로 저장",GUILayout.Width(100),GUILayout.ExpandWidth(true)))
                {
                }
            }

            GUILayout.EndHorizontal();
        }

        private void SetEvents()
        {
            GridPaintingState.scenePaintTargetChanged -= CheckCurrentTileMap;
            GridPaintingState.scenePaintTargetChanged += CheckCurrentTileMap;
            
            ToolManager.activeToolChanged -= OnToolChanged;
            ToolManager.activeToolChanged += OnToolChanged;
        }


        private bool CheckScene()
        {
            _prefabStage = PrefabStageUtility.GetCurrentPrefabStage();
            
            if (_prefabStage == null && EditorSceneManager.GetActiveScene().path != LevelToolScenePath)
            {
                if (GUILayout.Button("레벨 툴 씬 열기"))
                {
                    OpenLevelToolScene();
                }
                return false;
            }

            if (_sceneManager == null)
            {
                _sceneManager = FindObjectOfType<MapEditorSceneManager>();
            }

            return true;
        }

        private bool CheckMapCell()
        {
            if (_target == null)
            {
                EditorGUILayout.HelpBox("씬에 편집할 MapCell이 없습니다.\nMapCell 프리팹을 씬에 배치해주세요!", MessageType.Warning);
                
                if (_prefabStage != null)
                {
                    GetPrefabStageTarget();
                }
                else
                {
                    GUILayout.Space(20);
                
                    _target = _sceneManager.Target;
                    
                    if (GUILayout.Button("새로운 MapCell 만들기"))
                    {
                    
                    }
                }
                return false;
            }

            return true;
        }
        
        
        private void OpenLevelToolScene()
        {
            if (EditorSceneManager.GetActiveScene().path != LevelToolScenePath)
            {
                EditorSceneManager.OpenScene(LevelToolScenePath);
            }

            _sceneManager = FindObjectOfType<MapEditorSceneManager>();
        }
        
        private void GetPrefabStageTarget()
        {
            if (_prefabStage != null)
            {
                _target = _prefabStage.FindComponentOfType<MapCellData>();
            }
        }

        private void OnToolChanged()
        {
            _isTileEditing = ToolManager.activeToolType.IsSubclassOf(typeof(TilemapEditorTool));

            if (_isTileEditing)
            {
                CheckCurrentTileMap(GridPaintingState.scenePaintTarget);
            }
            else
            {
                var targets = FindObjectsOfType<Tilemap>();
            
                foreach (var target in targets)
                {
                    var color = target.color;
                    color.a = 1;
                
                    target.color = color;
                }
            }
        }
        
        private void CheckCurrentTileMap(GameObject currentTilemap)
        {
            if (!_isTileEditing)
            {
                return;
            }
            
            var targets = FindObjectsOfType<Tilemap>();
            
            foreach (var target in targets)
            {
                var color = target.color;
                color.a = target.gameObject == currentTilemap ? 1 : 0.5f;
                
                target.color = color;
            }
        }

        private void ShowMapCells()
        {
            Debug.Log(UnityEditor.Tilemaps.GridSelection.active);
        }
    }
}