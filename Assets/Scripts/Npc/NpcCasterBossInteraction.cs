using System;
using System.Collections;
using System.Collections.Generic;
using QT.Core;
using QT.Core.Map;
using UnityEngine;

namespace QT
{
    public class NpcCasterBossInteraction : MonoBehaviour, IHitAble
    {
        public int InstanceId => gameObject.GetInstanceID();
        public Vector2 Position => transform.position;
        [field: SerializeField] public float ColliderRad { get; private set; }
        public bool IsClearTarget => false;
        public bool IsDead => false;

        private PlayerManager _playerManager;

        private void Start()
        {
            _playerManager = SystemManager.Instance.PlayerManager;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Player") ||
                other.gameObject.layer == LayerMask.NameToLayer("PlayerDodge"))
            {
                _playerManager.PlayerItemInteraction.AddListener(MoveStairMap);
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Player") ||
                other.gameObject.layer == LayerMask.NameToLayer("PlayerDodge"))
            {
                _playerManager.PlayerItemInteraction.RemoveListener(MoveStairMap);
            }
        }

        private void MoveStairMap()
        {
            _playerManager.Player.transform.position = SystemManager.Instance.GetSystem<DungeonMapSystem>()._stairRoomEnterTransform.position;
            _playerManager.Player.LastSafePosition = SystemManager.Instance.GetSystem<DungeonMapSystem>()._stairRoomEnterTransform.position;
        }
        
        public void Hit(Vector2 dir, float power,AttackType attackType)
        {
        }
    }
}
