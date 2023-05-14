using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace QT.Level
{
    [Serializable]
    public class ToolOption
    {
        public ToolMode Mode;
        public DrawLayer DrawLayer;
        public ColorOption ColorOption;
    }

    [Serializable]
    public class ColorOption
    {
        public Color Tile;
        public Color Wall;
        public Color CharacterCollider;
        public Color ProjectileCollider;
        public Color HardCollider;
        public Color PlayerSpawnPoint;
    }

    public enum ToolMode
    {
        Tile,
        Range,
    }

    public enum DrawLayer
    {
        Tile,
        Wall = 1 << 10,
        CharacterCollider = 1 << 14,
        ProjectileCollider = 1 << 18,
        HardCollider = 1 << 15,
        PlayerSpawnPoint = 1 << 8,
    }

    [ExecuteInEditMode]
    public class LevelTool : MonoBehaviour
    {
        [Header("옵션")]
        [SerializeField] private ToolOption _option;
        [Header("크기")]
        [SerializeField] private Vector2Int _generateSize;
        [Header("")]
        [Header("데이터")]
        [SerializeField] private LevelTestingData _data;


        private List<LevelTestingData.TileData> _tiles = new List<LevelTestingData.TileData>();
        public ToolOption Option => _option;
        public LevelTestingData Data => _data;

        private void Start()
        {
            if (_data == null)
                return;

            _tiles.Clear();

            _tiles.AddRange(_data.Tiles);
        }

        private void OnDisable()
        {
            Save();
            EditorUtility.SetDirty(Data);
        }

        private void OnDrawGizmos()
        {
            DrawTile();
        }

        void DrawTile()
        {
            foreach (var tile in _tiles)
            {
                Gizmos.color = GetTileColor(tile.LayerMask);
                Gizmos.DrawCube(tile.PivotPositon, Vector3.one);
            }
        }
        
        public void AddTile(Vector2Int pos)
        {
            if (_tiles.Find((tile) => { return tile.Position.Equals(pos); }) != null)
            {
                return;
            }

            LevelTestingData.TileData tileData = new LevelTestingData.TileData();

            if (_option.DrawLayer == DrawLayer.PlayerSpawnPoint)
            {
                foreach (var tile in _tiles)
                {
                    if (tile.LayerMask == (int) DrawLayer.PlayerSpawnPoint)
                    {
                        _tiles.Remove(tile);
                        break;
                    }
                }
            }

            tileData.Position = pos;
            tileData.LayerMask = (int)_option.DrawLayer;
            _tiles.Add(tileData);
        }
        
        public void TileRange(Vector2Int leftTop, Vector2Int rightBottom,bool isAddRemove)
        {
            Vector2Int min = new Vector2Int(Mathf.Min(leftTop.x, rightBottom.x), Mathf.Min(leftTop.y, rightBottom.y));
            Vector2Int max = new Vector2Int(Mathf.Max(leftTop.x, rightBottom.x), Mathf.Max(leftTop.y, rightBottom.y));

            for(int x = min.x; x <= max.x; x++)
            {
                for(int y = min.y; y <= max.y; y++)
                {
                    if (isAddRemove)
                    {
                        AddTile(new Vector2Int(x, y));
                    }
                    else
                    {
                        RemoveTile(new Vector2Int(x, y));
                    }
                }
            }
        }
        
        public void RemoveTile(Vector2Int pos)
        {
            var removeCell = _tiles.Find((tile) => { return tile.Position.Equals(pos); });

            if(removeCell == null)
            {
                return;
            }

            _tiles.Remove(removeCell);
        }

        public void Save()
        {
            if (_data == null)
                return;
            _data.Tiles.Clear();
            _data.Tiles.AddRange(_tiles);
        }

        public void Load()
        {
            if (_data == null)
                return;
            
            _tiles.Clear();
            _tiles.AddRange(_data.Tiles);
        }

        public void ResetTile()
        {
            _tiles.Clear();
        }

        public void GenerateSizeCheck(Vector2Int pos)
        {
            _generateSize = pos;
        }

        public Color GetTileColor(LayerMask layerMask)
        {
            if (layerMask == LayerMask.GetMask("Wall"))
            {
                return Option.ColorOption.Wall;
            }
            else if (layerMask== LayerMask.GetMask("CharacterCollider"))
            {
                return Option.ColorOption.CharacterCollider;
            }
            else if (layerMask == LayerMask.GetMask("ProjectileCollider"))
            {
                return Option.ColorOption.ProjectileCollider;
            }
            else if (layerMask == LayerMask.GetMask("HardCollider"))
            {
                return Option.ColorOption.HardCollider;
            }
            else if (layerMask == LayerMask.GetMask("Player"))
            {
                return Option.ColorOption.PlayerSpawnPoint;
            }

            return Option.ColorOption.Tile;
        }
    }
}