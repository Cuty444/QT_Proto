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

        public override void OnTrigger(bool success)
        {
            if (success)
            {
                _shooter.Shoot(_shootDataId, _aimType, ProjectileOwner.Player);
            }
        }

        public override void OnRemoved()
        {
        }
    }
}
