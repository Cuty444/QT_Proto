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

        private Dullahan _dullahan = null;
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

            if (SystemManager.Instance.GetSystem<DungeonMapSystem>().DungeonMapData.BossRoomPosition == _cellPos)
            {
                _dullahan = GetComponentInChildren<Dullahan>();
            }
            
            for (int i = 0; i < _enemyElitList.Count; i++)
            {
                _enemyList.Remove(_enemyElitList[i]);
                _enemyElitList[i].gameObject.SetActive(false);
            }
            _dungeonMapSystem = SystemManager.Instance.GetSystem<DungeonMapSystem>();
            _playerManager = SystemManager.Instance.PlayerManager;
            _playerManager.PlayerMapPosition.AddListener((pos) =>
            {
                if (_cellPos == pos)
                {
                    _playerManager.CurrentRoomEnemyRegister.Invoke(transform.parent.parent.gameObject.GetComponentsInChildren<IHitable>().ToList());
                }
            });
            //_playerManager.CurrentRoomEnemyRegister.Invoke(transform.parent.parent.gameObject.GetComponentsInChildren<IHitable>().ToList());
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

            if (SystemManager.Instance.GetSystem<DungeonMapSystem>().DungeonMapData.BossRoomPosition == _cellPos)
            {
                if (_dullahan != null)
                {
                    if (_dullahan.CurrentStateIndex == (int)Dullahan.States.Dead)
                    {
                        _playerManager.PlayerMapClearPosition.Invoke(_cellPos); // TODO : 추후 적 처치시 맵 클리어 부분에 옮겨야함
                        SystemManager.Instance.SoundManager.PlayOneShot(SystemManager.Instance.SoundManager.SoundData.Door_OpenSFX);
                        _playerManager.PlayerMapPass.Invoke(true);
                        //SystemManager.Instance.UIManager.GetUIPanel<RecordCanvas>().OnOpen();
                    }
                }
                else
                {
                    _playerManager.PlayerMapClearPosition.Invoke(_cellPos); // TODO : 추후 적 처치시 맵 클리어 부분에 옮겨야함
                    SystemManager.Instance.SoundManager.PlayOneShot(SystemManager.Instance.SoundManager.SoundData.Door_OpenSFX);
                    _playerManager.PlayerMapPass.Invoke(true);
                }
            }
            else
            {
                if (_enemyList.Count == 0)
                {
                    _playerManager.PlayerMapClearPosition.Invoke(_cellPos); // TODO : 추후 적 처치시 맵 클리어 부분에 옮겨야함
                    SystemManager.Instance.SoundManager.PlayOneShot(SystemManager.Instance.SoundManager.SoundData.Door_OpenSFX);
                    _playerManager.PlayerMapPass.Invoke(true);
                }
            }
        }

        public void SetPos(Vector2Int position)
        {
            _cellPos = position;
        }
    }
}
