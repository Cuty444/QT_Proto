using System;
using System.Collections.Generic;
using System.Linq;
using QT.Tilemaps;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;
using UnityEditor.Tilemaps;
using UnityEditor.EditorTools;
using UnityEditor.SceneManagement;
using UnityEngine.Rendering;

namespace QT.Map
{
    public class MapEditor : EditorWindow
    {
        private const string LevelToolScenePath = "Assets/Scenes/RDScene/MapEditScene.unity";
        private const string MapCellPrefabPath = "Assets/Scripts/Map/MapEditor/MapCell.prefab";

        private MapCellData _target;
        private Tilemap _floorTilemap;

        private MapEditorSceneManager _sceneManager;
        private PrefabStage _prefabStage;
        
        private Volume _volume;

        private int _wallHeight = 3;
        private Color _deactivatedColor = new Color(0.8f, 0.8f, 0.8f, 0.2f);

        private bool _isPrefab = false;

        private string _command;
        
        private Palette _palette;
        private bool _isTileEditing;

        private TestStartDoor _startDoor;

        [MenuItem("맵 에디터/맵 에디터 열기", false, 0)]
        public static void OpenMapEditor()
        {
            GetWindow(typeof(MapEditor));
        }

        private void Awake()
        {
            //OpenLevelToolScene();
            SetPalette(GridPaintingState.palette);
        }

        private void OnValidate()
        {
            SetEvents();
            
            _volume = FindObjectOfType<Volume>();
        }

        private void Update()
        {
            if (!Application.isPlaying)
            {
                _sceneManager?.CheckTarget();
            }
        }

        private void OnGUI()
        {
            if (Application.isPlaying)
            {
                EditorGUILayout.HelpBox("플레이 모드에서는 맵 에디터 기능을 사용할 수 없습니다!", MessageType.Warning);
                return;
            }

            if (!CheckScene())
            {
                return;
            }
            
            if (_target == null && !CheckMapCell())
            {
                EditorGUILayout.HelpBox("씬에 편집할 MapCell이 없습니다.\nMapCell 프리팹을 씬에 배치해주세요!", MessageType.Warning);
                
                if (GUILayout.Button("새로운 MapCell 만들기"))
                {
                    var target = PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>(MapCellPrefabPath)) as GameObject;
                    PrefabUtility.UnpackPrefabInstance(target, PrefabUnpackMode.OutermostRoot, InteractionMode.UserAction);
                }

                return;
            }

            if (!GridPaintingState.isEditing)
            {
                if (GUILayout.Button("타일 팔레트 열기"))
                {
                    EditorApplication.ExecuteMenuItem("Window/2D/Tile Palette");
                    SetPalette(GridPaintingState.palette);
                }
            }
            
            SavePrefab();
            
            GUILayout.Space(10);
            
            
            _target.CameraSize = EditorGUILayout.FloatField("카메라 사이즈", _target.CameraSize);
            _target.VolumeProfile = EditorGUILayout.ObjectField("MapCell 볼륨 프로필", _target.VolumeProfile, typeof(VolumeProfile), false) as VolumeProfile;
            if (!_volume)
            {
                _volume = FindObjectOfType<Volume>();
            }
            else
            {
                _volume.profile = _target.VolumeProfile;
            }

            // GUILayout.Space(15);
            // GUILayout.BeginHorizontal();
            //
            // if (GUILayout.Button("기본 브러시", GUILayout.Width(100), GUILayout.ExpandWidth(true)))
            // {
            //     GridPaintingState.gridBrush = GridPaintingState.brushes.First((x)=> x is GridBrush);
            // }
            //
            // if (GUILayout.Button("적 브러시", GUILayout.Width(100), GUILayout.ExpandWidth(true)))
            // {
            //     GridPaintingState.gridBrush = GridPaintingState.brushes.First((x)=> x is EnemyBrush);
            // }
            //
            // GUILayout.EndHorizontal();
            
