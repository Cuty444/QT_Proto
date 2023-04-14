using System;
using System.Collections;
using System.Collections.Generic;
using QT.Core;
using QT.Core.Data;
using UnityEngine;

namespace QT.Core.Map
{
    public enum RoomType
    {
        None,
        Normal,
        Boss,
        GoldShop,
        HpShop,
    }

    public class MapData
    {
        public int[,] Map;
        public Vector2Int StartPosition;

        public MapData(int[,] map, Vector2Int startPosition)
        {
            Map = map;
            StartPosition = startPosition;
        }
    }
    public class DungeonMapSystem : SystemBase
    {
        [SerializeField] private int _mapWidth = 11;
        [SerializeField] private int _mapHeight = 7;
        [SerializeField] private int _maxRoomVale = 10;
        [Range(0.0f,1.0f)]
        [SerializeField] private float _manyPathCorrection = 1.0f;

        private int[,] _map;
        private Vector2Int[] _pathDirections = new Vector2Int[4] {Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right};
        private List<Vector2Int> _mapNodeList = new List<Vector2Int>();

        private MapData _mapData;
        public MapData DungeonMapData => _mapData;
        
        public override void OnInitialized()
        {
            Vector2Int startPos = new Vector2Int(_mapWidth / 2, _mapHeight / 2);
            GenerateMap(startPos);
            _mapData = new MapData(_map, startPos);
        }

        private void GenerateMap(Vector2Int startPos)
        {
            _map = new int[_mapHeight,_mapWidth];
            QT.Util.RandomSeed.SeedSetting();
            _map[startPos.y, startPos.x] = (int)RoomType.Normal;
            _mapNodeList.Add(startPos);
            for (int i = 0; i <= _maxRoomVale; i++)
            {
                RouteConfirm(_mapNodeList);
            }
        }

        private void RouteConfirm(List<Vector2Int> nodeList)
        {
            List<Vector2Int> routeList = new List<Vector2Int>();
            for (int i = 0; i < nodeList.Count; i++)
            {
                for (int j = 0; j < _pathDirections.Length; j++)
                {
                    Vector2Int nodeValue = nodeList[i] - _pathDirections[j];
                    if(nodeValue.x < 0 || nodeValue.x >= _mapWidth)
                        continue;
                    if (nodeValue.y < 0 || nodeValue.y >= _mapHeight)
                        continue;
                    if (routeList.Contains(nodeValue))
                        continue;
                    routeList.Add(nodeValue);
                }
            }

            RoomCreate(routeList[UnityEngine.Random.Range(0, routeList.Count)]);
        }
        
        private void RoomCreate(Vector2Int pos)
        {
            _mapNodeList.Add(pos);
            _map[pos.y, pos.x] = (int)RoomType.Normal;
        }
    }
}
