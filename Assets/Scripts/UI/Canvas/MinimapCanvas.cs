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

        private Vector2Int _currentPlayerPosition; // TODO : DungeonMapSystem으로 옮겨야함
        public override void Initialize()
        {
            _pathDirections.Add(Vector2Int.up,MapDirection.Up);
            _pathDirections.Add(Vector2Int.down,MapDirection.Down);
            _pathDirections.Add(Vector2Int.right,MapDirection.Left);
            _pathDirections.Add(Vector2Int.left,MapDirection.Right);
            SystemManager.Instance.ResourceManager.CacheAsset(CellPath);
            MinimapSetting();
            _miniMapOnOff.SetActive(false);
            _playerManager = SystemManager.Instance.PlayerManager;
            _playerManager.PlayerDoorEnter.AddListener(NextMapWarp);
            _playerManager.PlayerMapPosition.AddListener((position) =>
            {
                _currentPlayerPosition = position;
                MiniMapCellCenterPositionChagne(position);
            });
            _playerManager.PlayerCreateEvent.AddListener((player) =>
            {
                _playerManager.PlayerMapPosition.Invoke(_mapData.StartPosition);
                _playerManager.PlayerMapVisitedPosition.Invoke(_mapData.StartPosition);
                _playerManager.PlayerMapClearPosition.Invoke(_mapData.StartPosition);
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
                SystemManager.Instance.ResourceManager.ReleaseObject(CellPath, cell);
            }
            _cellList.Clear();
        }
        
        public override void PostSystemInitialize()
        {
            gameObject.SetActive(true);
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
            cell.Setting();
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
    }
}
