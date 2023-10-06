using System;
using System.Collections;
using System.Collections.Generic;
using QT.Core;
using QT.Core.Map;
using QT.InGame;
using QT.UI;
using UnityEngine;

namespace QT
{
    public class DungeonManager : MonoBehaviour
    {
        private DungeonMapSystem _dungeonMapSystem;
        private PlayerManager _playerManager;
        private MapData _mapData;
        
        private void Start()
        {
            _dungeonMapSystem = SystemManager.Instance.GetSystem<DungeonMapSystem>();
            _mapData = _dungeonMapSystem.DungeonMapData;
            _playerManager = SystemManager.Instance.PlayerManager;
            
            _playerManager.PlayerCreateEvent.AddListener(PlayerCreateEvent);
            _playerManager.PlayerMapTeleportPosition.AddListener(MapTeleport);
            
            _dungeonMapSystem.DungeonReady(transform);
        }

        private void OnDestroy()
        {
            _playerManager.PlayerDoorEnter.RemoveListener(MapEnter);
            _playerManager.PlayerCreateEvent.RemoveListener(PlayerCreateEvent);
            _playerManager.PlayerMapTeleportPosition.RemoveListener(MapTeleport);
        }

        
        private void PlayerCreateEvent(Player player)
        {
            _dungeonMapSystem.DungeonStart();
            _playerManager.PlayerMapPosition.Invoke(_mapData.StartPosition);
            _playerManager.PlayerMapVisitedPosition.Invoke(_mapData.StartPosition);
            _playerManager.PlayerMapClearPosition.Invoke(_mapData.StartPosition);
            _playerManager.PlayerDoorEnter.AddListener(MapEnter);
            
            _playerManager.Player._currentPlayerPosition = _mapData.StartPosition;
            
            SystemManager.Instance.UIManager.SetState(UIState.InGame);
        }
        
        private void MapEnter(Vector2Int nextDirection)
        {
            Vector2Int nextPosition = _playerManager.Player._currentPlayerPosition - nextDirection;
            _playerManager.PlayerMapVisitedPosition.Invoke(nextPosition);
            _playerManager.PlayerMapPosition.Invoke(nextPosition);
            
            _playerManager.Player._currentPlayerPosition = nextPosition;
        }

        private void MapTeleport(Vector2Int position)
        {
            _playerManager.PlayerMapPosition.Invoke(position);
            
            _playerManager.Player._currentPlayerPosition = position;
        }
    }
}
