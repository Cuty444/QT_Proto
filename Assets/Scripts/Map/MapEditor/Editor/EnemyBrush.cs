using System;
using QT.Core;
using QT.Map;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.Tilemaps;
using UnityEngine.SceneManagement;

namespace QT.Tilemaps
{
    [CustomGridBrush(true, false, false, "Enemy Brush")]
    public class EnemyBrush : GridBrush
    {
        [HideInInspector] public int EnemyId;

        [SerializeField] private Vector3 _offset = Vector3.zero;
        [SerializeField] private Vector3 _scale = Vector3.one;
        [SerializeField] private Quaternion _orientation = Quaternion.identity;
        
        public Vector3 m_Anchor = new Vector3(0.5f, 0.5f, 0.0f);
        
        
        public override void Paint(GridLayout grid, GameObject brushTarget, Vector3Int position)
        {
            Vector3Int min = position - pivot;
            BoundsInt bounds = new BoundsInt(min, Vector3Int.one);

            BoxFill(grid, brushTarget, bounds);
        }
        
        private void PaintCell(GridLayout grid, Vector3Int position, Transform parent)
        {
            var existingGO = GetObjectInCell(grid, parent, position, m_Anchor);
            if (existingGO == null)
            {
                SetSceneCell(grid, parent, position, _offset, _scale, _orientation, m_Anchor, EnemyId);
            }
        }

        public override void Erase(GridLayout gridLayout, GameObject brushTarget, Vector3Int position)
        {
            Vector3Int min = position - pivot;
            BoundsInt bounds = new BoundsInt(min, Vector3Int.one);

            GetGrid(ref gridLayout, ref brushTarget);
            BoxErase(gridLayout, brushTarget, bounds);
        }
        
        private void EraseCell(GridLayout grid, Vector3Int position, Transform parent)
        {
            ClearSceneCell(grid, parent, position);
        }

        public override void BoxFill(GridLayout gridLayout, GameObject brushTarget, BoundsInt position)
        {
            GetGrid(ref gridLayout, ref brushTarget);
            
            foreach (Vector3Int location in position.allPositionsWithin)
            {
                Vector3Int local = location - position.min;
                PaintCell(gridLayout, location, brushTarget != null ? brushTarget.transform : null);
            }
        }
        
        public override void BoxErase(GridLayout gridLayout, GameObject brushTarget, BoundsInt position)
        {
            GetGrid(ref gridLayout, ref brushTarget);
            
            foreach (Vector3Int location in position.allPositionsWithin)
            {
                EraseCell(gridLayout, location, brushTarget != null ? brushTarget.transform : null);
            }
        }
        
        private void GetGrid(ref GridLayout gridLayout, ref GameObject brushTarget)
        {
            if (brushTarget != null)
            {
                var targetGridLayout = brushTarget.GetComponent<GridLayout>();
                if (targetGridLayout != null)
                    gridLayout = targetGridLayout;
            }
        }
        
        private static void SetSceneCell(GridLayout grid, Transform parent, Vector3Int position, Vector3 offset, Vector3 scale, Quaternion orientation, Vector3 anchor, int EnemyId)
        {
            GameObject instance = new GameObject("Enemy");
            instance.transform.parent = parent;

            instance.AddComponent<EnemySpawner>().EnemyId = EnemyId;
            
            
            Undo.RegisterCreatedObjectUndo(instance, "Paint Enemy");
            var anchorRatio = GetAnchorRatio(grid, anchor);
            
            instance.transform.position = grid.LocalToWorld(grid.CellToLocalInterpolated(position) + grid.CellToLocalInterpolated(anchorRatio));
            instance.transform.localRotation = orientation;
            instance.transform.localScale = scale;
            instance.transform.Translate(offset);
        }
        
        private void ClearSceneCell(GridLayout grid, Transform parent, Vector3Int position)
        {
            var erased = GetObjectInCell(grid, parent, position, m_Anchor);
            if (erased != null)
                Undo.DestroyObjectImmediate(erased);
        }

