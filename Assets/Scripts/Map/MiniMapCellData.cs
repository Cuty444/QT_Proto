using System;
using UnityEngine;
using UnityEngine.UI;
using QT.Core;
using QT.Core.Map;
using UnityEngine.Serialization;

namespace QT.Map
{
    public class MiniMapCellData : MonoBehaviour
    {
        public RectTransform RectTransform => transform as RectTransform;
        
        [SerializeField] private Sprite _playerSprite;
        [SerializeField] private Sprite _startSprite;
        [SerializeField] private Sprite _normalSprite;
        [SerializeField] private Sprite _unknownSprite;
        
        [SerializeField] private Sprite _rewardSprite;
        [SerializeField] private Sprite _hpHealSprite;
        [SerializeField] private Sprite _shopSprite;
        [SerializeField] private Sprite _stairsSprite;
        [SerializeField] private Sprite _bossSprite;
        
        
        [SerializeField] private GameObject _lines;
        [SerializeField] private Image[] _mapLineImages;
        
        [SerializeField] private Transform _iconsTransform;
        
        [HideInInspector]public Vector2Int CellPos;

        //private const string IconPath = "Prefabs/Map/MiniMap/MiniMapIcon.prefab";

        private PlayerManager _playerManager;
        private DungeonMapSystem _dungeonMapSystem;
        private GameObject _cellMapObject;
        private Image _mapImage;

        private RoomType _roomType;

        private Button _clickTeleportButton;

        [SerializeField] private Image _iconObject;
        
        public void Setting()
        {
            _lines.SetActive(false);
            _mapImage = GetComponent<Image>();
            
            _dungeonMapSystem = SystemManager.Instance.GetSystem<DungeonMapSystem>();
            _playerManager = SystemManager.Instance.PlayerManager;
            _playerManager.PlayerMapPosition.AddListener(CellPosCheck);
            
            _mapImage.enabled = false;
            _iconsTransform.gameObject.SetActive(false);
        }

        public void ClearListener()
        {
            _playerManager.PlayerMapPosition.RemoveListener(CellPosCheck);
            _roomType = RoomType.None;
            
            Destroy(gameObject);
        }

        public void SetRouteDirection(MapDirection mapDirection)
        {
            _mapLineImages[0].enabled = (mapDirection & MapDirection.Up) != 0;
            _mapLineImages[1].enabled = (mapDirection & MapDirection.Down) != 0;
            _mapLineImages[2].enabled = (mapDirection & MapDirection.Left) != 0;
            _mapLineImages[3].enabled = (mapDirection & MapDirection.Right) != 0;
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
            }
            
            
            var cellData = _dungeonMapSystem.GetCellData(CellPos);
            var isCellVisible = cellData.IsClear || cellData.IsVisited;
            
            _lines.SetActive(isCellVisible);
            _mapImage.enabled = isCellVisible;

            if (isCellVisible)
            {
                if (pos == CellPos)
                {
                    _mapImage.sprite = _playerSprite;
                    return;
                }
                
                switch (_roomType)
                {
                    case RoomType.Start:
                        _mapImage.sprite = _startSprite;
                        break;
                    case RoomType.Boss:
                        _mapImage.sprite = _bossSprite;
                        break;
                    case RoomType.Stairs:
                        _mapImage.sprite = _stairsSprite;
                        break;
                    case RoomType.Reward:
                        _mapImage.sprite = _rewardSprite;
                        break;
                    case RoomType.HpHeal:
                        _mapImage.sprite = _hpHealSprite;
                        break;
                    case RoomType.GoldShop:
                        _mapImage.sprite = _shopSprite;
                        break;
                    
                    default:
                        _mapImage.sprite = _normalSprite;
                        break;
                }
                
            }
            else if (_dungeonMapSystem.MultiPathClearCheck(CellPos))
            {
                _mapImage.enabled = true;
                _mapImage.sprite = _roomType == RoomType.Boss ? _bossSprite : _unknownSprite; // 보스방은 상시 표시
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

            if (_roomType != RoomType.Normal)
            {
                _clickTeleportButton = gameObject.AddComponent<Button>();
                ColorBlock buttonColors = _clickTeleportButton.colors;
                buttonColors.disabledColor = Color.white;
                _clickTeleportButton.colors = buttonColors;
                _clickTeleportButton.onClick.AddListener(CellTeleportEvent);
                Debug.Log(_roomType.ToString(),gameObject);
            }
        }

        public void CellTeleportEvent()
        {
            var playerPos = DungeonManager.Instance.PlayerPosition;
            
            if (playerPos == CellPos) return;
            if (!_dungeonMapSystem.GetCellData(CellPos).IsVisited) return;
            if (!_dungeonMapSystem.GetCellData(playerPos).IsClear) return; 
            
            _playerManager.Player.Warp(CellPos);
        }
    }
}
