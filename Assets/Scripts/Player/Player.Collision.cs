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
        private LayerMask FallLayerMask => LayerMask.GetMask("Fall");
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
            else if (other.gameObject.layer == LayerMask.NameToLayer("StairCollider"))
            {
                if (isNextMap)
                    return;
                SystemManager.Instance.PlayerManager.StairNextRoomEvent.Invoke();
                SystemManager.Instance.RankingManager.PlayerOn.Invoke(false);
                isNextMap = true;
                OnMove = null;
                ClearAction(ButtonActions.Swing);
                ClearAction(ButtonActions.Dodge);
                ClearAction(ButtonActions.Interaction);
                SystemManager.Instance.SoundManager.PlayOneShot(SystemManager.Instance.SoundManager.SoundData.Player_Walk_StairSFX);
                //Rigidbody.velocity = Vector2.zero;
            }
            else if(other.gameObject.layer == LayerMask.NameToLayer("GardenCollider"))
            {
                IsGarden = true;
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if(other.gameObject.layer == LayerMask.NameToLayer("GardenCollider"))
            {
                IsGarden = false;
            }
        }

        public bool CheckFall()
        {
            var collider = Physics2D.OverlapPoint(transform.position, FallLayerMask);
            return collider != null;
        }
    }
}
