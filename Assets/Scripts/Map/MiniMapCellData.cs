using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;
using QT.Core;
using QT.Core.Map;
using QT.InGame;
using UnityEngine.Events;

namespace QT.Map
{
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
    
    public class MiniMapCellData : MonoBehaviour
    {
        [SerializeField] private Sprite[] _mapSprites;
        [SerializeField] private Color[] _mapColors;
        [SerializeField] private GameObject _lineRenders;
        [SerializeField] private UILineRenderer[] _uiLineRenderers;
        [SerializeField] private Image[] _mapLineImages;
        [SerializeField] private Sprite[] _mapIconSprite;
        [SerializeField] private Transform _iconsTransform;
        [HideInInspector]public Vector2Int CellPos;

        //private const string IconPath = "Prefabs/Map/MiniMap/MiniMapIcon.prefab";

        private PlayerManager _playerManager;
        private DungeonMapSystem _dungeonMapSystem;
        private GameObject _cellMapObject;
        private Image _mapImage;
        private MapDirection _pathOpenDirection;

        private MapCellData _mapCellData;

        private RoomType _roomType;

        [SerializeField] private Image _iconObject;

        public void Setting()
        {
            _lineRenders.SetActive(false);
            _mapImage = GetComponent<Image>();
            _dungeonMapSystem = SystemManager.Instance.GetSystem<DungeonMapSystem>();
            _playerManager = SystemManager.Instance.PlayerManager;
            _playerManager.PlayerCreateEvent.AddListener(PlayerCreateEvent);
            _playerManager.PlayerMapPosition.AddListener(CellPosCheck);
            _mapImage.enabled = false;
            _iconsTransform.gameObject.SetActive(false);
            
            if (RoomType.None == _roomType)
            {
                _iconObject.sprite = _mapIconSprite?[0];
            }
        }

        private void PlayerCreateEvent(Player obj)
        {
            switch (_roomType)
            {
                case RoomType.GoldShop:
                case RoomType.HpShop:
                    _cellMapObject = _dungeonMapSystem.ShopMapObject();
                    break;
                case RoomType.Start:
                    _cellMapObject = _dungeonMapSystem.StartMapObject();
                    break;
                case RoomType.Boss:
                    _cellMapObject = _dungeonMapSystem.BossMapObject();
                    break;
                case RoomType.Stairs:
                    _cellMapObject = _dungeonMapSystem.StairsMapObject();
                    break;
                case RoomType.None:
                case RoomType.Normal:
                default:
                    _cellMapObject = _dungeonMapSystem.GetMapObject();
                    break;
            }
            _mapCellData = Instantiate(_cellMapObject, _dungeonMapSystem.MapCellsTransform).GetComponent<MapCellData>();
            _mapCellData.transform.position = new Vector3((CellPos.x * 40.0f)- _dungeonMapSystem.GetMiniMapSizeToMapSize().x, (CellPos.y * -40.0f) - _dungeonMapSystem.GetMiniMapSizeToMapSize().y, 0f);
            _mapCellData.CellDataSet(_pathOpenDirection,CellPos,_roomType);
        }
        
        public void ListenerClear()
        {
            _playerManager.PlayerCreateEvent.RemoveListener(PlayerCreateEvent);
            _playerManager.PlayerMapPosition.RemoveListener(CellPosCheck);
            _roomType = RoomType.None;


            for (int i = 0; i < _mapLineImages.Length; i++)
            {
                _mapLineImages[i].enabled = false;
            }
            _mapImage.enabled = false;
            _mapImage.color = _mapColors[2];
            _mapImage.sprite = _mapSprites[2];
            _lineRenders.SetActive(false);
            Destroy(gameObject);
            if (_mapCellData != null)
            {
                Destroy(_mapCellData.gameObject);
            }
        }

        public void SetRouteDirection(MapDirection mapDirection)
        {
            _mapLineImages[0].enabled = (mapDirection & MapDirection.Up) != 0;
            _mapLineImages[1].enabled = (mapDirection & MapDirection.Down) != 0;
            _mapLineImages[2].enabled = (mapDirection & MapDirection.Left) != 0;
            _mapLineImages[3].enabled = (mapDirection & MapDirection.Right) != 0;
            _pathOpenDirection = mapDirection;
        }
        

