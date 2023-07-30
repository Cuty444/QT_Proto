using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace QT.InGame
{
    // Param1 : 텔레포트 허용 범위
    public class TeleportItemEffect : ItemEffect
    {
        public TeleportItemEffect(Player player, ItemEffectGameData effectData, SpecialEffectGameData specialEffectData) : base(player, effectData, specialEffectData)
        {
        }

        public override void OnEquip()
        {
            _lastTime = 0;
        }

        protected override void OnTriggerAction()
        {
        }

        public override void OnRemoved()
        {
        }
        
        
        // protected virtual void OnTeleport(bool isOn)
        // {
        //     if (isOn && !_ownerEntity.RigidEnemyCheck())
        //     {
        //         _ownerEntity.ChangeState(Player.States.Teleport);
        //     }
        // }
        
        
        // public bool RigidEnemyCheck()
        // {
        //     foreach (var hitable in _hitableList)
        //     {
        //         if (hitable is not Enemy)
        //         {
        //             continue;
        //         }
        //
        //         var enemy = (Enemy) hitable;
        //         
        //         if (enemy.CurrentStateIndex == (int) Enemy.States.Rigid && enemy.HP <= 0)
        //         {
        //             if (StatComponent.GetStat(PlayerStats.TeleportAllowableDistance) <
        //                 Vector2.Distance(transform.position, enemy.transform.position))
        //             {
        //                 continue;
        //             }
        //             _rigidTeleportEnemy = enemy;
        //             _rigidTargetEnemy = null;
        //             float lowHp = float.MaxValue;
        //             foreach (var targetHitable in _hitableList)
        //             {
        //                 if (targetHitable is not Enemy)
        //                 {
        //                     continue;
        //                 }
        //
        //                 var targetEnemy = (Enemy) targetHitable;
        //                 
        //                 if (targetEnemy.CurrentStateIndex < (int) Enemy.States.Projectile)
        //                 {
        //                     if (targetEnemy.HP > 0)
        //                     {
        //                         float percentageHp = targetEnemy.HP / targetEnemy.HP.BaseValue;
        //                         if (lowHp > percentageHp)
        //                         {
        //                             lowHp = percentageHp;
        //                             _rigidTargetEnemy = targetEnemy;
        //                         }
        //                     }
        //                 }
        //             }
        //
        //             if (_rigidTargetEnemy != null)
        //             {
        //                 return false;
        //             }
        //             return true;
        //         }
        //     }
        //
        //     return true;
        // }
    }
}
