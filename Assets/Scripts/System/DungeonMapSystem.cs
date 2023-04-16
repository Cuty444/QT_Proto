using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    public class CellData
    {
        public RoomType RoomType;
        public bool IsClear;

        public CellData()
        {
            RoomType = RoomType.None;
            IsClear = false;
        }
    }

    public class MapData
    {
        public CellData[,] Map;
        public Vector2Int StartPosition;
        public Vector2Int BossRoomPosition;
        public List<Vector2Int> MapNodeList;

        public MapData(CellData[,] map, Vector2Int startPosition,Vector2Int bossRoomPosition,List<Vector2Int> mapNodeList)
        {
            Map = map;
            StartPosition = startPosition;
            BossRoomPosition = bossRoomPosition;
            MapNodeList = mapNodeList;
        }
    }
    public class DungeonMapSystem : SystemBase
    {
        [SerializeField] private int _mapWidth = 11;
        [SerializeField] private int _mapHeight = 7;
        [SerializeField] private int _maxRoomValue = 10;
        [Range(0.0f,1.0f)]
        [SerializeField] private float _manyPathCorrection = 1.0f;

        private CellData[,] _map;
        private List<Vector2Int> _mapNodeList = new List<Vector2Int>();

        private MapData _mapData;
        public MapData DungeonMapData => _mapData;

        private Transform _mapCellsTransform;
        public Transform MapCellsTransform => _mapCellsTransform;

        private Vector2 _mapSizePosition;

        private List<GameObject> _mapList;
        private int _mapCount;
        
        public override void OnInitialized()
        {
            Vector2Int startPos = new Vector2Int(_mapWidth / 2, _mapHeight / 2);
            _mapSizePosition = new Vector2(startPos.x * 40.0f, startPos.y * -40.0f);
            GenerateMap(startPos);
            _mapData = new MapData(_map, startPos,GetFarthestRoomFromStart(),_mapNodeList);
            MapLoad();
            SystemManager.Instance.PlayerManager.PlayerMapClearPosition.AddListener(position =>
            {
                _map[position.y, position.x].IsClear = true;
            });
            SystemManager.Instance.PlayerManager.PlayerCreateEvent.AddListener((player) =>
            {
                _mapCellsTransform = GameObject.FindWithTag("MapCells").transform;
            });
        }

        public Vector2 GetMiniMapSizeToMapSize()
        {
            return _mapSizePosition;
        }
        
        
        public void DungeonStart()
        {
            SystemManager.Instance.PlayerManager.OnPlayerCreate();
        }

        private void GenerateMap(Vector2Int startPos)
        {
            _map = new CellData[_mapHeight, _mapWidth];
            for (int y = 0; y < _mapHeight; y++)
            {
                for (int x = 0; x < _mapWidth; x++)
                {
                    _map[y, x] = new CellData();
                }
            }

            QT.Util.RandomSeed.SeedSetting();
            _map[startPos.y, startPos.x].RoomType = RoomType.Normal;
            _map[startPos.y, startPos.x].IsClear = true;
            _mapNodeList.Add(startPos);
            for (int i = 1; i < _maxRoomValue; i++)
            {
                RouteConfirm(_mapNodeList);
            }
        }

        private void RouteConfirm(List<Vector2Int> nodeList)
        {
            List<Vector2Int> routeList = new List<Vector2Int>();
            for (int i = 0; i < nodeList.Count; i++)
            {
                for (int j = 0; j < QT.Util.UnityUtil.PathDirections.Length; j++)
                {
                    Vector2Int nodeValue = nodeList[i] - QT.Util.UnityUtil.PathDirections[j];
                    if(nodeValue.x < 0 || nodeValue.x >= _mapWidth)
                        continue;
                    if (nodeValue.y < 0 || nodeValue.y >= _mapHeight)
                        continue;
                    if (routeList.Contains(nodeValue))
                        continue;
                    if (_mapNodeList.Contains(nodeValue))
                        continue;
                    if (MultiPathCheck(nodeValue))
                    {
                        if (UnityEngine.Random.Range(0f, 1f) < _manyPathCorrection)
                            continue;
                    }

                    routeList.Add(nodeValue);
                }
            }

            RoomCreate(routeList[UnityEngine.Random.Range(0, routeList.Count)]);
        }

        private bool MultiPathCheck(Vector2Int pos)
        {
            int pathCount = 0;
            for (int i = 0; i < QT.Util.UnityUtil.PathDirections.Length; i++)
            {
                Vector2Int nodeValue = pos - QT.Util.UnityUtil.PathDirections[i];
                if(nodeValue.x < 0 || nodeValue.x >= _mapWidth)
                    continue;
                if (nodeValue.y < 0 || nodeValue.y >= _mapHeight)
                    continue;
                if(_map[nodeValue.y,nodeValue.x].RoomType != RoomType.Normal)
                    continue;

                pathCount++;
            }

            if (pathCount >= 2)
                return true;
            return false;
        }
        
        public bool MultiPathClearCheck(Vector2Int pos)
        {
            for (int i = 0; i < QT.Util.UnityUtil.PathDirections.Length; i++)
            {
                Vector2Int nodeValue = pos - QT.Util.UnityUtil.PathDirections[i];
                if(nodeValue.x < 0 || nodeValue.x >= _mapWidth)
                    continue;
                if (nodeValue.y < 0 || nodeValue.y >= _mapHeight)
                    continue;
                if (_map[nodeValue.y, nodeValue.x].RoomType == RoomType.Normal)
                {
                    if (_map[nodeValue.y, nodeValue.x].IsClear)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
        
        private Vector2Int GetFarthestRoomFromStart()
        {
            // ���۹��� ��ǥ
            Vector2Int startRoomPos = _mapNodeList[0];

            // BFS �˰��� ����� ť�� �湮 ���θ� üũ�� �迭
            Queue<Vector2Int> queue = new Queue<Vector2Int>();
            bool[,] visited = new bool[_mapHeight, _mapWidth];

            // ���� ���� ť�� �߰��ϰ� �湮 üũ
            queue.Enqueue(startRoomPos);
            visited[startRoomPos.y, startRoomPos.x] = true;

            // BFS �˰����� ����Ͽ� ���� �����κ��� ���� �� ����� ��ǥ�� ã���ϴ�.
            List<Vector2Int> farthestRoomPosList = new List<Vector2Int>();
            int farthestDistance = 0;
            while (queue.Count > 0)
            {
                Vector2Int currRoomPos = queue.Dequeue();
                int distance = Mathf.Abs(currRoomPos.x - startRoomPos.x) + Mathf.Abs(currRoomPos.y - startRoomPos.y);

                if (distance > farthestDistance)
                {
                    farthestDistance = distance;
                    farthestRoomPosList.Clear();
                    farthestRoomPosList.Add(currRoomPos);
                }
                else if (distance == farthestDistance)
                {
                    farthestRoomPosList.Add(currRoomPos);
                }

                foreach (Vector2Int dir in QT.Util.UnityUtil.PathDirections)
                {
                    Vector2Int nextRoomPos = currRoomPos + dir;

                    // ���� ���� ���� ����� �ǳʶ�
                    if (nextRoomPos.x < 0 || nextRoomPos.x >= _mapWidth || nextRoomPos.y < 0 || nextRoomPos.y >= _mapHeight)
                    {
                        continue;
                    }

                    // ���� ���� �̹� �湮�� ���̸� �ǳʶ�
                    if (visited[nextRoomPos.y, nextRoomPos.x])
                    {
                        continue;
                    }

                    // ���� ���� �� ���̸� �ǳʶ�
                    if (_map[nextRoomPos.y, nextRoomPos.x].RoomType == RoomType.None)
                    {
                        continue;
                    }

                    // ���� ���� ť�� �߰��ϰ� �湮 üũ
                    queue.Enqueue(nextRoomPos);
                    visited[nextRoomPos.y, nextRoomPos.x] = true;
                }
            }

            // ���� �� ��� �� �������� �ϳ� �����Ͽ� ��ȯ�մϴ�.
            int randomIndex = UnityEngine.Random.Range(0, farthestRoomPosList.Count);
            return farthestRoomPosList[randomIndex];
        }


        
        
        private void RoomCreate(Vector2Int pos)
        {
            _mapNodeList.Add(pos);
            _map[pos.y, pos.x].RoomType = RoomType.Normal;
        }
        private async void MapLoad()
        {
            var stageLocationList = await SystemManager.Instance.ResourceManager.GetLocations("Stage1"); //TODO : ���� ���̺� �������� ����Ʈ�� ����
            var ObjectList = await SystemManager.Instance.ResourceManager.LoadAssets<GameObject>(stageLocationList);
            _mapList = QT.Util.RandomSeed.GetRandomIndexes(ObjectList.ToList(),_maxRoomValue);
        }

        public GameObject GetMapObject()
        {
            return _mapList[_mapCount++ % _mapList.Count];
        }
    }
}
