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
        
        
        private const string CellPath = "Prefabs/Map/MiniMap/Cell.prefab";
        
        private PlayerManager _playerManager;
        
        private Dictionary<Vector2Int, MapDirection> _pathDirections = new Dictionary<Vector2Int, MapDirection>();
        private MapData _mapData;

        private List<MiniMapCellData> _cellList = new List<MiniMapCellData>();
        private Dictionary<MiniMapCellData, Vector2> _cellMapDictionary = new Dictionary<MiniMapCellData, Vector2>();
        private List<GameObject> _cellMapList = new List<GameObject>();
        private bool IsPreviousActive;
        private Vector2Int _currentPlayerPosition; // TODO : DungeonMapSystem으로 옮겨야함
        public override void Initialize()
        {
            _pathDirections.Add(Vector2Int.up,MapDirection.Up);
            _pathDirections.Add(Vector2Int.down,MapDirection.Down);
            _pathDirections.Add(Vector2Int.right,MapDirection.Left);
            _pathDirections.Add(Vector2Int.left,MapDirection.Right);
            SystemManager.Instance.ResourceManager.CacheAsset(CellPath);
            MinimapSetting();
            IsPreviousActive = true;
            _miniMapOnOff.SetActive(true);
            _playerManager = SystemManager.Instance.PlayerManager;
            _playerManager.PlayerDoorEnter.AddListener(NextMapWarp);
            _playerManager.PlayerMapPosition.AddListener((position) =>
            {
                _currentPlayerPosition = position;
                MiniMapCellCenterPositionChagne(position);
                if (!SystemManager.Instance.GetSystem<DungeonMapSystem>().GetCellData(position).IsClear)
                {
                    IsPreviousActive = false;
                    _miniMapOnOff.SetActive(false);
                }
            });
            _playerManager.PlayerCreateEvent.AddListener((player) =>
            {
                _playerManager.PlayerMapPosition.Invoke(_mapData.StartPosition);
                _playerManager.PlayerMapVisitedPosition.Invoke(_mapData.StartPosition);
                _playerManager.PlayerMapClearPosition.Invoke(_mapData.StartPosition);
                MapCreate();
            });
            _playerManager.PlayerMapClearPosition.AddListener((arg) =>
            {
                IsPreviousActive = true;
                _miniMapOnOff.SetActive(true);
            });
            
            SystemManager.Instance.UIManager.InventoryInputCheck.AddListener((isActive) =>
            {
                if (isActive)
                {
                    MapCreate();
                    if (IsPreviousActive)
                    {
                        _miniMapOnOff.SetActive(false);
                    }
                }
                else if(!isActive && IsPreviousActive)
                {
                    _miniMapOnOff.SetActive(true);
                }
            });
        }

        public void MinimapSetting()
        {
            _mapData = SystemManager.Instance.GetSystem<DungeonMapSystem>().DungeonMapData;
            _currentPlayerPosition = _mapData.StartPosition;
            MiniMapDraw();
        }


        public void CellClear()
        {
            foreach (var cell in _cellList)
            {
                cell.ListenerClear();
                //SystemManager.Instance.ResourceManager.ReleaseObject(CellPath, cell);
                Destroy(cell.gameObject);
            }
            _cellList.Clear();
            _cellMapDictionary.Clear();
        }
        
        public override void PostSystemInitialize()
        {
            gameObject.SetActive(true);
        }

        private void MiniMapDraw()
        {

            for (int i = 0; i< _mapData.MapNodeList.Count; i++)
            {
                MapDirection direction = DirectionCheck(_mapData.MapNodeList[i]);
                CellCreate(_mapData.MapNodeList[i],direction,i);
            }
            
            MiniMapCellCenterPositionChagne(_currentPlayerPosition);
        }
        
        private async void CellCreate(Vector2Int createPos,MapDirection direction,int index)
        {
            Vector3 pos = new Vector3(createPos.x * 200f, createPos.y * -200f, 0f);
            var cell = await SystemManager.Instance.ResourceManager.GetFromPool<MiniMapCellData>(CellPath,_miniMapCellTransform);
            cell.name = cell.name +"_"+ index.ToString();
            cell.transform.localScale = Vector3.one;
            cell.transform.localPosition = pos;
            cell.SetRouteDirection(direction);
            cell.CellPos = createPos;
            if (createPos == _mapData.BossRoomPosition)
            {
                cell.name = cell.name + "_Boss";
                cell.SetRoomType(RoomType.Boss);
            }
            else if (createPos == _mapData.ShopRoomPosition)
            {
                cell.name = cell.name + "_Shop";
                cell.SetRoomType(RoomType.GoldShop);
            }
            else if (createPos == _mapData.StartPosition)
            {
                cell.name = cell.name + "_Start";
                cell.SetRoomType(RoomType.Start);
            }
            cell.Setting();
            _cellList.Add(cell);
            _cellMapDictionary.Add(cell,pos);
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
                if (_mapData.Map[nodeValue.y, nodeValue.x].RoomType != RoomType.Normal)
                {
                    continue;
                }
                mapDirection |= direction.Value;
            }

            return mapDirection;
        }

        private void NextMapWarp(Vector2Int nextDirection)
        {
            Vector2Int nextPosition = _currentPlayerPosition - nextDirection;
            for (int i = 0; i < _cellList.Count; i++)
            {
                if (_cellList[i].CellPos == nextPosition)
                {
                    _cellList[i].PlayerEnterDoor(nextDirection);
                    break;
                }
            }
        }

        private void MiniMapCellCenterPositionChagne(Vector2Int position)
        {
            Vector3 pos = new Vector3(position.x * 200f, position.y * -200f, 0f);
            _miniMapCellTransform.transform.localPosition = -pos;
        }

        private void MapCreate()
        {
            foreach (var cell in _cellMapList)
            {
                Destroy(cell.gameObject);
            }
            _cellMapList.Clear();
            var pool = SystemManager.Instance.UIManager.GetUIPanel<UIInventoryCanvas>().MapTransform;
            foreach (var cell in _cellMapDictionary)
            {
                var obj = Instantiate(cell.Key.gameObject, pool);
                obj.transform.localScale = Vector3.one;
                obj.transform.localPosition = cell.Value;
                _cellMapList.Add(obj);
            }
        }
    }
}
