using System;
using System.Collections.Generic;
using System.Linq;
using QT.Tilemaps;
using QT.Util;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;
using UnityEditor.Tilemaps;
using UnityEditor.EditorTools;
using UnityEditor.SceneManagement;
using UnityEngine.Rendering;

using Random = UnityEngine.Random;

namespace QT.Map
{
    public class MapEditor : EditorWindow
    {
        private const string LevelToolScenePath = "Assets/Scenes/RDScene/MapEditScene.unity";
        private const string MapCellPrefabPath = "Assets/Scripts/Map/MapEditor/MapCell.prefab";

        private MapCellData _target;

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

        private Vector2 _scroll;

        private bool _isDirty = false;

        [MenuItem("맵 에디터/맵 에디터 열기", false, 0)]
        public static void OpenMapEditor()
        {
            GetWindow(typeof(MapEditor));
        }
        
        
        [MenuItem("맵 에디터/타일 팔레트 열기", false, 0)]
        public static void OpenTilePalette()
        {
            EditorApplication.ExecuteMenuItem("Window/2D/Tile Palette");
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
                
                if (_isDirty)
                {
                    ResetWall();
                    _isDirty = false;
                }
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

            SavePrefab();
            
            GUILayout.Space(5);
            
            if (!GridPaintingState.isEditing)
            {
                if (GUILayout.Button("타일 팔레트 열기"))
                {
                    OpenTilePalette();
                    SetPalette(GridPaintingState.palette);
                }
            }
            else
            {
                GUILayout.BeginHorizontal();
            
                if (GUILayout.Button("기본 브러시", GUILayout.Width(100), GUILayout.ExpandWidth(true)))
                {
                    GridPaintingState.gridBrush = GridPaintingState.brushes.First((x)=> x is GridBrush);
                }
            
                if (GUILayout.Button("적 브러시", GUILayout.Width(100), GUILayout.ExpandWidth(true)))
                {
                    GridPaintingState.gridBrush = GridPaintingState.brushes.First((x)=> x is EnemyBrush);
                }
            
                GUILayout.EndHorizontal();
            }

            DrawLine("",2);
            
            _scroll = EditorGUILayout.BeginScrollView(_scroll);

            GUILayout.Label("방 환경 설정", EditorStyles.whiteLargeLabel);
            GUILayout.Space(7);
            
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

            EditorGUILayout.Space(20);
            
            _deactivatedColor = EditorGUILayout.ColorField("비 활성화된 레이어 색상", _deactivatedColor);

            GUILayout.Space(5);
            
            _wallHeight = EditorGUILayout.IntSlider("벽 높이", _wallHeight, 0, 10);
            
            if (GUILayout.Button("Top 타일맵 리셋"))
            {
                ResetTilemapTop();
            }
            
            DrawLine("적 웨이브 세팅");
            
            ControlEnemyWaves();
            
            DrawLine("테스트");
            

            GUILayout.Label("디버그 커맨드");
            _command = EditorGUILayout.TextArea(_command,GUILayout.Height(60));

            GUILayout.Space(5);

            _startDoor = (TestStartDoor)EditorGUILayout.EnumPopup("테스트 시작 위치",  _startDoor);
            
            
            if (GUILayout.Button("플레이 테스트"))
            {
                _target.TilemapHardCollider.enabled = false;
                _sceneManager.StartGame(_command, _startDoor);
            }

            GUILayout.Space(10);

            EditorGUILayout.EndScrollView();
        }

        private void SetEvents()
        {
            GridPaintingState.paletteChanged -= SetPalette;
            GridPaintingState.paletteChanged += SetPalette;

            GridPaintingState.scenePaintTargetChanged -= CheckCurrentTileMap;
            GridPaintingState.scenePaintTargetChanged += CheckCurrentTileMap;

            Tilemap.tilemapTileChanged -= CheckTileChanged;
            Tilemap.tilemapTileChanged += CheckTileChanged;

            ToolManager.activeToolChanged -= OnToolChanged;
            ToolManager.activeToolChanged += OnToolChanged;
        }

        private void SetPalette(GameObject paletteObject)
        {
            if(paletteObject)
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
            
            SetEvents();

            return true;
        }

