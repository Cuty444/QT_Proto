using System;
using System.Collections;
using System.Collections.Generic;
using QT.Core;
using QT.Map;
using QT.Core.Map;
using UnityEngine;

namespace QT.UI
{
    public class MinimapCanvas : UIPanel
    {
        [SerializeField] private Transform _miniMapCellTransform;
        [SerializeField] private GameObject _miniMapOnOff;
        [SerializeField] private GameObject _cellObject;

        private Dictionary<Vector2Int, MapDirection> _pathDirections = new Dictionary<Vector2Int, MapDirection>();
        private MapData _mapData;

        private List<GameObject> _cellList = new List<GameObject>();
        public override void Initialize()
        {
            _mapData = SystemManager.Instance.GetSystem<DungeonMapSystem>().DungeonMapData;
            _miniMapOnOff.SetActive(false);
            _pathDirections.Add(Vector2Int.up,MapDirection.Up);
            _pathDirections.Add(Vector2Int.down,MapDirection.Down);
            _pathDirections.Add(Vector2Int.left,MapDirection.Left);
            _pathDirections.Add(Vector2Int.right,MapDirection.Right);
            MiniMapDraw();
        }

        private void Update()
        {
            MiniMapInput();
        }

        private void MiniMapInput()
        {
            if (Input.GetKey(KeyCode.Tab))
            {
                _miniMapOnOff.SetActive(true);
            }
            else
            {
                _miniMapOnOff.SetActive(false);
            }
        }

        private void MiniMapDraw()
        {
            Vector3 pos = new Vector3(_mapData.StartPosition.x * 200f, _mapData.StartPosition.y * -200f, 0f);
            for (int y = 0; y < _mapData.Map.GetLength(0); y++)
            {
                for (int x = 0; x < _mapData.Map.GetLength(1); x++)
                {
                    if ((RoomType)_mapData.Map[y, x] == RoomType.Normal)
                    {
                        Vector2Int position = new Vector2Int(x, y);
                        MapDirection direction = DirectionCheck(position);
                        CellCreate(position,direction);
                    }
                }
            }

            _miniMapCellTransform.transform.localPosition = -pos;
        }

        private void CellCreate(Vector2Int createPos,MapDirection direction)
        {
            Vector3 pos = new Vector3(createPos.x * 200f, createPos.y * -200f, 0f);
            var cell = Instantiate(_cellObject,_miniMapCellTransform);
            cell.transform.localPosition = pos;
            cell.GetComponent<CellRouteLineDrawer>().SetRouteDirection(direction);
            _cellList.Add(cell);
        }

        private MapDirection DirectionCheck(Vector2Int position)
        {
            MapDirection mapDirection = MapDirection.None;
            foreach (KeyValuePair<Vector2Int, MapDirection> direction in _pathDirections)
            {
                Vector2Int nodeValue = position - direction.Key;
                if (nodeValue.x < 0 || nodeValue.x >= _mapData.Map.GetLength(1))
                    continue;
                if (nodeValue.y < 0 || nodeValue.y >= _mapData.Map.GetLength(0))
                    continue;
                if ((RoomType) _mapData.Map[nodeValue.y, nodeValue.x] != RoomType.Normal)
                {
                    continue;
                }
                mapDirection |= direction.Value;
            }

            return mapDirection;
        }
    }
}
