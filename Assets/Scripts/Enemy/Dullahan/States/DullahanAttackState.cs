using System;
using System.Collections;
using System.Collections.Generic;
using QT.Core;
using UnityEngine;

namespace QT.InGame
{
    [FSMState((int) Dullahan.States.Attack)]
    public class DullahanAttackState : FSMState<Dullahan>
    {
        private readonly int AttackAnimHash = Animator.StringToHash("Attack");

        private const string SmashEffectPath = "Effect/Prefabs/FX_Boss_Smash.prefab";


        private List<EnemyAtkGameData> _atkList;

        private Coroutine _atkSeqence;
        
        public DullahanAttackState(IFSMEntity owner) : base(owner)
        {
            _atkList = SystemManager.Instance.DataManager.GetDataBase<EnemyAtkGameDataBase>().GetData(_ownerEntity.DullahanData.AttackAtkId);
        }

        public override void InitializeState()
        {
            _ownerEntity.Rigidbody.velocity = Vector2.zero;
            _ownerEntity.Rigidbody.constraints = RigidbodyConstraints2D.FreezeAll;
            
            _atkSeqence = _ownerEntity.StartCoroutine(AttackSequence());
        }
        
        public override void ClearState()
        {
            _ownerEntity.Rigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
            _ownerEntity.StopCoroutine(_atkSeqence);
        }
        
        private IEnumerator AttackSequence()
        {
            foreach (var data in _atkList)
            {  
                Vector2 dir = (SystemManager.Instance.PlayerManager.Player.transform.position - _ownerEntity.transform.position);

                var side = _ownerEntity.SetDir(dir,2);
                _ownerEntity.Shooter.ShootPoint = _ownerEntity.ShootPoints[side];

                if (data.BeforeDelay > 0)
                {
                    _ownerEntity.Animator.SetTrigger(AttackAnimHash);
                    yield return new WaitForSeconds(data.BeforeDelay);

                SystemManager.Instance.ResourceManager.EmitParticle(SmashEffectPath,
                    _ownerEntity.Shooter.ShootPoint.position);
                }
                
                _ownerEntity.Shooter.Shoot(data.ShootDataId, data.AimType, ProjectileOwner.Boss);

                if (data.AfterDelay > 0)
                {
                    yield return new WaitForSeconds(data.AfterDelay);
                }
            }

            _ownerEntity.Animator.ResetTrigger(AttackAnimHash);
            
            _ownerEntity.ChangeState(Dullahan.States.Normal);
        }
    }
}