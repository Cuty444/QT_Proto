using System;
using System.Collections;
using System.Collections.Generic;
using QT.Core;
using QT.Enemy;
using UnityEngine;

using Enemy = QT.Enemy.Enemy;

namespace QT
{
    public class EnemyProjectileShooter : ProjectileShooter
    {
        private readonly int AttackAnimHash = Animator.StringToHash("Attack");
        public override LayerMask BounceMask => LayerMask.GetMask("Wall", "Player");

        private Enemy.Enemy _enemy;
        
        public void Initialize(Enemy.Enemy enemy)
        {
            _enemy = enemy;
        }

        public void PlayEnemyAtkSequence(int atkDataId, bool canOverlap = false)
        {
            var atkList = SystemManager.Instance.DataManager.GetDataBase<EnemyAtkGameDataBase>().GetData(atkDataId);  
            if (atkList == null)
            {
                return;
            }

            if (!canOverlap)
            {
                StopAllCoroutines();
            }

            StartCoroutine(AttackSequence(atkList));
        }

        private IEnumerator AttackSequence(List<EnemyAtkGameData> atkList)
        {
            foreach (var data in atkList)
            {
                yield return new WaitForSeconds(data.BeforeDelay);

                _enemy.Animator.SetTrigger(AttackAnimHash);
                Shoot(data.ShootDataId, data.AimType);
                
                yield return new WaitForSeconds(data.AfterDelay);
            }
        }
    }
}
