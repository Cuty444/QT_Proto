using QT.Core;
using QT.Core.Map;
using QT.InGame;
using QT.UI;
using UnityEngine;

namespace QT
{
    public class DungeonManager : MonoSingleton<DungeonManager>
    {
        private DungeonMapSystem _dungeonMapSystem;
        private PlayerManager _playerManager;
        private MapData _mapData;

        public Vector2Int PlayerPosition { get; private set; }

        private void Awake()
        {
        }
        
        private async void Start()
        {
            _dungeonMapSystem = SystemManager.Instance.GetSystem<DungeonMapSystem>();
            _mapData = _dungeonMapSystem.DungeonMapData;
            _playerManager = SystemManager.Instance.PlayerManager;
            
            _playerManager.PlayerCreateEvent.AddListener(PlayerCreateEvent);
            _playerManager.PlayerMapTeleportPosition.AddListener(MapTeleport);
            
            _dungeonMapSystem.DungeonReady(transform);
            
            var minimapCanvas = await SystemManager.Instance.UIManager.Get<MinimapCanvasModel>();
            minimapCanvas.SetMiniMap(_mapData);
            minimapCanvas.ChangeCenter(_mapData.StartPosition);
            
            var phoneCanvas = await SystemManager.Instance.UIManager.Get<PhoneCanvasModel>();
            phoneCanvas.SetMiniMap(_mapData);
        }

        private void OnDestroy()
        {
            _playerManager.PlayerDoorEnter.RemoveListener(MapEnter);
            _playerManager.PlayerMapClearPosition.RemoveListener(MapClear);
            _playerManager.PlayerCreateEvent.RemoveListener(PlayerCreateEvent);
            _playerManager.PlayerMapTeleportPosition.RemoveListener(MapTeleport);
        }

        
        private void PlayerCreateEvent(Player player)
        {
            _dungeonMapSystem.DungeonStart();
            _playerManager.PlayerDoorEnter.AddListener(MapEnter);
            _playerManager.PlayerMapClearPosition.AddListener(MapClear);
            
            _playerManager.PlayerMapPosition.Invoke(_mapData.StartPosition);
            _playerManager.PlayerMapVisitedPosition.Invoke(_mapData.StartPosition);
            _playerManager.PlayerMapClearPosition.Invoke(_mapData.StartPosition);
            
            PlayerPosition = _mapData.StartPosition;
        }
        
        private async void MapEnter(Vector2Int nextDirection)
        {
            Vector2Int nextPosition = PlayerPosition - nextDirection;
            _playerManager.PlayerMapVisitedPosition.Invoke(nextPosition);
            _playerManager.PlayerMapPosition.Invoke(nextPosition);
            
            PlayerPosition = nextPosition;
            
            var data = SystemManager.Instance.GetSystem<DungeonMapSystem>().GetCellData(nextPosition);
            
            (await SystemManager.Instance.UIManager.Get<MinimapCanvasModel>()).ChangeCenter(nextPosition);
            SystemManager.Instance.UIManager.SetState(data.IsClear ? UIState.InGame : UIState.Battle);

            switch (data.RoomType)
            {
                case RoomType.GoldShop:
                    SystemManager.Instance.SoundManager.PlayBGM(SystemManager.Instance.SoundManager.SoundData.ShopStageBGM);
                    break;
                case RoomType.Boss:
                    SystemManager.Instance.SoundManager.PlayBGM(SystemManager.Instance.SoundManager.SoundData.BossStageBGM);
                    break;
                default:
                    SystemManager.Instance.SoundManager.PlayBGM(SystemManager.Instance.SoundManager.SoundData.Stage1BGM);
                    break;
            }
        }

        private void MapClear(Vector2Int position)
        {
            SystemManager.Instance.UIManager.SetState(UIState.InGame);
        }

        private async void MapTeleport(Vector2Int position)
        {
            _playerManager.PlayerMapPosition.Invoke(position);
            (await SystemManager.Instance.UIManager.Get<MinimapCanvasModel>()).ChangeCenter(position);
            
            PlayerPosition = position;
        }
    }
}