        public void PlayerEnterDoor(Vector2Int pos)
        {
            _mapCellData.DoorExitDirection(pos);
            if (!_dungeonMapSystem.GetCellData(CellPos).IsClear)
                _mapCellData.PlayRoom(CellPos);
            _playerManager.PlayerMapVisitedPosition.Invoke(CellPos);
            _playerManager.PlayerMapPosition.Invoke(CellPos);
        }

        private void CellPosCheck(Vector2Int pos)
        {
            if (pos == CellPos)
            {
                if (pos == _dungeonMapSystem.DungeonMapData.ShopRoomPosition)
                {
                    SystemManager.Instance.SoundManager.PlayBGM(SystemManager.Instance.SoundManager.SoundData.ShopStageBGM);
                }
                else if (pos == _dungeonMapSystem.DungeonMapData.BossRoomPosition)
                {
                    //TODO : BossHPCanavas 에서 제어중
                }
                else
                {
                    SystemManager.Instance.SoundManager.PlayBGM(SystemManager.Instance.SoundManager.SoundData.Stage1BGM);
                }
                _lineRenders.SetActive(true);
                _mapImage.enabled = true;
                _iconObject.sprite = _mapIconSprite[7];
                _iconsTransform.gameObject.SetActive(true);
                _mapImage.color = _mapColors[0];
                _mapImage.sprite = _mapSprites[3];
                
                if (_roomType == RoomType.Stairs)
                {
                    _mapImage.sprite = _mapSprites[4];
                }

                if (_mapCellData != null)
                {
                    _playerManager.OnVolumeProfileChange.Invoke(_mapCellData.VolumeProfile);
                }
            }
            else if (_dungeonMapSystem.GetCellData(CellPos).IsClear)
            {
                _lineRenders.SetActive(true);
                _mapImage.enabled = true;
                _mapImage.color = _mapColors[0];
                _mapImage.sprite = _mapSprites[0];
                if (_roomType == RoomType.Stairs)
                {
                    _mapImage.sprite = _mapSprites[4];
                }
                _iconObject.sprite = _mapIconSprite?[(int) _roomType];
                _iconsTransform.gameObject.SetActive(true);
                //ColorSetLineRender(_mapColors[1]);
            }
            else if (_dungeonMapSystem.GetCellData(CellPos).IsVisited)
            {
                _lineRenders.SetActive(true);
                _mapImage.enabled = true;
                _mapImage.color = _mapColors[1];
                _mapImage.sprite = _mapSprites[1];
                if (_roomType == RoomType.Stairs)
                {
                    _mapImage.sprite = _mapSprites[4];
                }
                _iconObject.sprite = _mapIconSprite?[(int) _roomType];
                _iconsTransform.gameObject.SetActive(true);
            }
            else if (_dungeonMapSystem.MultiPathClearCheck(CellPos))
            {
                _mapImage.enabled = true;
                _mapImage.color = _mapColors[1];
                _mapImage.sprite = _mapSprites[2];
                //_iconsTransform.gameObject.SetActive(true);
            }
        }

        private void ColorSetLineRender(Color color) // TODO : 라인렌더가 곂침에 따른 쇼팅오류가 생김 수정 필요
        {
            for (int i = 0; i < _uiLineRenderers.Length; i++)
            {
                _uiLineRenderers[i].color = color;
            }
        }

        public void SetRoomType(RoomType roomType)
        {
            _roomType = roomType;
            _iconObject.gameObject.SetActive(true);
            if (_roomType == RoomType.Boss)
            {
                if (SystemManager.Instance.GetSystem<DungeonMapSystem>().GetFloor() < 2)
                {
                    _roomType = RoomType.Stairs;
                }
            }
            _iconObject.sprite = _mapIconSprite?[(int) _roomType];
        }
    }
}
