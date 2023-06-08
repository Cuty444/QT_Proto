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

        private List<EnemyAtkGameData> _atkList;
        
        public DullahanAttackState(IFSMEntity owner) : base(owner)
        {
            _atkList = SystemManager.Instance.DataManager.GetDataBase<EnemyAtkGameDataBase>().GetData(_ownerEntity.DullahanData.AttackAtkId);
        }

        public override void InitializeState()
        {
            SetDir();
            
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
                _ownerEntity.Animator.SetBool(AttackAnimHash, true);
                yield return new WaitForSeconds(data.BeforeDelay);

                _ownerEntity.Shooter.Shoot(data.ShootDataId, data.AimType);
                
                _ownerEntity.Animator.SetBool(AttackAnimHash, false);
                yield return new WaitForSeconds(data.AfterDelay);
            }

            _ownerEntity.ChangeState(Dullahan.States.Normal);
        }
    }
}