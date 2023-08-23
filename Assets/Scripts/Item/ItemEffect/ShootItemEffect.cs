using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using QT.Core;
using UnityEngine;

namespace QT.InGame
{
    // Param1: 발사할 ShootIDataID
    // Param2: 에임 타입 (0 = World, 1 = Target, 2 = MoveDirection)
    public class ShootItemEffect : ItemEffect
    {
        private int _shootDataId;
        private AimTypes _aimType;
        
        private ProjectileShooter _shooter;
        
        public ShootItemEffect(Player player, ItemEffectGameData effectData, SpecialEffectGameData specialEffectData) : base(player, effectData, specialEffectData)
        {
            _shootDataId = (int)specialEffectData.Param1;
            _aimType = (AimTypes)specialEffectData.Param2;

            _shooter = player.ProjectileShooter;
        }

        public override void OnEquip()
        {
        }

        protected override void OnTriggerAction()
        {
            // TODO : 뭔가 간헐적으로 플레이어가 사라지는 현상이 있음
            SystemManager.Instance.PlayerManager.Player.ProjectileShooter.Shoot(_shootDataId, _aimType, ProjectileOwner.Player);
            // _shooter.Shoot(_shootDataId, _aimType, ProjectileOwner.Player);
        }

        public override void OnRemoved()
        {
        }
    }
}
