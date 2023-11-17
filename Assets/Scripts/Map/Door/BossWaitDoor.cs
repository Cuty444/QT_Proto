using System;
using System.Collections;
using System.Collections.Generic;
using QT.Core;
using QT.Core.Map;
using QT.Map;
using UnityEngine;

namespace QT
{
    public class BossWaitDoor : Door
    {
        [SerializeField] private BoxCollider2D _waveStartCollider;
        [SerializeField] private BoxCollider2D _waveHardCollider;
        [HideInInspector] public MapCellData _mapCellData;
        
        private void Start()
        {
            DoorOpen();
        }

        public void PlayerEnter()
        {
            _waveHardCollider.enabled = true;
            _waveStartCollider.enabled = false;
            _mapCellData.PlayBossEnterRoom();
            SystemManager.Instance.GetSystem<DungeonMapSystem>().IsBossWaitEnter = true;
            DoorClose();
        }

        public void BossClear()
        {
            _waveHardCollider.enabled = false;
            SystemManager.Instance.GetSystem<DungeonMapSystem>().IsBossWaitEnter = false;
            DoorOpen();
        }
        
    }
}
