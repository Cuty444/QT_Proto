using System;
using System.Collections;
using System.Collections.Generic;
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
            _waveHardCollider.enabled = false;
            _waveStartCollider.enabled = false;
            _mapCellData.PlayBossEnterRoom();
            DoorClose();
        }
        
    }
}
