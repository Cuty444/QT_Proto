using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;
using QT.Core;
using QT.Core.Map;

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
        [SerializeField] private GameObject _miniMapIconObject;
        [HideInInspector]public Vector2Int CellPos;

        private PlayerManager _playerManager;
        private DungeonMapSystem _dungeonMapSystem;
        private GameObject _cellMapObject;
        private Image _mapImage;
        private bool _isClear;   // 맵 클리어여부(몬스터를 죽임)
        private MapDirection _pathOpenDirection;

        private MapCellData _mapCellData;

        private RoomType _roomType;
        private void Awake()
        {
            _lineRenders.SetActive(false);
            _mapImage = GetComponent<Image>();
            _mapImage.color = _mapColors[2];
            _dungeonMapSystem = SystemManager.Instance.GetSystem<DungeonMapSystem>();
            _playerManager = SystemManager.Instance.PlayerManager;
            _playerManager.PlayerMapPosition.AddListener(CellPosCheck);
            _playerManager.PlayerMapClearPosition.AddListener(CellMapClearCheck);
            _mapImage.enabled = false;
            _playerManager.PlayerCreateEvent.AddListener((obj) =>
            {
                _cellMapObject = _dungeonMapSystem.GetMapObject();
                _mapCellData = Instantiate(_cellMapObject, _dungeonMapSystem.MapCellsTransform).GetComponent<MapCellData>();
                _mapCellData.transform.position = new Vector3((CellPos.x * 40.0f)- _dungeonMapSystem.GetMiniMapSizeToMapSize().x, (CellPos.y * -40.0f) - _dungeonMapSystem.GetMiniMapSizeToMapSize().y, 0f);
                _mapCellData.OpenDoorDirection(_pathOpenDirection);
            });
            _iconsTransform.gameObject.SetActive(false);
        }

        public void SetRouteDirection(MapDirection mapDirection)
        {
            _uiLineRenderers[0].enabled = (mapDirection & MapDirection.Up) != 0;
            _uiLineRenderers[1].enabled = (mapDirection & MapDirection.Down) != 0;
            _uiLineRenderers[2].enabled = (mapDirection & MapDirection.Left) != 0;
            _uiLineRenderers[3].enabled = (mapDirection & MapDirection.Right) != 0;
            _pathOpenDirection = mapDirection;
        }

        public void StartRoomCellClear() // 시작방 클리어 설정
        {
            _isClear = true;
            _playerManager.PlayerMapClearPosition.RemoveListener(CellMapClearCheck);
        }

        public void PlayerEnterDoor(Vector2Int pos)
        {
            _mapCellData.DoorExitDirection(pos);
            if (!_isClear)
                _mapCellData.RoomPlay(CellPos);
            _playerManager.PlayerMapVisitedPosition.Invoke(CellPos); // TODO : 맵 클리어 체크 부분 정리가 필요함
            //_playerManager.PlayerMapClearPosition.Invoke(CellPos); // TODO : 추후 적 처치시 맵 클리어 부분에 옮겨야함
            _playerManager.PlayerMapPosition.Invoke(CellPos);
        }
        
        private void CellMapClearCheck(Vector2Int pos)
        {
            _isClear = pos == CellPos;
            if (_isClear)
            {
                _playerManager.PlayerMapClearPosition.RemoveListener(CellMapClearCheck);
                
            }
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
            else if (_isClear)
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

        public void SetRoomType(RoomType roomType)
        {
            _roomType = roomType;
            GameObject iconObject = Instantiate(_miniMapIconObject, _iconsTransform);
            iconObject.GetComponent<Image>().sprite = _mapIconSprite?[(int) RoomType.Boss];
        }
    }
}
