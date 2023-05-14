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
            
        }
    }
}
