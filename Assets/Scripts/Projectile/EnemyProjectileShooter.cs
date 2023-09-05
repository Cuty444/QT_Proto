using System;
using System.Collections;
using System.Collections.Generic;
using QT.Core;
using UnityEngine;


namespace QT.InGame
{
    public class EnemyProjectileShooter : ProjectileShooter
    {
        private readonly int IsAttackAnimHash = Animator.StringToHash("IsAttack");
        private readonly int AttackAnimHash = Animator.StringToHash("Attack");
        
        public override LayerMask BounceMask => LayerMask.GetMask("Wall","HardCollider","ProjectileCollider", "Player","InteractionCollider");
        public override ProjectileOwner Owner => ProjectileOwner.Enemy;

        public bool IsAttacking { get;private set; }

        private Animator _animator;
        
        public void Initialize(Animator animator)
        {
            _animator = animator;
        }

        public void PlayEnemyAtkSequence(int atkDataId,ProjectileOwner owner, bool canOverlap = false)
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

            StartCoroutine(AttackSequence(atkList,owner));
        }

        private IEnumerator AttackSequence(List<EnemyAtkGameData> atkList,ProjectileOwner owner)
        {
            _animator?.ResetTrigger(AttackAnimHash);
            _animator?.SetBool(IsAttackAnimHash, true);
            IsAttacking = true;
            
            foreach (var data in atkList)
            {
                yield return new WaitForSeconds(data.BeforeDelay);

                _animator?.SetTrigger(AttackAnimHash);
                Shoot(data.ShootDataId, data.AimType,owner);
                
                yield return new WaitForSeconds(data.AfterDelay);
                
                _animator?.SetTrigger(AttackAnimHash);
            }
            
            _animator?.ResetTrigger(AttackAnimHash);
            _animator?.SetBool(IsAttackAnimHash, false);
            IsAttacking = false;
        }
        
        public void StopAttack()
        {
            StopAllCoroutines();
            _animator?.ResetTrigger(AttackAnimHash);
            _animator?.SetBool(IsAttackAnimHash, false);
            IsAttacking = false;
        }
    }
}
