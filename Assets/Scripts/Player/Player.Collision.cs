using System;
using System.Collections;
using System.Collections.Generic;
using QT.Core;
using QT.Core.Map;
using QT.Map;
using UnityEngine;

namespace QT.InGame
{
    public partial class Player
    {
        private bool isNextMap = false;
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Door"))
            {
                if (!_isEnterDoor)
                    return;
                switch (other.gameObject.name)
                {
                    case "Up":
                        _playerManager.PlayerDoorEnter.Invoke(Vector2Int.up);
                        break;
                    case "Down":
                        _playerManager.PlayerDoorEnter.Invoke(Vector2Int.down);
                        break;
                    case "Left":
                        _playerManager.PlayerDoorEnter.Invoke(Vector2Int.right);
                        break;
                    case "Right":
                        _playerManager.PlayerDoorEnter.Invoke(Vector2Int.left);
                        break;
                }
            }
            if (other.gameObject.layer == LayerMask.NameToLayer("StairCollider"))
            {
                if (isNextMap)
                    return;
                SystemManager.Instance.PlayerManager.StairNextRoomEvent.Invoke();
                isNextMap = true;
                OnMove = null;
                ClearAction(Player.ButtonActions.Swing);
                //_ownerEntity.ClearAction(Player.ButtonActions.Throw);
                ClearAction(Player.ButtonActions.Dodge);
                ClearAction(Player.ButtonActions.Teleport);
                ClearAction(Player.ButtonActions.Interaction);

                Rigidbody.velocity = Vector2.zero;
            }
        }
    }
}
