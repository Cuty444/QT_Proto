using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using QT.Core;
using QT.Core.Map;
using QT.InGame;
using UnityEngine;

namespace QT.Map
{
    public class MapEnemySpawner : MonoBehaviour
    {
        private List<Enemy> _enemyList;

        private Vector2Int _cellPos;

        private DungeonMapSystem _dungeonMapSystem;

        private PlayerManager _playerManager;

        private void Awake()
        {
            _enemyList = GetComponentsInChildren<Enemy>().ToList();
            _dungeonMapSystem = SystemManager.Instance.GetSystem<DungeonMapSystem>();
            _playerManager = SystemManager.Instance.PlayerManager;
            _playerManager.CurrentRoomEnemyRegister.Invoke(_enemyList);
        }

        private void Update()
        {
            if (_dungeonMapSystem.GetCellData(_cellPos).IsClear)
                return;
            MapClearCheck();
        }

        private void MapClearCheck()
        {
            for (int i = 0; i < _enemyList.Count; i++)
            {
                if (_enemyList[i].CurrentStateIndex >= (int) Enemy.States.Rigid)
                {
                    if (_enemyList[i].HP <= 0)
                    {
                        _enemyList.RemoveAt(i);
                        i = 0;
                    }
                }
            }

            if (_enemyList.Count == 0)
            {
                _playerManager.PlayerMapClearPosition.Invoke(_cellPos); // TODO : 추후 적 처치시 맵 클리어 부분에 옮겨야함
                _playerManager.PlayerMapPass.Invoke(true);
            }
        }

        public void SetPos(Vector2Int position)
        {
            _cellPos = position;
        }
    }
}
