using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using QT.Core;
using UnityEngine;

namespace QT.Map
{
    public class MapEnemySpawner : MonoBehaviour
    {
        private List<Enemy.Enemy> _enemyList;

        private Vector2Int _cellPos;
        
        private bool _isClear;

        private void Awake()
        {
            _enemyList = GetComponentsInChildren<Enemy.Enemy>().ToList();
        }

        private void Update()
        {
            if (_isClear)
                return;
            MapClearCheck();
        }

        private void MapClearCheck()
        {
            for (int i = 0; i < _enemyList.Count; i++)
            {
                if (_enemyList[i].CurrentState > (int) Enemy.Enemy.States.Rigid)
                {
                    _enemyList.RemoveAt(i);
                    i = 0;
                }
            }

            if (_enemyList.Count == 0)
            {
                SystemManager.Instance.PlayerManager.PlayerMapClearPosition.Invoke(_cellPos); // TODO : 추후 적 처치시 맵 클리어 부분에 옮겨야함
                _isClear = true;
            }
        }

        public void SetPos(Vector2Int position)
        {
            _cellPos = position;
        }
    }
}