            GUILayout.Space(15);
            
            _deactivatedColor = EditorGUILayout.ColorField("비 활성화된 레이어 색상", _deactivatedColor);

            GUILayout.Space(5);
            
            _wallHeight = EditorGUILayout.IntSlider("벽 높이", _wallHeight, 0, 10);
            
            if (GUILayout.Button("Top 타일맵 리셋"))
            {
                ResetTilemapTop();
            }
            
            GUILayout.Space(20);

            GUILayout.Label("디버그 커맨드");
            _command = EditorGUILayout.TextArea(_command,GUILayout.Height(60));

            GUILayout.Space(5);

            _startDoor = (TestStartDoor)EditorGUILayout.EnumPopup("테스트 시작 위치",  _startDoor);
            
            
            if (GUILayout.Button("플레이 테스트"))
            {
                _sceneManager.StartGame(_command, _startDoor);
            }
            
            GUILayout.Space(10);
            
            if(_floorTilemap)
                GUILayout.Label($"가로 : {_floorTilemap.cellBounds.size.x} | 세로 : {_floorTilemap.cellBounds.size.y}");
        }

        private void SetEvents()
        {
            GridPaintingState.paletteChanged -= SetPalette;
            GridPaintingState.paletteChanged += SetPalette;

            GridPaintingState.scenePaintTargetChanged -= CheckCurrentTileMap;
            GridPaintingState.scenePaintTargetChanged += CheckCurrentTileMap;

            Tilemap.tilemapTileChanged -= CheckHardColliderLayer;
            Tilemap.tilemapTileChanged += CheckHardColliderLayer;

            ToolManager.activeToolChanged -= OnToolChanged;
            ToolManager.activeToolChanged += OnToolChanged;
        }