        private bool CheckMapCell()
        {
            if (_prefabStage != null)
            {
                GetPrefabStageTarget();
            }
            else if (_sceneManager != null)
            {
                _target = _sceneManager.Target;
                
                if (_target != null)
                {
                    _isPrefab = PrefabUtility.IsOutermostPrefabInstanceRoot(_target.gameObject);
                    foreach (var tileMap in _target.GetComponentsInChildren<Tilemap>())
                    {
                        SceneVisibilityManager.instance.DisablePicking(tileMap.gameObject, true);
                    }
                    
                    SceneVisibilityManager.instance.DisablePicking(_sceneManager.gameObject, true);
                
                    if (_target.TilemapHardCollider)
                    {
                        _target.TilemapHardCollider.enabled = true;
                    }
                }
            }
            
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
            
            
            if (_target.TilemapHardCollider)
            {
                _target.TilemapHardCollider.enabled = true;
            }
            
            GUILayout.EndHorizontal();
        }

        private void CompressMapCell()
        {
            _target.transform.position = Vector3.zero;
            var targets = FindObjectsOfType<Tilemap>();

            foreach (var target in targets)
            {
                target.color = Color.white;
                target.CompressBounds();
            }
            
            if (_target.TilemapHardCollider)
            {
                _target.TilemapHardCollider.enabled = false;
            }
        }