        private GameObject GetObjectInCell(GridLayout grid, Transform parent, Vector3Int position, Vector3 anchor)
        {
            int childCount;
            GameObject[] sceneChildren = null;
            if (parent == null)
            {
                var scene = SceneManager.GetActiveScene();
                sceneChildren = scene.GetRootGameObjects();
                childCount = scene.rootCount;
            }
            else
            {
                childCount = parent.childCount;
            }

            var anchorRatio = GetAnchorRatio(grid, anchor);
            var anchorWorld = grid.CellToLocalInterpolated(anchorRatio);
            for (var i = 0; i < childCount; i++)
            {
                var child = sceneChildren == null ? parent.GetChild(i) : sceneChildren[i].transform;
                var childCell = grid.LocalToCell(grid.WorldToLocal(child.position) - anchorWorld);
                if (position == childCell)
                    return child.gameObject;
            }
            return null;
        }
        
        private static Vector3 GetAnchorRatio(GridLayout grid, Vector3 cellAnchor)
        {
            var cellSize = grid.cellSize;
            var cellStride = cellSize + grid.cellGap;
            cellStride.x = Mathf.Approximately(0f, cellStride.x) ? 1f : cellStride.x;
            cellStride.y = Mathf.Approximately(0f, cellStride.y) ? 1f : cellStride.y;
            cellStride.z = Mathf.Approximately(0f, cellStride.z) ? 1f : cellStride.z;
            var anchorRatio = new Vector3(
                cellAnchor.x * cellSize.x / cellStride.x,
                cellAnchor.y * cellSize.y / cellStride.y,
                cellAnchor.z * cellSize.z / cellStride.z
            );
            return anchorRatio;
        }

    }

    [CustomEditor(typeof(EnemyBrush))]
    public class EnemyBrushEditor : GridBrushEditor
    {
        private EnemyBrush _enemyBrush => target as EnemyBrush;
        private bool _isFoldout = false;

        public override void OnPaintSceneGUI(GridLayout gridLayout, GameObject brushTarget, BoundsInt position, GridBrushBase.Tool tool, bool executing)
        {
            BoundsInt gizmoRect = position;

            if (tool == GridBrushBase.Tool.Paint || tool == GridBrushBase.Tool.Erase)
            {
                gizmoRect = new BoundsInt(position.min - brush.pivot, brush.size);
            }

            base.OnPaintSceneGUI(gridLayout, brushTarget, gizmoRect, tool, executing);
        }

        
        public override void OnPaintInspectorGUI()
        {
            if (GridPaintingState.scenePaintTarget != null)
            {
                var wave = GridPaintingState.scenePaintTarget.GetComponent<EnemyWave>();
                if (wave != null)
                {
                    EditorGUILayout.Space(5);
                    
                    Rect rect = EditorGUILayout.GetControlRect(false, 15);
                    rect.height = 15;
                    EditorGUI.DrawRect(rect, wave.WaveColor);

                    EditorGUILayout.Space(3);
                }

            }
            
            EditorGUILayout.BeginHorizontal();
            _enemyBrush.EnemyId = EditorGUILayout.IntField("적 아이디", _enemyBrush.EnemyId, GUILayout.Width(200), GUILayout.ExpandWidth(true));

            
            if (EditorSystemManager.Instance.DataManager.IsInitialized)
            {
                var dataBase = EditorSystemManager.Instance.DataManager.GetDataBase<EnemyGameDataBase>();
                
                var ids = dataBase.Ids;
                var names = dataBase.Names;

                _enemyBrush.EnemyId = EditorGUILayout.IntPopup(_enemyBrush.EnemyId, names, ids);
            }
            
            EditorGUILayout.EndHorizontal();
            
            GUILayout.Space(20);
            
            _isFoldout = EditorGUILayout.Foldout(_isFoldout, "기타 설정");
            if (_isFoldout)
            {
                base.OnPaintInspectorGUI();
            }
        }
        
        public override GameObject[] validTargets
        {
            get
            {
                StageHandle currentStageHandle = StageUtility.GetCurrentStageHandle();
                var mapcell = currentStageHandle.FindComponentOfType<MapCellData>();

                if(mapcell == null || mapcell.EnemyLayer == null)
                    return Array.Empty<GameObject>();


                var result = new GameObject[mapcell.EnemyLayer.childCount];
                
                for (int i = 0; i < result.Length; i++)
                    result[i] = mapcell.EnemyLayer.GetChild(i).gameObject;

                return result;
            }
        }
    }
}