        private void SetPalette(GameObject paletteObject)
        {
            _palette = paletteObject.GetComponent<Palette>();
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
            if (_prefabStage != null)
            {
                GetPrefabStageTarget();
            }
            else
            {
                _target = _sceneManager.Target;
                if (_target != null)
                {
                    _isPrefab = PrefabUtility.IsOutermostPrefabInstanceRoot(_target.gameObject);

                    foreach (var tileMap in _target.GetComponentsInChildren<Tilemap>())
                    {
                        SceneVisibilityManager.instance.DisablePicking(tileMap.gameObject, true);
                    }
                }
            }
            
            if(_target)
                _floorTilemap = _target.GetComponentsInChildren<Tilemap>().FirstOrDefault(x => x.gameObject.layer == LayerMask.NameToLayer("Fall"));

            return _target != null;
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
                _isPrefab = true;
            }
        }

        private void SavePrefab()
        {
            if (_prefabStage != null)
            {
                return;
            }
            
            GUILayout.Space(5);
            GUILayout.BeginHorizontal();
            
            if (_isPrefab && GUILayout.Button("저장", GUILayout.Width(100), GUILayout.ExpandWidth(true)))
            {
                CompressMapCell();
                PrefabUtility.ApplyPrefabInstance(_target.gameObject, InteractionMode.UserAction);
            }

            if (GUILayout.Button("다른 이름으로 저장", GUILayout.Width(100), GUILayout.ExpandWidth(true)))
            {
                var path = EditorUtility.SaveFilePanelInProject("맵 셀 저장", $"{_target.gameObject.name}.prefab", "prefab", "MapCell 저장");

                if (!string.IsNullOrWhiteSpace(path))
                {
                    CompressMapCell();
                    
                    if(PrefabUtility.IsOutermostPrefabInstanceRoot(_target.gameObject))
                    {
                        PrefabUtility.UnpackPrefabInstance(_target.gameObject, PrefabUnpackMode.OutermostRoot, InteractionMode.UserAction);
                    }
                    
                    var target = PrefabUtility.SaveAsPrefabAssetAndConnect(_target.gameObject, path, InteractionMode.UserAction);
                    AssetDatabase.SaveAssets();
        
                    EditorUtility.FocusProjectWindow();
                    Selection.activeObject = target;
                        
                    _isPrefab = true;

                    _target.gameObject.name = path.Split('\\', '/', '.')[^2];
                }
            }
            GUILayout.EndHorizontal();
        }

        private void CompressMapCell()
        {
            var targets = FindObjectsOfType<Tilemap>();

            foreach (var target in targets)
            {
                target.CompressBounds();
            }
        }
        
        private void ResetTilemapTop()
        {
            TileChangeData CreateTileData(Vector3Int pos, TileBase tile)
            {
                return new TileChangeData(pos, tile, Color.white, Matrix4x4.identity);
            }

            Undo.RecordObject(_target.TilemapTop, "Reset Top");

            var targetMap = _target.TilemapHardCollider;
            var bound = targetMap.cellBounds;
            var datas = new List<TileChangeData>();

            _target.TilemapTop.ClearAllTiles();

            for (int x = bound.xMin; x < bound.xMax; x++)
            {
                for (int y = bound.yMax; y >= bound.yMin; y--)
                {
                    var pos = new Vector3Int(x, y);

                    if (!targetMap.HasTile(pos))
                    {
                        continue;
                    }

                    if (!targetMap.HasTile(pos + Vector3Int.up))
                    {
                        datas.Add(CreateTileData(pos + Vector3Int.up, _palette.TopTile));
                    }

                    if (!targetMap.HasTile(pos + Vector3Int.left) ||
                        !targetMap.HasTile(pos + Vector3Int.right))
                    {
                        bool heightCheck = true;

                        var checkPos = pos;
                        for (int i = 0; i < _wallHeight; i++)
                        {
                            checkPos += Vector3Int.down;
                            if (!targetMap.HasTile(checkPos))
                            {
                                heightCheck = false;
                                break;
                            }
                        }

                        if (heightCheck)
                        {
                            datas.Add(CreateTileData(pos, _palette.TopTile));
                        }
                    }
                }
            }
            
            _target.TilemapTop.SetTiles(datas.ToArray(), false);
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
                    target.color = Color.white;
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
                target.color = target.gameObject == currentTilemap ? Color.white : _deactivatedColor;
            }
        }


        private void CheckHardColliderLayer(Tilemap target, Tilemap.SyncTile[] tiles)
        {
            if (!CheckMapCell())
            {
                return;
            }
            
            if (target == _target.TilemapHardCollider)
            {
                TileChangeData CreateTileData(Vector3Int pos, TileBase tile)
                {
                    return new TileChangeData(pos, tile, Color.white, Matrix4x4.identity);
                }

                var datas = new List<TileChangeData>();

                // 위가 비어있거나 아래로 부터 n칸 이후, 한 사이드가 비어있으면 Top에 타일 추가
                foreach (var tile in tiles)
                {
                    bool isAdd = tile.tile != null;

                    if (!target.HasTile(tile.position + Vector3Int.up))
                    {
                        datas.Add(CreateTileData(tile.position + Vector3Int.up, isAdd ? _palette.TopTile : null));
                    }

                    if (isAdd && (!target.HasTile(tile.position + Vector3Int.left) ||
                                  !target.HasTile(tile.position + Vector3Int.right)))
                    {
                        bool heightCheck = true;

                        var pos = tile.position;
                        for (int i = 0; i < _wallHeight; i++)
                        {
                            pos += Vector3Int.down;
                            if (!target.HasTile(pos))
                            {
                                heightCheck = false;
                                break;
                            }
                        }

                        datas.Add(CreateTileData(tile.position, heightCheck ? _palette.TopTile : null));
                    }
                    else
                    {
                        datas.Add(CreateTileData(tile.position, null));
                    }
                }

                Undo.RecordObject(_target.TilemapTop, "Set Top");
                
                _target.TilemapTop.SetTiles(datas.ToArray(), false);
            }
        }
        
    }
}