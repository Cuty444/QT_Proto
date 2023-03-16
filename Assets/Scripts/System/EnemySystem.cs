using System.Collections;
using System.Collections.Generic;
using QT.Core.Player;
using UnityEngine;

namespace QT.Core.Enemy
{
    public class EnemySystem : SystemBase
    {
        private Transform _playerTransform;
        public Transform PlayerTransform => _playerTransform;
        public override void OnInitialized()
        {
            SystemManager.Instance.GetSystem<PlayerSystem>().PlayerCreateEvent.AddListener(
                (player) => { _playerTransform = player.transform; });
        }
    }
}