using System;
using System.Collections;
using System.Collections.Generic;
using QT.Core;
using QT.Core.Map;
using QT.InGame;
using UnityEngine;

namespace QT
{
    public class DungeonManager : MonoBehaviour
    {
        private DungeonMapSystem _dungeonMapSystem;
        private PlayerManager _playerManager;
        private MapData _mapData;
        private Vector2Int _currentPlayerPosition;
        
        private void Start()
        {
            _dungeonMapSystem = SystemManager.Instance.GetSystem<DungeonMapSystem>();
            _mapData = _dungeonMapSystem.DungeonMapData;
            _playerManager = SystemManager.Instance.PlayerManager;
            _currentPlayerPosition = _mapData.StartPosition;
            
            _playerManager.PlayerCreateEvent.AddListener(PlayerCreateEvent);
            _playerManager.PlayerMapPosition.AddListener(PlayerMapPosition);
            
            _dungeonMapSystem.DungeonReady(transform);
        }

        private void OnDestroy()
        {
            _playerManager.PlayerDoorEnter.RemoveListener(MapEnter);
            _playerManager.PlayerCreateEvent.RemoveListener(PlayerCreateEvent);
            _playerManager.PlayerMapPosition.RemoveListener(PlayerMapPosition);
        }

        
        private void PlayerCreateEvent(Player player)
        {
            _dungeonMapSystem.DungeonStart();
            _playerManager.PlayerMapPosition.Invoke(_mapData.StartPosition);
            _playerManager.PlayerMapVisitedPosition.Invoke(_mapData.StartPosition);
            _playerManager.PlayerMapClearPosition.Invoke(_mapData.StartPosition);
            
            _playerManager.PlayerDoorEnter.AddListener(MapEnter);
        }
        
        private void PlayerMapPosition(Vector2Int position)
        {
            _currentPlayerPosition = position;
        }

        private void MapEnter(Vector2Int nextDirection)
        {
            Vector2Int nextPosition = _currentPlayerPosition - nextDirection;
            _playerManager.PlayerMapVisitedPosition.Invoke(nextPosition);
            _playerManager.PlayerMapPosition.Invoke(nextPosition);
        }
    }
}
