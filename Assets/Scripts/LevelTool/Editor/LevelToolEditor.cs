using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
namespace QT.Level
{
    [CustomEditor(typeof(LevelTool))]
    public class LevelToolEditor : Editor
    {
        private Plane _plane;
        private LevelTool _levelTool;
        private bool _isRangeDrawing = false;
        private Vector2Int _startGeneratePosition;
        private Vector3 _startGenerateWorldPos;
        private Vector3 _endGenerateWorldPos;
        private void OnSceneGUI()
        {
            if(_levelTool == null)
            {
                Quaternion rotation = Quaternion.Euler(-90.0f, 0.0f, 0.0f);
                Vector3 rotatedNormal = rotation * Vector3.up;
                _plane = new Plane(rotatedNormal, 0);
                _levelTool = (LevelTool)target;
            }
            
            
            
            if (_levelTool.Option.Mode == ToolMode.Cell)
            {
                CellEditMode();
            }
            else if(_levelTool.Option.Mode == ToolMode.Range)
            {
                RangeEditMode();
                DrawRange();
            }
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            LevelTool levelTool = (LevelTool)target;

            if(_levelTool == null)
            {
                _levelTool = levelTool;
            }
            
            if (GUILayout.Button("Load"))
            {
                levelTool.Load();
            }
            
            if (GUILayout.Button("Save"))
            {
                levelTool.Save();
                AssetDatabase.Refresh();
                EditorUtility.SetDirty(levelTool.Data);
                AssetDatabase.SaveAssets();
                Debug.Log($"[세이브 완료] 파일 : {levelTool.Data.name}, 시간 : {System.DateTime.Now}");
            }
            
            if (GUILayout.Button("Reset"))
            {
                levelTool.ResetTile();
            }
            
            serializedObject.ApplyModifiedProperties();
        }


        void CellEditMode()
        {
            int currentId = GUIUtility.GetControlID(FocusType.Passive);
            Event current = Event.current;

            if (Event.current.button == 0)
            {
                if (Event.current.type == EventType.MouseDown ||
                    Event.current.type == EventType.MouseDrag)
                {
                    GUIUtility.hotControl = currentId;
                    current.Use();
                    _levelTool.AddTile(GetMousePosToTilePos());
                }
            }

            if (Event.current.button == 1)
            {
                if (Event.current.type == EventType.MouseDown ||
                    Event.current.type == EventType.MouseDrag)
                {
                    GUIUtility.hotControl = currentId;
                    current.Use();
                    _levelTool.RemoveTile(GetMousePosToTilePos());
                }
            }
        }
        
        void RangeEditMode()
        {
            int currentId = GUIUtility.GetControlID(FocusType.Passive);
            Event current = Event.current;

            if (Event.current.button == 0)
            {
                if(Event.current.type == EventType.MouseDown)
                {
                    GUIUtility.hotControl = currentId;
                    current.Use();
                    _startGeneratePosition = GetMousePosToTilePos();
                    _startGenerateWorldPos = GetMousePosToWorldPos();
                }

                if(Event.current.type == EventType.MouseDrag)
                {
                    _isRangeDrawing = true;
                    GUIUtility.hotControl = currentId;
                    current.Use();
                    _endGenerateWorldPos = GetMousePosToWorldPos();
                }

                if(Event.current.type == EventType.MouseUp)
                {
                    _isRangeDrawing = false;
                    GUIUtility.hotControl = currentId;
                    current.Use();
                    _levelTool.AddTileRange(_startGeneratePosition, GetMousePosToTilePos());

                    _startGenerateWorldPos = _endGenerateWorldPos = Vector3.zero;
                }
            }
            else if (Event.current.button == 1)
            {
                if(Event.current.type == EventType.MouseDown)
                {
                    GUIUtility.hotControl = currentId;
                    current.Use();
                    _startGeneratePosition = GetMousePosToTilePos();
                    _startGenerateWorldPos = GetMousePosToWorldPos();
                }

                if(Event.current.type == EventType.MouseDrag)
                {
                    _isRangeDrawing = true;
                    GUIUtility.hotControl = currentId;
                    current.Use();
                    _endGenerateWorldPos = GetMousePosToWorldPos();
                }

                if(Event.current.type == EventType.MouseUp)
                {
                    _isRangeDrawing = false;
                    GUIUtility.hotControl = currentId;
                    current.Use();
                    _levelTool.RemoveTileRange(_startGeneratePosition, GetMousePosToTilePos());

                    _startGenerateWorldPos = _endGenerateWorldPos = Vector3.zero;
                }
            }
        }
        
        private Vector2Int GetMousePosToTilePos()
        {
            Vector2Int tilePos = Vector2Int.zero;

            Vector3 mousePos = Event.current.mousePosition;
            Ray ray = HandleUtility.GUIPointToWorldRay(mousePos);
            if (_plane.Raycast(ray, out var distance))
            {
                Vector3 worldPos = ray.GetPoint(distance);

                tilePos.x = Mathf.FloorToInt(worldPos.x);
                tilePos.y = Mathf.FloorToInt(worldPos.y);
            }
            return tilePos;
        }
        
        private Vector3 GetMousePosToWorldPos()
        {
            Vector3 worldPos = Vector3.zero;
            Vector3 mousePos = Event.current.mousePosition;
            Ray ray = HandleUtility.GUIPointToWorldRay(mousePos);

            if (_plane.Raycast(ray, out var distance))
            {
                worldPos = ray.GetPoint(distance);
            }

            return worldPos;
        }

        private void DrawRange()
        {
            if(!_isRangeDrawing)
            {
                return;
            }

            Vector3 size = _endGenerateWorldPos - _startGenerateWorldPos;

            Handles.DrawWireCube(_startGenerateWorldPos + size * 0.5f, size);
        }
    }

}
