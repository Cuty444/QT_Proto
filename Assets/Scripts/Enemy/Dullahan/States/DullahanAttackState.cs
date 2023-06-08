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
        private readonly int RotationAnimHash = Animator.StringToHash("Rotation");

        private const string SmashEffectPath = "Effect/Prefabs/FX_Boss_Smash.prefab";


        private List<EnemyAtkGameData> _atkList;
        
        public DullahanAttackState(IFSMEntity owner) : base(owner)
        {
            _atkList = SystemManager.Instance.DataManager.GetDataBase<EnemyAtkGameDataBase>().GetData(_ownerEntity.DullahanData.AttackAtkId);
        }

        public override void InitializeState()
        {
            _ownerEntity.Rigidbody.velocity = Vector2.zero;
            
            _ownerEntity.StartCoroutine(AttackSequence());
        }


        private void SetDir()
        {
            Vector2 dir = (SystemManager.Instance.PlayerManager.Player.transform.position -
                           _ownerEntity.transform.position);
            
            var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg + 90;

            if (angle < 0)
            {
                angle += 360;
            }

            if (angle > 180)
            {
                angle = 360 - angle;
            }

            int side = (int)Mathf.Round(angle / 180 * 2);
            
            _ownerEntity.Animator.SetFloat(RotationAnimHash, side);
            _ownerEntity.SetFlip(dir.x > 0);
            
            _ownerEntity.Shooter.ShootPoint = _ownerEntity.ShootPoints[side];
        }
        
        private IEnumerator AttackSequence()
        {
            foreach (var data in _atkList)
            {
                SetDir();
                
                _ownerEntity.Animator.SetTrigger(AttackAnimHash);
                yield return new WaitForSeconds(data.BeforeDelay);

                SystemManager.Instance.ResourceManager.EmitParticle(SmashEffectPath,
                    _ownerEntity.Shooter.ShootPoint.position);

                _ownerEntity.Shooter.Shoot(data.ShootDataId, data.AimType);
                
                yield return new WaitForSeconds(data.AfterDelay);
            }

            _ownerEntity.ChangeState(Dullahan.States.Normal);
        }
    }
}