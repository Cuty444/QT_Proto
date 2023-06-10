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
        private readonly string _eliteName = "Elite";

        private void Awake()
        {
            _enemyList = GetComponentsInChildren<Enemy>().ToList();
            List<Enemy> _enemyElitList = new List<Enemy>();
            for (int i = 0; i < _enemyList.Count; i++)
            {
                if (SystemManager.Instance.GetSystem<DungeonMapSystem>().GetFloor() == 0)
                {
                    if (_enemyList[i].gameObject.name.LastIndexOf(_eliteName, StringComparison.Ordinal) >= 0)
                    {
                        _enemyElitList.Add(_enemyList[i]);
                    }
                }
            }
            for (int i = 0; i < _enemyElitList.Count; i++)
            {
                _enemyList.Remove(_enemyElitList[i]);
                _enemyElitList[i].gameObject.SetActive(false);
            }
            _dungeonMapSystem = SystemManager.Instance.GetSystem<DungeonMapSystem>();
            _playerManager = SystemManager.Instance.PlayerManager;
            _playerManager.CurrentRoomEnemyRegister.Invoke(GetComponentsInChildren<IHitable>().ToList());
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
