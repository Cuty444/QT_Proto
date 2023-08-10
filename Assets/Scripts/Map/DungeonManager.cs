using System;
using System.Collections;
using System.Collections.Generic;
using QT.Core;
using QT.Core.Map;
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
            _playerManager.PlayerCreateEvent.AddListener((player) =>
            {
                _dungeonMapSystem.DungeonStart();
                _playerManager.PlayerMapPosition.Invoke(_mapData.StartPosition);
                _playerManager.PlayerMapVisitedPosition.Invoke(_mapData.StartPosition);
                _playerManager.PlayerMapClearPosition.Invoke(_mapData.StartPosition);
                _playerManager.PlayerDoorEnter.AddListener(MapEnter);

            });

            _playerManager.PlayerMapPosition.AddListener((position) =>
            {
                _currentPlayerPosition = position;
            });
            _dungeonMapSystem.DungeonReady(transform);

        }

        private void MapEnter(Vector2Int nextDirection)
        {
            Vector2Int nextPosition = _currentPlayerPosition - nextDirection;
            _playerManager.PlayerMapVisitedPosition.Invoke(nextPosition);
            _playerManager.PlayerMapPosition.Invoke(nextPosition);
        }
    }
}
