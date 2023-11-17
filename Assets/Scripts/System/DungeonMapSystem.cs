using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using QT.Core;
using QT.Core.Data;
using QT.Map;
using QT.UI;
using QT.Util;
using UnityEngine;
using Random = UnityEngine.Random;
using UnityEngine.Events;

namespace QT.Core.Map
{
    public enum RoomType
    {
        None,
        Normal,
        Boss,
        GoldShop,
        HpShop,
        Start,
        Stairs,
        Reward,
        HpHeal,
        Wait,
        Tutorial,
        
        Length,
    }

    [Flags]
    public enum MapDirection
    {
        None = 0,
        Up = 1,
        Down = 2,
        Left = 4,
        Right = 8,

        All = Up | Down | Left | Right
    }

    public class BFSCellData
    {
        public readonly int RoomCount;
        public Vector2Int Position;

        public BFSCellData(Vector2Int position, int roomCount)
        {
            Position = position;
            RoomCount = roomCount;
        }
    }

    public class CellData
    {
        public MapDirection DoorDirection;
        public RoomType RoomType;
        public bool IsVisited;
        public bool IsClear;

        public CellData(RoomType type = RoomType.None)
        {
            RoomType = type;
            IsVisited = false;
            IsClear = false;
        }
    }

    public class MapData
    {
        public CellData[,] Map;
        public Vector2Int StartPosition;
        public Vector2Int BossRoomPosition;
        public Vector2Int ShopRoomPosition;
        public List<Vector2Int> MapNodeList;

        public MapData(CellData[,] map, Vector2Int startPosition, Vector2Int bossRoomPosition,
            Vector2Int shopRoomPosition, List<Vector2Int> mapNodeList)
        {
            Map = map;
            StartPosition = startPosition;
            BossRoomPosition = bossRoomPosition;
            ShopRoomPosition = shopRoomPosition;
            MapNodeList = mapNodeList;
        }
    }

    public class DungeonMapSystem : SystemBase
    {
        [SerializeField] private Vector2Int _mapSize = new Vector2Int(7, 7);
        
        private int _mapWidth = 11;
        private int _mapHeight = 7;
        
        [SerializeField] private int _floorValue = 0;
        [Range(0.0f, 1.0f)] [SerializeField] private float _manyPathCorrection = 1.0f;

        private CellData[,] _map;
        private List<Vector2Int> _mapNodeList = new List<Vector2Int>();

        private MapData _mapData;
        public MapData DungeonMapData => _mapData;

        private Transform _dungeonTransform;
        public Transform DungeonTransform => _dungeonTransform;

        public UnityEvent DungeonMapGeneratedEvent { get; } = new();
        
        private Vector2 _mapSizePosition;

        private int _maxRoomCount = 10;
        
        //private List<GameObject> _mapList;
        //private List<GameObject> _startList;
        //private List<GameObject> _shopMapList;
        //private List<GameObject> _bossMapList;
        //private List<GameObject> _stairsMapList;
        //private List<GameObject> _rewardMapList;
        //private List<GameObject> _hpHealMapList;
        private Dictionary<RoomType, List<GameObject>> _mapList = new Dictionary<RoomType, List<GameObject>>();
        private int _mapCount;
        private Dictionary<Vector2Int, MapDirection> _pathDirections = new ();

        private const string _stagePath = "Stage";

        private GlobalData _globalData;

        [HideInInspector] public Transform _stairRoomEnterTransform;
        [HideInInspector] public bool IsBossWaitEnter = false; // TODO : 보스 대기방인지 보스방 입장했는 시점 체크 여부

        public override void OnInitialized()
        {
            _globalData = SystemManager.Instance.GetSystem<GlobalDataSystem>().GlobalData;
            _pathDirections.Add(Vector2Int.up, MapDirection.Up);
            _pathDirections.Add(Vector2Int.down, MapDirection.Down);
            _pathDirections.Add(Vector2Int.right, MapDirection.Left);
            _pathDirections.Add(Vector2Int.left, MapDirection.Right);
            _maxRoomCount--; // TODO : 보스방 생성에 의해 1개 줄임

            for (int i = 0; i < (int) RoomType.Length; i++)
            {
                _mapList.Add((RoomType) i, null);
            }

            SystemManager.Instance.PlayerManager.PlayerMapClearPosition.AddListener(position =>
            {
                _map[position.y, position.x].IsClear = true;
            });
            SystemManager.Instance.PlayerManager.PlayerMapVisitedPosition.AddListener(position =>
            {
                _map[position.y, position.x].IsVisited = true;
            });
        }

