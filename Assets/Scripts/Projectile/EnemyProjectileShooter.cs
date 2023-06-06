using System;
using System.Collections;
using System.Collections.Generic;
using QT.Core;
using QT.InGame;
using UnityEngine;


namespace QT
{
    public class EnemyProjectileShooter : ProjectileShooter
    {
        private readonly int AttackAnimHash = Animator.StringToHash("Attack");
        public override LayerMask BounceMask => LayerMask.GetMask("Wall","HardCollider","ProjectileCollider", "Player");
        public override ProjectileOwner Owner => ProjectileOwner.Enemy;

        private Enemy _enemy;
        
        public void Initialize(Enemy enemy)
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
            _enemy.Animator.SetBool(AttackAnimHash, true);
            
            foreach (var data in atkList)
            {
                yield return new WaitForSeconds(data.BeforeDelay);

                Shoot(data.ShootDataId, data.AimType);
                
                yield return new WaitForSeconds(data.AfterDelay);
            }
            
            _enemy?.Animator.SetBool(AttackAnimHash, false);
        }
    }
}
