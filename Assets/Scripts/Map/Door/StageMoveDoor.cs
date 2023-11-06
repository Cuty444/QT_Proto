using System;
using System.Collections;
using System.Collections.Generic;
using QT.Core;
using UnityEngine;
using UnityEngine.UIElements;

namespace QT
{
    public class StageMoveDoor : Door
    {
        private Vector3 _enterPosition;

        private PlayerManager _playerManager;

        private void Start()
        {
            _playerManager = SystemManager.Instance.PlayerManager;
        }

        public void StageMoveDoorInit(Vector3 position)
        {
            _enterPosition = position;
        }
        private void OnTriggerEnter2D(Collider2D col)
        {
            _playerManager.Player.transform.position = _enterPosition;
            _playerManager.Player.LastSafePosition = _enterPosition;
        }

    }
}
