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
        public Vector2Int Position;
        public int RoomCount;

        public BFSCellData(Vector2Int position, int roomCount)
        {
            Position = position;
            RoomCount = roomCount;
        }
    }

    public class CellData
    {
        public RoomType RoomType;
        public bool IsVisited;
        public bool IsClear;

        public CellData()
        {
            RoomType = RoomType.None;
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
        [SerializeField] private int _mapWidth = 11;
        [SerializeField] private int _mapHeight = 7;
        [SerializeField] private int _maxRoomValue = 10;
        [SerializeField] private int _floorValue = 0;
        [Range(0.0f, 1.0f)] [SerializeField] private float _manyPathCorrection = 1.0f;

        private CellData[,] _map;
        private List<Vector2Int> _mapNodeList = new List<Vector2Int>();

        private MapData _mapData;
        public MapData DungeonMapData => _mapData;

        private Transform _dungeonManagerTransform;
        public Transform DungeonManagerTransform => _dungeonManagerTransform;

        private Vector2 _mapSizePosition;

        private List<GameObject> _mapList;
        private List<GameObject> _startList;
        private List<GameObject> _shopMapList;
        private List<GameObject> _bossMapList;
        private List<GameObject> _stairsMapList;
        private List<GameObject> _rewardMapList;
        private List<GameObject> _hpHealMapList;
        private int _mapCount;
        private Dictionary<Vector2Int, MapDirection> _pathDirections = new Dictionary<Vector2Int, MapDirection>();

        private MinimapCanvas _minimapCanvas;

        private const string _stagePath = "Stage";

        private GlobalData _globalData;

        public override void OnInitialized()
        {
            _globalData = SystemManager.Instance.GetSystem<GlobalDataSystem>().GlobalData;
            _pathDirections.Add(Vector2Int.up, MapDirection.Up);
            _pathDirections.Add(Vector2Int.down, MapDirection.Down);
            _pathDirections.Add(Vector2Int.right, MapDirection.Left);
            _pathDirections.Add(Vector2Int.left, MapDirection.Right);
            _maxRoomValue--; // TODO : 보스방 생성에 의해 1개 줄임
            SystemManager.Instance.PlayerManager.StairNextRoomEvent.AddListener(NextFloor);

            SystemManager.Instance.PlayerManager.PlayerMapClearPosition.AddListener(position =>
            {
                _map[position.y, position.x].IsClear = true;
            });
            SystemManager.Instance.PlayerManager.PlayerMapVisitedPosition.AddListener(position =>
            {
                _map[position.y, position.x].IsVisited = true;
            });
        }

        public override void OnPostInitialized()
        {
            _minimapCanvas = SystemManager.Instance.UIManager.GetUIPanel<MinimapCanvas>();
        }

        #region MapGenerate

        public void DungenMapGenerate()
        {
            var data = SystemManager.Instance.DataManager.GetDataBase<ProductialMapGameDataBase>()
                .GetData(800 + _floorValue);

            if (data != null)
            {
                _maxRoomValue = data.MaxRoomValue - 1;
            }
            else
            {
                _maxRoomValue = 9;
            }

            Vector2Int startPos = new Vector2Int(_mapWidth / 2, _mapHeight / 2);
            _mapSizePosition = new Vector2(startPos.x * 40.0f, startPos.y * -40.0f);
            if (_mapList != null)
                _mapCount = Random.Range(0, _mapList.Count);
            GenerateMap(startPos);
            var roomPositionValues = GetFarthestRoomFromStart();
            SpecialRoomGenerate();
            _mapData = new MapData(_map, startPos, roomPositionValues.Item1, roomPositionValues.Item2, _mapNodeList);
        }


        public Vector2 GetMiniMapSizeToMapSize()
        {
            return _mapSizePosition;
        }

        public void DungeonReady(Transform dungeonManagerTransform)
        {
            _dungeonManagerTransform = dungeonManagerTransform;
            SystemManager.Instance.PlayerManager.CreatePlayer();
        }

        public void DungeonStart()
        {
            MapCellDraw();
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
            _map[startPos.y, startPos.x].RoomType = RoomType.Start;
            _map[startPos.y, startPos.x].IsVisited = true;
            _mapNodeList.Clear();
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

            RoomCreate(routeList[UnityEngine.Random.Range(0, routeList.Count)]);
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
            _map[bossRoom.y, bossRoom.x].RoomType =
                _floorValue >= 2 ? RoomType.Boss : RoomType.Stairs; // TODO : 계단 층 타입이랑 추후 체크 필요
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

            if (roomPosition == _mapData.BossRoomPosition)
            {
                return RoomType.Boss;
            }

            return _map[roomPosition.y, roomPosition.x].RoomType;
        }

        private void RoomCreate(Vector2Int pos)
        {
            _mapNodeList.Add(pos);
            _map[pos.y, pos.x].RoomType = RoomType.Normal;
        }

        #endregion

        #region MapDataLoad

        public async UniTask MapLoad(string stageNum)
        {
            var stageLocationList =
                await SystemManager.Instance.ResourceManager
                    .GetLocations(_stagePath + stageNum); //TODO : 추후 레이블 스테이지 리스트로 관리
            var objectList = await SystemManager.Instance.ResourceManager.LoadAssets<GameObject>(stageLocationList);
            _mapList = QT.Util.RandomSeed.GetRandomIndexes(objectList.ToList(), _maxRoomValue);
        }

        public async UniTask ShopLoad(string stageNum)
        {
            var stageShopLocationList =
                await SystemManager.Instance.ResourceManager.GetLocations(_stagePath + stageNum +
                                                                          "Shop"); //TODO : 추후 레이블 스테이지 리스트로 관리
            var shopObjectList =
                await SystemManager.Instance.ResourceManager.LoadAssets<GameObject>(stageShopLocationList);
            _shopMapList = shopObjectList.ToList();
        }

        public async UniTask StartRoomLoad(string stageNum)
        {
            var stageStartLocationList =
                await SystemManager.Instance.ResourceManager.GetLocations(_stagePath + stageNum +
                                                                          "Start"); //TODO : 추후 레이블 스테이지 리스트로 관리
            var startObjectList =
                await SystemManager.Instance.ResourceManager.LoadAssets<GameObject>(stageStartLocationList);
            _startList = startObjectList.ToList();
        }

        public async UniTask BossRoomLoad(string stageNum)
        {
            var stageBossLocationList =
                await SystemManager.Instance.ResourceManager.GetLocations(_stagePath + stageNum +
                                                                          "Boss"); //TODO : 추후 레이블 스테이지 리스트로 관리
            var bossObjectList =
                await SystemManager.Instance.ResourceManager.LoadAssets<GameObject>(stageBossLocationList);
            _bossMapList = bossObjectList.ToList();
        }

        public async UniTask StairsRoomLoad(string stageNum)
        {
            var stageStairsLocationList =
                await SystemManager.Instance.ResourceManager.GetLocations(_stagePath + stageNum +
                                                                          "Stairs"); //TODO : 추후 레이블 스테이지 리스트로 관리
            var stairsObjectList =
                await SystemManager.Instance.ResourceManager.LoadAssets<GameObject>(stageStairsLocationList);
            _stairsMapList = stairsObjectList.ToList();
        }
        
        public async UniTask RewardRoomLoad(string stageNum)
        {
            var stageRewardLocationList =
                await SystemManager.Instance.ResourceManager.GetLocations(_stagePath + stageNum +
                                                                          "Reward"); //TODO : 추후 레이블 스테이지 리스트로 관리
            var rewardObjectList =
                await SystemManager.Instance.ResourceManager.LoadAssets<GameObject>(stageRewardLocationList);
            _rewardMapList = rewardObjectList.ToList();
        }
        
        public async UniTask HpHealRoomLoad(string stageNum)
        {
            var stageHpHealLocationList =
                await SystemManager.Instance.ResourceManager.GetLocations(_stagePath + stageNum +
                                                                          "Hp"); //TODO : 추후 레이블 스테이지 리스트로 관리
            var hpHealObjectList =
                await SystemManager.Instance.ResourceManager.LoadAssets<GameObject>(stageHpHealLocationList);
            _hpHealMapList = hpHealObjectList.ToList();
        }

        #endregion

        #region GetMapData

        public GameObject GetMapObject()
        {
            return _mapList[_mapCount++ % _mapList.Count];
        }

        public GameObject ShopMapObject()
        {
            return _shopMapList[Random.Range(0, _shopMapList.Count)];
        }

        public GameObject StartMapObject()
        {
            return _startList[Random.Range(0, _startList.Count)];
        }

        public GameObject BossMapObject()
        {
            return _bossMapList[Random.Range(0, _bossMapList.Count)];
        }

        public GameObject StairsMapObject()
        {
            return _stairsMapList[Random.Range(0, _stairsMapList.Count)];
        }

        public GameObject RewardMapObject()
        {
            return _rewardMapList[Random.Range(0, _rewardMapList.Count)];
        }
        
        public GameObject HpMapObject()
        {
            return _hpHealMapList[Random.Range(0, _hpHealMapList.Count)];
        }
        
        public CellData GetCellData(Vector2Int pos)
        {
            return _map[pos.y, pos.x];
        }

        #endregion

        #region CellCreate

        private void MapCellDraw()
        {
            for (int i = 0; i < _mapData.MapNodeList.Count; i++)
            {
                MapDirection direction = DirectionCheck(_mapData.MapNodeList[i]);
                CellCreate(_mapData.MapNodeList[i], direction);
                Vector2Int pos = _mapData.MapNodeList[i];
                _minimapCanvas.CellCreate(_mapData.MapNodeList[i], direction, i, _mapData.Map[pos.y, pos.x].RoomType);
            }

            _minimapCanvas.MiniMapCellCenterPositionChange(_mapData.StartPosition);
        }

        private void CellCreate(Vector2Int createPos, MapDirection direction)
        {
            RoomType roomType = _mapData.Map[createPos.y, createPos.x].RoomType;
            GameObject cellMapObject = null;
            switch (roomType)
            {
                case RoomType.None:
                    return;
                case RoomType.Normal:
                    cellMapObject = GetMapObject();
                    break;
                case RoomType.Boss:
                    cellMapObject = BossMapObject();
                    break;
                case RoomType.GoldShop:
                case RoomType.HpShop:
                    cellMapObject = ShopMapObject();
                    break;
                case RoomType.Start:
                    cellMapObject = StartMapObject();
                    break;
                case RoomType.Stairs:
                    cellMapObject = StairsMapObject();
                    break;
                case RoomType.Reward:
                    cellMapObject = RewardMapObject();
                    break;
                case RoomType.HpHeal:
                    cellMapObject = HpMapObject();
                    break;
            }

            var mapCellData = Instantiate(cellMapObject, DungeonManagerTransform).GetComponent<MapCellData>();
            mapCellData.transform.position = new Vector3((createPos.x * 40.0f) - GetMiniMapSizeToMapSize().x,
                (createPos.y * -40.0f) - GetMiniMapSizeToMapSize().y, 0f);
            mapCellData.CellDataSet(direction, createPos, roomType);
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

        public void NextFloor()
        {
            if (_floorValue == 2)
                return;
            _floorValue++;
            var _playerManager = SystemManager.Instance.PlayerManager;
            SystemManager.Instance.PlayerManager.AddItemEvent.RemoveAllListeners();

            _playerManager.PlayerIndexInventory = _playerManager.Player.Inventory.GetItemList()
                .Select((x) => x.ItemGameData.Index).ToList();
            if (_playerManager.Player.Inventory.ActiveItem != null)
            {
                _playerManager.PlayerActiveItemIndex = _playerManager.Player.Inventory.ActiveItem.ItemGameData.Index;
            }

            var uiManager = SystemManager.Instance.UIManager;
            uiManager.GetUIPanel<FadeCanvas>().FadeOut(() =>
            {
                uiManager.GetUIPanel<MinimapCanvas>().OnClose();
                uiManager.GetUIPanel<FadeCanvas>().FadeIn();
                uiManager.GetUIPanel<LoadingCanvas>().OnOpen();
                SystemManager.Instance.UIManager.GetUIPanel<MinimapCanvas>().CellClear();
                ProjectileManager.Instance.Clear();
                HitAbleManager.Instance.Clear();
                //SystemManager.Instance.ResourceManager.AllReleasedObject();

                StartCoroutine(UnityUtil.WaitForFunc(() =>
                {
                    SystemManager.Instance.LoadingManager.FloorLoadScene(1);
                    DungenMapGenerate();
                    //SystemManager.Instance.UIManager.GetUIPanel<MinimapCanvas>().MinimapSetting(); TODO : 이 부분 로딩 정리하기
                }, 5f));
                SystemManager.Instance.StageLoadManager.StageLoad((GetFloor() + 1).ToString());
            });
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
