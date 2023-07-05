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

        private Palette _palette;

        private int _wallHeight = 4;
        private Color _deactivatedColor = new Color(0.8f, 0.8f, 0.8f, 0.8f);

        [MenuItem("맵 에디터/맵 에디터 열기", false, 0)]
        public static void OpenMapEditor()
        {
            GetWindow(typeof(MapEditor));
        }

        private void Awake()
        {
            OpenLevelToolScene();
            SetPalette(GridPaintingState.palette);
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

            if (!GridPaintingState.isEditing)
            {
                if (GUILayout.Button("타일 팔레트 열기"))
                {
                    EditorApplication.ExecuteMenuItem("Window/2D/Tile Palette");
                }
            }
            else
            {
                SetPalette(GridPaintingState.palette);
            }

            GUILayout.Space(5);
            GUILayout.BeginHorizontal();

            if (_prefabStage == null)
            {
                if (GUILayout.Button("저장", GUILayout.Width(100), GUILayout.ExpandWidth(true)))
                {
                }

                if (GUILayout.Button("다른 이름으로 저장", GUILayout.Width(100), GUILayout.ExpandWidth(true)))
                {
                }
            }

            GUILayout.EndHorizontal();

            
            GUILayout.Space(20);
            
            _deactivatedColor = EditorGUILayout.ColorField("비 활성화된 레이어 색상", _deactivatedColor);

            GUILayout.Space(5);
            
            _wallHeight = EditorGUILayout.IntSlider("벽 높이", _wallHeight, 0, 10);
            
            if (GUILayout.Button("Top 타일맵 리셋"))
            {
                ResetTilemapTop();
            }
        }

        private void SetEvents()
        {
            GridPaintingState.paletteChanged -= SetPalette;
            GridPaintingState.paletteChanged += SetPalette;

            GridPaintingState.scenePaintTargetChanged -= CheckCurrentTileMap;
            GridPaintingState.scenePaintTargetChanged += CheckCurrentTileMap;

            Tilemap.tilemapTileChanged -= OnTilemapTileChanged;
            Tilemap.tilemapTileChanged += OnTilemapTileChanged;

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


        private void OnTilemapTileChanged(Tilemap target, Tilemap.SyncTile[] tiles)
        {
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
    }
}