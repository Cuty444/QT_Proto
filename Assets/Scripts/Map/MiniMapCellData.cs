using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;
using QT.Core;
using QT.Core.Map;
using QT.InGame;

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
    }
    
    public class MiniMapCellData : MonoBehaviour
    {
        [SerializeField] private Color[] _mapColors;
        [SerializeField] private GameObject _lineRenders;
        [SerializeField] private UILineRenderer[] _uiLineRenderers;
        [SerializeField] private Sprite[] _mapIconSprite;
        [SerializeField] private Transform _iconsTransform;
        [HideInInspector]public Vector2Int CellPos;

        private const string IconPath = "Prefabs/Map/MiniMap/MiniMapIcon.prefab";

        private PlayerManager _playerManager;
        private DungeonMapSystem _dungeonMapSystem;
        private GameObject _cellMapObject;
        private Image _mapImage;
        private MapDirection _pathOpenDirection;

        private MapCellData _mapCellData;

        private RoomType _roomType;

        private Image _iconObject;

        public void Setting()
        {
            _lineRenders.SetActive(false);
            _mapImage = GetComponent<Image>();
            _mapImage.color = _mapColors[2];
            _dungeonMapSystem = SystemManager.Instance.GetSystem<DungeonMapSystem>();
            _playerManager = SystemManager.Instance.PlayerManager;
            _playerManager.PlayerMapPosition.AddListener(CellPosCheck);
            _mapImage.enabled = false;
            _playerManager.PlayerCreateEvent.AddListener(PlayerCreateEvent);
            _iconsTransform.gameObject.SetActive(false);
            _iconObject = null;
            var image = _iconsTransform.GetComponentInChildren<Image>();
            if (image != null)
            {
                image.enabled = false;
                image.gameObject.SetActive(false);
                SystemManager.Instance.ResourceManager.ReleaseObject(IconPath, image);
            }
        }

        private void PlayerCreateEvent(Player obj)
        {
            _cellMapObject = _dungeonMapSystem.GetMapObject();
            _mapCellData = Instantiate(_cellMapObject, _dungeonMapSystem.MapCellsTransform).GetComponent<MapCellData>();
            _mapCellData.transform.position = new Vector3((CellPos.x * 40.0f)- _dungeonMapSystem.GetMiniMapSizeToMapSize().x, (CellPos.y * -40.0f) - _dungeonMapSystem.GetMiniMapSizeToMapSize().y, 0f);
            _mapCellData.OpenDoorDirection(_pathOpenDirection);
        }
        
        public void ListenerClear()
        {
            _playerManager.PlayerCreateEvent.RemoveListener(PlayerCreateEvent);
            _playerManager.PlayerMapPosition.RemoveListener(CellPosCheck);
            if (_iconObject != null)
            {
                _iconObject.gameObject.SetActive(false);
                SystemManager.Instance.ResourceManager.ReleaseObject(IconPath, _iconObject);
                _iconObject = null;
            }

            for (int i = 0; i < _uiLineRenderers.Length; i++)
            {
                _uiLineRenderers[i].enabled = false;
            }
            _mapImage.enabled = false;
            _mapImage.color = _mapColors[2];
            _lineRenders.SetActive(false);
        }

        public void SetRouteDirection(MapDirection mapDirection)
        {
            _uiLineRenderers[0].enabled = (mapDirection & MapDirection.Up) != 0;
            _uiLineRenderers[1].enabled = (mapDirection & MapDirection.Down) != 0;
            _uiLineRenderers[2].enabled = (mapDirection & MapDirection.Left) != 0;
            _uiLineRenderers[3].enabled = (mapDirection & MapDirection.Right) != 0;
            _pathOpenDirection = mapDirection;
        }
        

        public void PlayerEnterDoor(Vector2Int pos)
        {
            _mapCellData.DoorExitDirection(pos);
            if (!_dungeonMapSystem.GetCellData(CellPos).IsClear)
                _mapCellData.RoomPlay(CellPos);
            _playerManager.PlayerMapVisitedPosition.Invoke(CellPos);
            _playerManager.PlayerMapPosition.Invoke(CellPos);
        }

        private void CellPosCheck(Vector2Int pos)
        {
            if (pos == CellPos)
            {
                _lineRenders.SetActive(true);
                _mapImage.enabled = true;
                _iconsTransform.gameObject.SetActive(true);
                _mapImage.color = _mapColors[0];
                //ColorSetLineRender(_mapColors[0]);
            }
            else if (_dungeonMapSystem.GetCellData(CellPos).IsClear || _dungeonMapSystem.GetCellData(CellPos).IsVisited)
            {
                _lineRenders.SetActive(true);
                _mapImage.enabled = true;
                _mapImage.color = _mapColors[1];
                _iconsTransform.gameObject.SetActive(true);
                //ColorSetLineRender(_mapColors[1]);
            }
            else if (_dungeonMapSystem.MultiPathClearCheck(CellPos))
            {
                _mapImage.enabled = true;
                _iconsTransform.gameObject.SetActive(true);
            }
        }

        private void ColorSetLineRender(Color color) // TODO : 라인렌더가 곂침에 따른 쇼팅오류가 생김 수정 필요
        {
            for (int i = 0; i < _uiLineRenderers.Length; i++)
            {
                _uiLineRenderers[i].color = color;
            }
        }

        public async void SetRoomType(RoomType roomType)
        {
            _roomType = roomType;
            _iconObject = await SystemManager.Instance.ResourceManager.GetFromPool<Image>(IconPath,_iconsTransform);
            _iconObject.transform.localPosition = Vector3.zero;
            _iconObject.transform.localScale = Vector3.one;
            _iconObject.gameObject.SetActive(true);
            _iconObject.sprite = _mapIconSprite?[(int) RoomType.Boss];
        }
    }
}
