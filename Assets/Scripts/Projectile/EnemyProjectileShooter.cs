using System;
using System.Collections;
using System.Collections.Generic;
using QT.Core;
using UnityEngine;


namespace QT.InGame
{
    public class EnemyProjectileShooter : ProjectileShooter
    {
        private readonly int AttackAnimHash = Animator.StringToHash("Attack");
        public override LayerMask BounceMask => LayerMask.GetMask("Wall","HardCollider","ProjectileCollider", "Player");
        public override ProjectileOwner Owner => ProjectileOwner.Enemy;

        private Animator _animator;
        
        public void Initialize(Animator animator)
        {
            _animator = animator;
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
            _animator.SetBool(AttackAnimHash, true);
            
            foreach (var data in atkList)
            {
                yield return new WaitForSeconds(data.BeforeDelay);

                Shoot(data.ShootDataId, data.AimType);
                
                yield return new WaitForSeconds(data.AfterDelay);
            }
            
            _animator.SetBool(AttackAnimHash, false);
        }
    }
}