        #region MapGenerate

        public void DungenMapGenerate()
        {
            IsBossWaitEnter = false;
            _mapWidth = _mapSize.x;
            _mapHeight = _mapSize.y;
            
            var data = SystemManager.Instance.DataManager.GetDataBase<ProductialMapGameDataBase>()
                .GetData(800 + _floorValue);

            if (data != null)
            {
                _maxRoomCount = data.MaxRoomValue - 1;
            }
            else
            {
                _maxRoomCount = 9;
            }

            Vector2Int startPos = new Vector2Int(_mapWidth / 2, _mapHeight / 2);
            _mapSizePosition = new Vector2(startPos.x * 100.0f, startPos.y * -100.0f);
            if (_mapList != null)
                _mapCount = Random.Range(0, _mapList.Count);
            GenerateMap(startPos);
            var roomPositionValues = GetFarthestRoomFromStart();
            SpecialRoomGenerate();
            _mapData = new MapData(_map, startPos, roomPositionValues.Item1, roomPositionValues.Item2, _mapNodeList);
            
            foreach (var pos in _mapData.MapNodeList)
            {
                _mapData.Map[pos.y,pos.x].DoorDirection = DirectionCheck(pos);
            }
            
            DungeonMapGeneratedEvent.Invoke();
        }

        public void TutorialMapGenerate()
        {
            _mapWidth = 3;
            _mapHeight = 3;
            
            _map = new CellData[3,3]
            {
                {new (RoomType.Start), new (RoomType.Tutorial), new (RoomType.Tutorial)},
                {new (),                               new (),                                new (RoomType.Tutorial)},
                {new (),                               new (RoomType.Tutorial), new (RoomType.Tutorial) }
            };

            _mapNodeList = new List<Vector2Int>
            {
                new(0, 0), new(1, 0), new(2, 0),
                new(2, 1),
                new(2, 2), new(1, 2)
            };
            
            _mapData = new MapData(_map, Vector2Int.zero, Vector2Int.zero, Vector2Int.zero, _mapNodeList);
            
            foreach (var pos in _mapData.MapNodeList)
            {
                _mapData.Map[pos.y,pos.x].DoorDirection = DirectionCheck(pos);
            }
            
            DungeonMapGeneratedEvent.Invoke();
        }


        public Vector2 GetMiniMapSizeToMapSize()
        {
            return _mapSizePosition;
        }

        public void DungeonReady(Transform dungeonManagerTransform)
        {
            _dungeonTransform = dungeonManagerTransform;
            SystemManager.Instance.PlayerManager.CreatePlayer();
        }

        public void DungeonStart(ref Dictionary<Vector2Int, MapCellData> dictionary)
        {
            foreach (var pos in _mapData.MapNodeList)
            {
                dictionary.Add(pos,CellCreate(pos, _mapData.Map[pos.y, pos.x].DoorDirection));
            }
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

            RandomSeed.SeedSetting();
            _map[startPos.y, startPos.x].RoomType = RoomType.Start;
            _map[startPos.y, startPos.x].IsVisited = true;
            _mapNodeList.Clear();
            _mapNodeList.Add(startPos);
            for (int i = 1; i < _maxRoomCount; i++)
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
                    if (nodeValue.x < 0 || nodeValue.x >= _mapWidth)
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

            var result = routeList[Random.Range(0, routeList.Count)];
            
            _mapNodeList.Add(result);
            _map[result.y, result.x].RoomType = RoomType.Normal;
        }

