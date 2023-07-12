using System;
using System.Collections;
using System.Collections.Generic;
using QT.Core;
using UnityEngine;
using UnityEditor;
using UnityEngine.Tilemaps;
using UnityEditor.Tilemaps;
using UnityEngine.SceneManagement;

namespace QT.Tilemaps
{
    [CustomGridBrush(true, false, false, "Enemy Brush")]
    public class EnemyBrush : GridBrush
    {
        public override void Paint(GridLayout grid, GameObject brushTarget, Vector3Int position)
        {
            base.Paint(grid, brushTarget, position);
        }
        
        
        
        private static void SetSceneCell(GridLayout grid, Transform parent, Vector3Int position, GameObject go, Vector3 offset, Vector3 scale, Quaternion orientation, Vector3 anchor)
        {
            if (go == null)
                return;

            GameObject instance;
            if (PrefabUtility.IsPartOfPrefabAsset(go))
            {
                instance = (GameObject) PrefabUtility.InstantiatePrefab(go, parent != null ? parent.root.gameObject.scene : SceneManager.GetActiveScene());
                instance.transform.parent = parent;
            }
            else
            {
                instance = Instantiate(go, parent);
                instance.name = go.name;
                instance.SetActive(true);
                foreach (var renderer in instance.GetComponentsInChildren<Renderer>())
                {
                    renderer.enabled = true;
                }
            }

            Undo.RegisterCreatedObjectUndo(instance, "Paint Enemy");
            var anchorRatio = GetAnchorRatio(grid, anchor);
            
            instance.transform.position = grid.LocalToWorld(grid.CellToLocalInterpolated(position) + grid.CellToLocalInterpolated(anchorRatio));
            instance.transform.localRotation = orientation;
            instance.transform.localScale = scale;
            instance.transform.Translate(offset);
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
            //Debug.LogError(SystemManager.Instance.DataManager.GetDataBase<EnemyGameDataBase>().GetData(500).Index);
        }
    }
}