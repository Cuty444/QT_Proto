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
        private bool _useAnimator;
        
        public void Initialize(Animator animator)
        {
            StopAllCoroutines();
            _animator = animator;
            _useAnimator = animator != null;
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
            if (_useAnimator)
            {
                _animator.ResetTrigger(AttackAnimHash);
                _animator.SetBool(IsAttackAnimHash, true);
            }

            IsAttacking = true;

            foreach (var data in atkList)
            {
                if (data.BeforeDelay != 0)
                {
                    yield return new WaitForSeconds(data.BeforeDelay);
                }

                if (_useAnimator)
                {
                    _animator.SetTrigger(AttackAnimHash);
                }

                Shoot(data.ShootDataId, data.AimType, owner);

                if (data.AfterDelay != 0)
                {
                    yield return new WaitForSeconds(data.AfterDelay);
                }

                if (_useAnimator)
                {
                    _animator.SetTrigger(AttackAnimHash);
                }
            }

            if (_useAnimator)
            {
                _animator.ResetTrigger(AttackAnimHash);
                _animator.SetBool(IsAttackAnimHash, false);
            }

            IsAttacking = false;
        }
        
        public void StopAttack()
        {
            StopAllCoroutines();
            
            if (_useAnimator)
            {
                _animator.ResetTrigger(AttackAnimHash);
                _animator.SetBool(IsAttackAnimHash, false);
            }
            IsAttacking = false;
        }
    }
}