        private bool MultiPathCheck(Vector2Int pos)
        {
            int pathCount = 0;
            for (int i = 0; i < QT.Util.UnityUtil.PathDirections.Length; i++)
            {
                Vector2Int nodeValue = pos - QT.Util.UnityUtil.PathDirections[i];
                if (nodeValue.x < 0 || nodeValue.x >= _mapWidth)
                    continue;
                if (nodeValue.y < 0 || nodeValue.y >= _mapHeight)
                    continue;
                if (_map[nodeValue.y, nodeValue.x].RoomType != RoomType.None)
                {
                    pathCount++;
                }
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
                if (nodeValue.x < 0 || nodeValue.x >= _mapWidth)
                    continue;
                if (nodeValue.y < 0 || nodeValue.y >= _mapHeight)
                    continue;
                if (_map[nodeValue.y, nodeValue.x].RoomType != RoomType.None)
                {
                    if (_map[nodeValue.y, nodeValue.x].IsVisited)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private (Vector2Int, Vector2Int) GetFarthestRoomFromStart()
        {
            // 시작방의 좌표
            Vector2Int startRoomPos = _mapNodeList[0];
            List<BFSCellData> farthestRoomPosList = new List<BFSCellData>();
            Dictionary<Vector2Int, Vector2Int> bossRoomList = new Dictionary<Vector2Int, Vector2Int>();
            int countCheck = 0;
            do
            {
                do
                {
                    Debug.Log("맵 생성 : " + countCheck++ + "회 생성");
                    if (countCheck > 0)
                    {
                        GenerateMap(new Vector2Int(_mapWidth / 2, _mapHeight / 2));
                    }

                    // BFS 알고리즘에 사용할 큐와 방문 여부를 체크할 배열
                    Queue<BFSCellData> queue = new Queue<BFSCellData>();
                    bool[,] visited = new bool[_mapHeight, _mapWidth];

                    // 시작 방을 큐에 추가하고 방문 체크
                    queue.Enqueue(new BFSCellData(startRoomPos, 0));
                    visited[startRoomPos.y, startRoomPos.x] = true;

                    farthestRoomPosList.Clear();
                    int farthestDistance = 0;
                    while (queue.Count > 0)
                    {
                        BFSCellData currRoomPos = queue.Dequeue();

                        if (currRoomPos.RoomCount > farthestDistance)
                        {
                            farthestDistance = currRoomPos.RoomCount;
                            farthestRoomPosList.Clear();
                            farthestRoomPosList.Add(currRoomPos);
                        }
                        else if (currRoomPos.RoomCount == farthestDistance)
                        {
                            farthestRoomPosList.Add(currRoomPos);
                        }

                        foreach (Vector2Int dir in QT.Util.UnityUtil.PathDirections)
                        {
                            Vector2Int nextRoomPos = currRoomPos.Position - dir;

                            // 다음 방이 맵을 벗어나면 건너뜀
                            if (nextRoomPos.x < 0 || nextRoomPos.x >= _mapWidth || nextRoomPos.y < 0 ||
                                nextRoomPos.y >= _mapHeight)
                            {
                                continue;
                            }

                            // 다음 방이 이미 방문한 방이면 건너뜀
                            if (visited[nextRoomPos.y, nextRoomPos.x])
                            {
                                continue;
                            }

                            // 다음 방이 빈 방이면 건너뜀
                            if (_map[nextRoomPos.y, nextRoomPos.x].RoomType == RoomType.None)
                            {
                                continue;
                            }

                            // 다음 방을 큐에 추가하고 방문 체크
                            queue.Enqueue(new BFSCellData(nextRoomPos, currRoomPos.RoomCount + 1));
                            visited[nextRoomPos.y, nextRoomPos.x] = true;
                        }
                    }

                    List<BFSCellData> removeData = new List<BFSCellData>(); // 경로 2개 이상인 방은 제외 처리
                    foreach (var roomData in farthestRoomPosList)
                    {
                        if (RoomPathCount(roomData.Position) != 1)
                        {
                            removeData.Add(roomData);
                        }
                    }

                    foreach (var roomData in removeData)
                    {
                        farthestRoomPosList.Remove(roomData);
                    }
                } while (farthestRoomPosList.Count < 2);

                // 가장 먼 방들 중 랜덤으로 하나 선택하여 반환합니다.
                bossRoomList = BossRoomUpperCreate(farthestRoomPosList);
            } while (bossRoomList.Count < 1);

            int randomIndex = 0;
            Vector2Int bossRoom = new Vector2Int(9999, 9999);
            do
            {
                randomIndex = UnityEngine.Random.Range(0, farthestRoomPosList.Count);
                if (bossRoomList.ContainsKey(farthestRoomPosList[randomIndex].Position))
                {
                    bossRoom = bossRoomList[farthestRoomPosList[randomIndex].Position];
                }
            } while (bossRoom == new Vector2Int(9999, 9999));

            _mapNodeList.Add(bossRoom);
            //_map[bossRoom.y, bossRoom.x].RoomType =
            //    _floorValue >= 2 ? RoomType.Boss : RoomType.Stairs; // TODO : 계단 층 타입이랑 추후 체크 필요
            _map[bossRoom.y, bossRoom.x].RoomType = RoomType.Boss;
            farthestRoomPosList.RemoveAt(randomIndex);
            randomIndex = UnityEngine.Random.Range(0, farthestRoomPosList.Count);
            var shopRoom = farthestRoomPosList[randomIndex].Position;
            _map[shopRoom.y, shopRoom.x].RoomType = RoomType.GoldShop;
            return (bossRoom, shopRoom);
        }

        private void SpecialRoomGenerate()
        {
            List<Vector2Int> routeList = new List<Vector2Int>();
            for (int y = 0; y < _mapHeight; y++)
            {
                for (int x = 0; x < _mapWidth; x++)
                {
                    if (_map[y, x].RoomType == RoomType.None)
                    {
                        Vector2Int position = new Vector2Int(x, y);
                        if (SpecialRoomMultiPathCheck(position))
                        {
                            routeList.Add(position);
                        }
                    }
                }
            }
            
            for(int i = 0; i < routeList.Count;i++)
            {
                if (SpecialRoomRoutDuplicationClear(routeList[i],routeList))
                {
                    i = 0;
                }
            }

            int rewardRoomCount = _globalData.RewardRoomMaxCount[_floorValue];
            int hpRoomCount = _globalData.HpRoomMaxCount[_floorValue];
            while (true)
            {
                if (routeList.Count == 0)
                {
                    Debug.LogFormat("경로가 부족합니다.\n{0}개 만큼 생성 되지 못한 보상방\n{1}개 만큼 생성 되지 못한 회복방",rewardRoomCount,hpRoomCount);
                    break;
                }
                if (rewardRoomCount > 0)
                {
                    int index = Random.Range(0, routeList.Count);
                    Vector2Int position = routeList[index];
                    rewardRoomCount--;
                    routeList.RemoveAt(index);
                    _map[position.y, position.x].RoomType = RoomType.Reward;
                    _mapNodeList.Add(position);
                }
                if (routeList.Count == 0)
                    break;
                if (hpRoomCount > 0)
                {
                    int index = Random.Range(0, routeList.Count);
                    Vector2Int position = routeList[index];
                    hpRoomCount--;
                    routeList.RemoveAt(index);
                    _map[position.y, position.x].RoomType = RoomType.HpHeal;
                    _mapNodeList.Add(position);
                }

                if (rewardRoomCount == 0 && hpRoomCount == 0)
                    break;
            }
        }

        private bool SpecialRoomMultiPathCheck(Vector2Int pos)
        {
            int pathCount = 0;
            for (int i = 0; i < QT.Util.UnityUtil.PathDirections.Length; i++)
            {
                Vector2Int nodeValue = pos - QT.Util.UnityUtil.PathDirections[i];
                if (nodeValue.x < 0 || nodeValue.x >= _mapWidth)
                    continue;
                if (nodeValue.y < 0 || nodeValue.y >= _mapHeight)
                    continue;
                if (_map[nodeValue.y, nodeValue.x].RoomType != RoomType.None)
                {
                    if (_map[nodeValue.y, nodeValue.x].RoomType == RoomType.Boss ||
                        _map[nodeValue.y, nodeValue.x].RoomType == RoomType.GoldShop ||
                        _map[nodeValue.y, nodeValue.x].RoomType == RoomType.Stairs)
                    {
                        return false;
                    }

                    pathCount++;
                }
            }

            if (pathCount == 1)
                return true;
            return false;
        }

        private bool SpecialRoomRoutDuplicationClear(Vector2Int routePos, List<Vector2Int> routeList) // 특수방이 생성될수 있는 위치 양옆이 존재할경우 중복 제거
        {
            bool isRemover = false;
            for (int j = 0; j < QT.Util.UnityUtil.PathDirections.Length; j++)
            {
                Vector2Int nodeValue = routePos - QT.Util.UnityUtil.PathDirections[j];
                if (nodeValue.x < 0 || nodeValue.x >= _mapWidth)
                    continue;
                if (nodeValue.y < 0 || nodeValue.y >= _mapHeight)
                    continue;
                for (int i = 0; i < routeList.Count; i++)
                {
                    if (routeList[i] == nodeValue)
                    {
                        routeList.RemoveAt(i);
                        i--;
                        isRemover = true;
                    }
                }
            }

            return isRemover;
        }

        private int RoomPathCount(Vector2Int roomPosition)
        {
            int roomPathCount = 0;
            foreach (Vector2Int dir in QT.Util.UnityUtil.PathDirections)
            {
                Vector2Int nextRoomPos = roomPosition - dir;
                if (nextRoomPos.x < 0 || nextRoomPos.x >= _mapWidth || nextRoomPos.y < 0 ||
                    nextRoomPos.y >= _mapHeight)
                {
                    continue;
                }

                if (_map[nextRoomPos.y, nextRoomPos.x].RoomType != RoomType.None)
                {
                    roomPathCount++;
                }
            }

            return roomPathCount;
        }

        private Dictionary<Vector2Int, Vector2Int> BossRoomUpperCreate(List<BFSCellData> cellData)
        {
            Dictionary<Vector2Int, Vector2Int> bossRoomPositions = new Dictionary<Vector2Int, Vector2Int>();
            for (int i = 0; i < cellData.Count; i++)
            {
                Vector2Int nextRoomPos = cellData[i].Position - Vector2Int.up;
                if (nextRoomPos.y < 0)
                {
                    continue;
                }

                if (_map[nextRoomPos.y, nextRoomPos.x].RoomType == RoomType.None)
                {
                    bossRoomPositions.Add(cellData[i].Position, nextRoomPos);
                }
            }

            Dictionary<Vector2Int, Vector2Int> bossRoomPositionsConfirmed = new Dictionary<Vector2Int, Vector2Int>();
            foreach (var bossRoomPos in bossRoomPositions)
            {
                if (BossDirectionCheck(bossRoomPos))
                {
                    bossRoomPositionsConfirmed.Add(bossRoomPos.Key, bossRoomPos.Value);
                }
            }

            return bossRoomPositionsConfirmed;
        }

        private bool BossDirectionCheck(KeyValuePair<Vector2Int, Vector2Int> bossPosition)
        {
            bool bCheck = true;
            foreach (Vector2Int dir in QT.Util.UnityUtil.PathDirections)
            {
                if (dir == Vector2Int.down)
                    continue;
                Vector2Int nextRoomPos = bossPosition.Value - dir;
                if (nextRoomPos.y < 0)
                {
                    continue;
                }

                if (nextRoomPos.x < 0 || nextRoomPos.x >= _mapWidth)
                {
                    continue;
                }

                if (_map[nextRoomPos.y, nextRoomPos.x].RoomType != RoomType.None)
                {
                    bCheck = false;
                    break;
                }
            }

            return bCheck;
        }

        public RoomType RoomCheck(Vector2Int roomPosition)
        {
            if (roomPosition.x < 0 || roomPosition.x >= _mapWidth || roomPosition.y < 0 ||
                roomPosition.y >= _mapHeight)
            {
                return RoomType.None;
            }

            if (roomPosition == _mapData.ShopRoomPosition) // TODO : 추후 룸타입 갱신하기
            {
                return RoomType.GoldShop;
            }

            return _map[roomPosition.y, roomPosition.x].RoomType;
        }

        #endregion

        #region MapDataLoad

        public async UniTask MapLoad(string stageNum,RoomType roomType)
        {
            var stageLocationList =
                await SystemManager.Instance.ResourceManager
                    .GetLocations(_stagePath + stageNum + RoomTypeToPath(roomType));
            var objectList = await SystemManager.Instance.ResourceManager.LoadAssets<GameObject>(stageLocationList);
            
            switch (roomType)
            {
                case RoomType.None:
                case RoomType.Normal:
                    var list = objectList.ToList();
                    list.Shuffle();
                    _mapList[roomType] = list;
                    return;
            }
            _mapList[roomType] = objectList.ToList();
        }

        private string RoomTypeToPath(RoomType roomType)
        {
            switch (roomType)
            {
                case RoomType.None:
                case RoomType.Normal:
                case RoomType.Tutorial:
                    return string.Empty;
                case RoomType.Boss:
                    return "Boss";
                case RoomType.GoldShop:
                case RoomType.HpShop:
                    return "Shop";
                case RoomType.Start:
                    return "Start";
                case RoomType.Stairs:
                    return "Stairs";
                case RoomType.Reward:
                    return "Reward";
                case RoomType.HpHeal:
                    return "Hp";
                case RoomType.Wait:
                    return "Wait";
                default:
                    throw new ArgumentOutOfRangeException(nameof(roomType), roomType, null);
            }
        }

        #endregion

        #region GetMapData

        public GameObject GetMapObject(RoomType roomType,MapDirection mapDirection)
        {
            switch (roomType)
            {
                case RoomType.None:
                case RoomType.Normal:
                case RoomType.Tutorial:
                    return _mapList[roomType][_mapCount++ % _mapList[roomType].Count];
                case RoomType.Reward:
                    if (mapDirection == MapDirection.Left || mapDirection == MapDirection.Down)
                    {
                        return _mapList[roomType][0];
                    }
                    return _mapList[roomType][1];
            }
            return _mapList[roomType][Random.Range(0, _mapList[roomType].Count)];
        }
        
        public CellData GetCellData(Vector2Int pos)
        {
            return _map[pos.y, pos.x];
        }

        #endregion

        #region CellCreate

        private MapCellData CellCreate(Vector2Int createPos, MapDirection direction)
        {
            RoomType roomType = _mapData.Map[createPos.y, createPos.x].RoomType;
            if (roomType == RoomType.None)
            {
                return null;
            }
            
            var cellMapObject = GetMapObject(roomType,_mapData.Map[createPos.y,createPos.x].DoorDirection);

            var cellPosition = new Vector2((createPos.x * 100.0f) - GetMiniMapSizeToMapSize().x,
                (createPos.y * -100.0f) - GetMiniMapSizeToMapSize().y);
            
            var mapCellData = Instantiate(cellMapObject, DungeonTransform).GetComponent<MapCellData>();
            mapCellData.transform.position = cellPosition;
            mapCellData.CellDataSet(direction, createPos, roomType);
            
            if (roomType == RoomType.Boss)
            {
                if (_floorValue < 2)
                {
                    var floorCellData = Instantiate(GetMapObject(RoomType.Stairs,MapDirection.None), DungeonTransform)
                        .GetComponent<MapCellData>();
                    floorCellData.transform.position = new Vector3(cellPosition.x, cellPosition.y + 1000f,0f);
                    //mapCellData.CreateBossDoor(floorCellData.GetStageEnterDoorPosition(1));
                    //floorCellData.CreateStairsDoor(mapCellData.GetStageEnterDoorPosition(0));
                    _stairRoomEnterTransform = floorCellData.GetStageEnterDoorPosition(1);
                }

                var waitCellData = Instantiate(GetMapObject(RoomType.Wait, MapDirection.None), DungeonTransform).GetComponent<MapCellData>();
                waitCellData.transform.position = new Vector3(mapCellData.GetDownTransform().position.x, mapCellData.GetDownTransform().position.y - 19f, 0f);
                mapCellData.CreateBossDoor(waitCellData.GetStageEnterDoorPosition(1));
                waitCellData.CreateWaitDoor();
            }
            return mapCellData;
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
                if (_mapData.Map[nodeValue.y, nodeValue.x].RoomType == RoomType.None)
                {
                    continue;
                }

                mapDirection |= direction.Value;
            }

            return mapDirection;
        }

        #endregion


        #region Floor

        public void EnterStairMap()
        {
            _mapData.MapNodeList.Clear();
            _mapData.MapNodeList.Add(_mapData.BossRoomPosition);
        }
        public async void NextFloor()
        {
            if (_floorValue == 2)
                return;
            
            _floorValue++;
            
            //var _playerManager = SystemManager.Instance.PlayerManager;

            //_playerManager.PlayerIndexInventory = _playerManager.Player.Inventory.GetItemList()
            //    .Select((x) => x.ItemGameData.Index).ToList();
            //
            //if (_playerManager.Player.Inventory.ActiveItem != null)
            //{
            //    _playerManager.PlayerActiveItemIndex = _playerManager.Player.Inventory.ActiveItem.ItemGameData.Index;
            //}
            
            SystemManager.Instance.UIManager.SetState(UIState.Loading);
            
            ProjectileManager.Instance.Clear();
            HitAbleManager.Instance.Clear();

            await SystemManager.Instance.StageLoadManager.StageLoad(GetFloor() + 1);
            
            SystemManager.Instance.LoadingManager.LoadScene(SceneNumber.InGame);
            DungenMapGenerate();
        }

        public void SetFloor(int value)
        {
            _floorValue = value;
        }

        public int GetFloor()
        {
            return _floorValue;
        }

        #endregion
    }
}
