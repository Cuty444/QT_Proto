using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.TerrainUtils;

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
    }

    public enum ToolMode
    {
        Cell,
        Range,
    }

    public enum DrawLayer
    {
        Tile,
        Wall = 1 << 10,
        CharacterCollider = 1 << 14,
        ProjectileCollider = 1 << 18,
        HardCollider = 1 << 15,
    }

    [ExecuteInEditMode]
    public class LevelTool : MonoBehaviour
    {
        [SerializeField] private ToolOption _option;
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
            if (_tiles.Find((cell) => { return cell.Position.Equals(pos); }) != null)
            {
                return;
            }

            LevelTestingData.TileData tileData = new LevelTestingData.TileData();

            tileData.Position = pos;
            tileData.LayerMask = (int)_option.DrawLayer;
            _tiles.Add(tileData);
        }
        
        public void AddTileRange(Vector2Int leftTop, Vector2Int rightBottom)
        {
            Vector2Int min = new Vector2Int(Mathf.Min(leftTop.x, rightBottom.x), Mathf.Min(leftTop.y, rightBottom.y));
            Vector2Int max = new Vector2Int(Mathf.Max(leftTop.x, rightBottom.x), Mathf.Max(leftTop.y, rightBottom.y));

            for(int x = min.x; x <= max.x; x++)
            {
                for(int y = min.y; y <= max.y; y++)
                {
                    AddTile(new Vector2Int(x, y));
                }
            }
        }
        
        public void RemoveTile(Vector2Int pos)
        {
            var removeCell = _tiles.Find((cell) => { return cell.Position.Equals(pos); });

            if(removeCell == null)
            {
                return;
            }

            _tiles.Remove(removeCell);
        }

        public void RemoveTileRange(Vector2Int leftTop, Vector2Int rightBottom)
        {
            Vector2Int min = new Vector2Int(Mathf.Min(leftTop.x, rightBottom.x), Mathf.Min(leftTop.y, rightBottom.y));
            Vector2Int max = new Vector2Int(Mathf.Max(leftTop.x, rightBottom.x), Mathf.Max(leftTop.y, rightBottom.y));

            for(int x = min.x; x <= max.x; x++)
            {
                for(int y = min.y; y <= max.y; y++)
                {
                    RemoveTile(new Vector2Int(x, y));
                }
            }
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

            return Option.ColorOption.Tile;
        }
    }
}