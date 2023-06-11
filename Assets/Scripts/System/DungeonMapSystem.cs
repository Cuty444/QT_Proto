using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using QT.Core;
using QT.Core.Data;
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

        public MapData(CellData[,] map, Vector2Int startPosition,Vector2Int bossRoomPosition,Vector2Int shopRoomPosition,List<Vector2Int> mapNodeList)
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
        private List<GameObject> _startList;
        private List<GameObject> _shopMapList;
        private List<GameObject> _bossMapList;
        private List<GameObject> _stairsMapList;
        private int _mapCount;
        
        public override void OnInitialized()
        {
            _maxRoomValue--; // TODO : 보스방 생성에 의해 1개 줄임
            SystemManager.Instance.PlayerManager.StairNextRoomEvent.AddListener(NextFloor);
            DungenMapGenerate();
            SystemManager.Instance.PlayerManager.PlayerMapClearPosition.AddListener(position =>
            {
                _map[position.y, position.x].IsClear = true;
            });
            SystemManager.Instance.PlayerManager.PlayerMapVisitedPosition.AddListener(position =>
            {
                _map[position.y, position.x].IsVisited = true;
            });
            SystemManager.Instance.PlayerManager.PlayerCreateEvent.AddListener((player) =>
            {
                _mapCellsTransform = GameObject.FindWithTag("MapCells").transform;
                StartCoroutine(UnityUtil.WaitForFunc(() =>
                {
                    SystemManager.Instance.PlayerManager.FloorAllHitalbeRegister.Invoke(_mapCellsTransform
                        .GetComponentsInChildren<IHitable>().ToList());
                }, 5f));
            });
        }

        public void DungenMapGenerate()
        {
            if (_floorValue != 0)
            {
                var data = SystemManager.Instance.DataManager.GetDataBase<ProductialMapGameDataBase>().GetData(800 + _floorValue);
                _maxRoomValue = data.MaxRoomValue - 1;
            }
            else
            {
                _maxRoomValue = 9;
            }
            Vector2Int startPos = new Vector2Int(_mapWidth / 2, _mapHeight / 2);
            _mapSizePosition = new Vector2(startPos.x * 40.0f, startPos.y * -40.0f);
            if(_mapList != null)
                _mapCount = Random.Range(0,_mapList.Count);
            GenerateMap(startPos);
            var roomPositionValues = GetFarthestRoomFromStart();
            _mapData = new MapData(_map, startPos,roomPositionValues.Item1,roomPositionValues.Item2,_mapNodeList);
        }
        

        public Vector2 GetMiniMapSizeToMapSize()
        {
            return _mapSizePosition;
        }
        
        
        public void DungeonStart()
        {
            SystemManager.Instance.PlayerManager.CreatePlayer();
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
                    if (_map[nodeValue.y, nodeValue.x].IsVisited)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
        
        private (Vector2Int,Vector2Int) GetFarthestRoomFromStart()
        {
            // 시작방의 좌표
            Vector2Int startRoomPos = _mapNodeList[0];
            List<BFSCellData> farthestRoomPosList = new List<BFSCellData>();
            Dictionary<Vector2Int,Vector2Int> bossRoomList = new Dictionary<Vector2Int,Vector2Int>();
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
            Vector2Int bossRoom = new Vector2Int(9999,9999);
            do
            {
                randomIndex = UnityEngine.Random.Range(0, farthestRoomPosList.Count);
                if (bossRoomList.ContainsKey(farthestRoomPosList[randomIndex].Position))
                {
                    bossRoom = bossRoomList[farthestRoomPosList[randomIndex].Position];
                }
            } while (bossRoom == new Vector2Int(9999,9999));
            _mapNodeList.Add(bossRoom);
            _map[bossRoom.y, bossRoom.x].RoomType = RoomType.Normal;
            farthestRoomPosList.RemoveAt(randomIndex);
            randomIndex = UnityEngine.Random.Range(0, farthestRoomPosList.Count);
            var shopRoom = farthestRoomPosList[randomIndex].Position;
            return (bossRoom,shopRoom);
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

        private Dictionary<Vector2Int,Vector2Int> BossRoomUpperCreate(List<BFSCellData> cellData)
        {
            Dictionary<Vector2Int,Vector2Int> bossRoomPositions = new Dictionary<Vector2Int,Vector2Int>();
            for (int i = 0; i < cellData.Count; i++)
            {
                Vector2Int nextRoomPos = cellData[i].Position - Vector2Int.up;
                if (nextRoomPos.y < 0)
                {
                    continue;
                }

                if (_map[nextRoomPos.y, nextRoomPos.x].RoomType == RoomType.None)
                {
                    bossRoomPositions.Add(cellData[i].Position,nextRoomPos);
                }
            }
            Dictionary<Vector2Int,Vector2Int> bossRoomPositionsConfirmed = new Dictionary<Vector2Int,Vector2Int>();
            foreach (var bossRoomPos in bossRoomPositions)
            {
                if (BossDirectionCheck(bossRoomPos))
                {
                    bossRoomPositionsConfirmed.Add(bossRoomPos.Key,bossRoomPos.Value);
                }
            }
            return bossRoomPositionsConfirmed;
        }

        private bool BossDirectionCheck(KeyValuePair<Vector2Int,Vector2Int> bossPosition)
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
        public async UniTask MapLoad()
        {
            var stageLocationList = await SystemManager.Instance.ResourceManager.GetLocations("Stage1"); //TODO : 추후 레이블 스테이지 리스트로 관리
            var objectList = await SystemManager.Instance.ResourceManager.LoadAssets<GameObject>(stageLocationList);
            _mapList = QT.Util.RandomSeed.GetRandomIndexes(objectList.ToList(),_maxRoomValue);
        }

        public async UniTask ShopLoad()
        {
            var stageShopLocationList = await SystemManager.Instance.ResourceManager.GetLocations("Stage1Shop"); //TODO : 추후 레이블 스테이지 리스트로 관리
            var shopObjectList = await SystemManager.Instance.ResourceManager.LoadAssets<GameObject>(stageShopLocationList);
            _shopMapList = shopObjectList.ToList();
        }

        public async UniTask StartRoomLoad()
        {
            var stageStartLocationList = await SystemManager.Instance.ResourceManager.GetLocations("Stage1Start"); //TODO : 추후 레이블 스테이지 리스트로 관리
            var startObjectList = await SystemManager.Instance.ResourceManager.LoadAssets<GameObject>(stageStartLocationList);
            _startList = startObjectList.ToList();
        }

        public async UniTask BossRoomLoad()
        {
            var stageBossLocationList = await SystemManager.Instance.ResourceManager.GetLocations("Stage1Boss"); //TODO : 추후 레이블 스테이지 리스트로 관리
            var bossObjectList = await SystemManager.Instance.ResourceManager.LoadAssets<GameObject>(stageBossLocationList);
            _bossMapList = bossObjectList.ToList();
        }

        public async UniTask StairsRoomLoad()
        {
            var stageStairsLocationList = await SystemManager.Instance.ResourceManager.GetLocations("Stage1Stairs"); //TODO : 추후 레이블 스테이지 리스트로 관리
            var stairsObjectList = await SystemManager.Instance.ResourceManager.LoadAssets<GameObject>(stageStairsLocationList);
            _stairsMapList = stairsObjectList.ToList();
        }

        public GameObject GetMapObject()
        {
            return _mapList[_mapCount++ % _mapList.Count];
        }

        public GameObject ShopMapObject()
        {
            return _shopMapList[Random.Range(0,_shopMapList.Count)];
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
        
        public CellData GetCellData(Vector2Int pos)
        {
            return _map[pos.y, pos.x];
        }

        public void NextFloor()
        {
            if (_floorValue == 2)
                return;
            _floorValue++;
            var _playerManager = SystemManager.Instance.PlayerManager;
            SystemManager.Instance.PlayerManager.AddItemEvent.RemoveAllListeners();
            _playerManager._playerIndexInventory.Clear();
            var playerInventory = _playerManager._playerIndexInventory;
            var itemList = SystemManager.Instance.PlayerManager.Player.Inventory.GetItemList();
            for (int i = 0; i < itemList.Length; i++)
            {
                playerInventory.Add(itemList[i].GetItemID());
            }

            _playerManager.globalGold = _playerManager.Player.GetGoldCost();
            var uiManager = SystemManager.Instance.UIManager;
            uiManager.GetUIPanel<FadeCanvas>().FadeOut(() =>
            {
                uiManager.GetUIPanel<MinimapCanvas>().OnClose();
                uiManager.GetUIPanel<FadeCanvas>().FadeIn();
                uiManager.GetUIPanel<LoadingCanvas>().OnOpen();
                SystemManager.Instance.PlayerManager.OnDamageEvent.RemoveAllListeners();
                SystemManager.Instance.UIManager.GetUIPanel<MinimapCanvas>().CellClear();
                SystemManager.Instance.PlayerManager.CurrentRoomEnemyRegister.Invoke(new List<IHitable>());
                SystemManager.Instance.ProjectileManager.ProjectileListClear();
                SystemManager.Instance.ResourceManager.AllReleasedObject();

                StartCoroutine(UnityUtil.WaitForFunc(() =>
                {
                    SystemManager.Instance.LoadingManager.FloorLoadScene(1);
                    DungenMapGenerate();
                    SystemManager.Instance.UIManager.GetUIPanel<MinimapCanvas>().MinimapSetting();
                },5f));
            });
        }

        public void SetFloor(int value)
        {
            _floorValue = value;
        }

        public float GetEnemyHpIncreasePer()
        {
            return SystemManager.Instance.DataManager.GetDataBase<ProductialMapGameDataBase>().GetData(800 + _floorValue).EnemyHpIncreasePer;
        }

        public int GetFloor()
        {
            return _floorValue;
        }
    }
}