        private void ResetTilemapTop()
        {
            Undo.RecordObject(_target.TilemapTop, "Reset Top");

            var targetMap = _target.TilemapWall;
            var bound = targetMap.cellBounds;
            var data = new List<TileChangeData>();

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
                        data.Add(new TileChangeData(pos + Vector3Int.up, _palette.TopTile, Color.white, Matrix4x4.identity));
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
                            data.Add(new TileChangeData(pos, _palette.TopTile, Color.white, Matrix4x4.identity));
                        }
                    }
                }
            }
            
            _target.TilemapTop.SetTiles(data.ToArray(), false);
        }

        private void ResetWall()
        {
            if(_target == null || _target.TilemapWall == null)
                return;
            
            var targetMap = _target.TilemapWall;
            var targetRenderer = targetMap.GetComponent<TilemapRenderer>();

            var targets = FindObjectsOfType<Tilemap>().Where(x =>
                x.gameObject.TryGetComponent<TilemapRenderer>(out var renderer) &&
                renderer != _target.TilemapHardCollider &&
                renderer.sortOrder == targetRenderer.sortOrder &&
                renderer.sortingOrder >= targetRenderer.sortingOrder).ToArray();
            
            var data = new List<TileChangeData>[targets.Length];

            var bound = targetMap.cellBounds;
            var level = 0;

            for (int x = bound.xMin; x < bound.xMax; x++)
            {
                for (int y = bound.yMin; y <= bound.yMax; y++)
                {
                    var pos = new Vector3Int(x, y);

                    var transform = (level == _wallHeight) ? 
                        Matrix4x4.Translate(new Vector3(0, 0, _wallHeight * -1 + 0.5f)) : 
                        Matrix4x4.TRS(new Vector3(0, 0, level * -1), Quaternion.Euler(-45, 0, 0), new Vector3(1, 1.415f, 1));

                    for (var i = 0; i < targets.Length; i++)
                    {
                        var tile = targets[i].GetTile(pos);

                        if (tile != null)
                        {
                            data[i] ??= new List<TileChangeData>();
                            data[i].Add(new TileChangeData(pos, tile, Color.white, transform));
                        }
                    }
                    
                    level = targetMap.HasTile(pos) ? Mathf.Clamp(level + 1, 0, _wallHeight) : 0;
                }
                level = 0;
            }

            for (var i = 0; i < targets.Length; i++)
            {
                if (data[i] != null)
                {
                    targets[i].SetTiles(data[i].ToArray(), false);
                }
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


        private void CheckTileChanged(Tilemap target, Tilemap.SyncTile[] tiles)
        {
            if (!CheckMapCell())
            {
                return;
            }
            
            if (target == _target.TilemapWall && _palette != null)
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

            _isDirty = true;
        }
        
        
        #region EnemyWave

        private static GUILayoutOption _miniButtonWidth = GUILayout.Width(20f);
        
        private static GUIContent
            _moveDownButtonContent = new ("▼", "아래로 내리기"),
            _moveUpButtonContent = new ("▲", "위로 올리기"),
            _deleteButtonContent = new ("-", "삭제"),
            _addButtonContent = new ("+", "웨이브 추가");


        private void ControlEnemyWaves()
        {
            bool isChanged = false;
            var targetSerializedObject = new SerializedObject(_target);
            
            targetSerializedObject.Update();
            ShowElements(targetSerializedObject.FindProperty("EnemyWaves"));
            
            isChanged = targetSerializedObject.hasModifiedProperties;

            if (isChanged)
            {
                targetSerializedObject.ApplyModifiedProperties();
            }

            _target.EnemyWaves.Distinct();

            for (var i = 0; i < _target.EnemyWaves.Length; i++)
            {
                var wave = _target.EnemyWaves[i];
                
                if (wave == null)
                {
                    continue;
                }

                var lastName = wave.gameObject.name;
                wave.gameObject.name = $"Wave{i + 1}";

                isChanged |= lastName != wave.gameObject.name;
                
                wave.transform.SetAsLastSibling();
                
                wave.NextWave = i + 1 < _target.EnemyWaves.Length ? _target.EnemyWaves[i + 1] : null;
            }
            
            if (isChanged && GridPaintingState.gridBrush is EnemyBrush)
            {
                GridPaintingState.gridBrush = GridPaintingState.brushes.First((x)=> x is GridBrush);
                GridPaintingState.gridBrush = GridPaintingState.brushes.First((x)=> x is EnemyBrush);
            }
        }
        
        private void ShowElements (SerializedProperty list)
        {
            if (!list.isExpanded)
            {
                return;
            }
            
            for (int i = 0; i < list.arraySize; i++) 
            {
                EditorGUILayout.BeginHorizontal();
                
                var property = list.GetArrayElementAtIndex(i);
                var targetWaveObject = (EnemyWave)property.objectReferenceValue;
                
                if (targetWaveObject != null)
                {
                    GUILayout.Label($"웨이브 {i + 1}", GUILayout.Width(55));
                    GUILayout.Label($"Count : {targetWaveObject.transform.childCount}", EditorStyles.whiteLabel, GUILayout.Width(70));
                    
                    targetWaveObject.WaveColor = EditorGUILayout.ColorField(targetWaveObject.WaveColor, GUILayout.Width(50f));
                }
                else
                {
                    GUILayout.Label($"웨이브 {i} ", GUILayout.Width(125));
                }

                GUI.enabled = false;
                EditorGUILayout.PropertyField(property, GUIContent.none);
                GUI.enabled = true;
                
                ShowButtons(list, targetWaveObject, i);
                
                EditorGUILayout.EndHorizontal();
                
                
                Rect rect = EditorGUILayout.GetControlRect(false, 1 );
                rect.height = 1;
                EditorGUI.DrawRect(rect, new Color(0.4f, 0.4f, 0.4f, 1));
            }
            
            if (GUILayout.Button(_addButtonContent, EditorStyles.miniButton)) 
            {
                list.arraySize += 1;
                list.GetArrayElementAtIndex(list.arraySize - 1).objectReferenceValue = CreateWave();
            }
        }
        
        private void ShowButtons (SerializedProperty list, EnemyWave target, int index) 
        {
            if (GUILayout.Button(_moveUpButtonContent, EditorStyles.miniButtonLeft, _miniButtonWidth)) 
            {
                list.MoveArrayElement(index, index - 1);
            }
            
            if (GUILayout.Button(_moveDownButtonContent, EditorStyles.miniButtonMid, _miniButtonWidth)) 
            {
                list.MoveArrayElement(index, index + 1);
            }
            
            if (GUILayout.Button(_deleteButtonContent, EditorStyles.miniButtonRight, _miniButtonWidth)) 
            {
                int oldSize = list.arraySize;
                list.DeleteArrayElementAtIndex(index);
                
                if (list.arraySize == oldSize) 
                {
                    list.DeleteArrayElementAtIndex(index);
                }

                if (target != null)
                {
                    DestroyImmediate(target.gameObject);
                }
            }
        }

        private EnemyWave CreateWave()
        {
            var waveObject = new GameObject();
            waveObject.transform.SetParent(_target.EnemyLayer);
            waveObject.transform.ResetLocalTransform();
            
            var wave = waveObject.AddComponent<EnemyWave>();

            wave.CellData = _target;
            wave.WaveColor = new Color(Random.Range(0.5f, 1f), Random.Range(0.5f, 1f), Random.Range(0.5f, 1f), 1);

            return wave;
        }


        #endregion
        
        
        private void DrawLine(string title = null, int height = 1)
        {
            GUILayout.Space(10);

            Rect rect = EditorGUILayout.GetControlRect(false, height );
            rect.height = height;
            EditorGUI.DrawRect(rect, new Color ( 0.5f,0.5f,0.5f, 1 ) );
            
            
            if (!string.IsNullOrEmpty(title))
            {
                GUILayout.Space(3);
                GUILayout.Label(title, EditorStyles.whiteLargeLabel);
                GUILayout.Space(7);
            }
            else
            {
                GUILayout.Space(10);
            }
        }
        
    }
}