using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using QT.Core;
using QT.Sound;
using QT.Util;
using UnityEngine;

namespace QT.InGame
{
    [FSMState((int) Saddy.States.Struggle)]
    public class SaddyStruggleState : FSMState<Saddy>
    {
        private readonly int StruggleAnimHash = Animator.StringToHash("IsStruggle");
        
        private SoundManager _soundManager;
        
        private SaddyData _data;

        private float _timer;
        
        public SaddyStruggleState(IFSMEntity owner) : base(owner)
        {
            _data = _ownerEntity.SaddyData;
        }

        public override void InitializeState()
        {
            if (_ownerEntity.HP / _ownerEntity.HP.Value > _data.StruggleHPPer)
            {
                _ownerEntity.ChangeState(_ownerEntity.GetNextGroupStartState());
                return;
            }
            
            _soundManager = SystemManager.Instance.SoundManager;
            
            _ownerEntity.Rigidbody.velocity = Vector2.zero;
            _ownerEntity.Rigidbody.constraints = RigidbodyConstraints2D.FreezeAll;
            
            _ownerEntity.Animator.SetBool(StruggleAnimHash, true);

            _ownerEntity.Shooter.ShootPoint = _ownerEntity.ShootPointPivot;
            _ownerEntity.Shooter.PlayEnemyAtkSequence(_data.StruggleAtkId, ProjectileOwner.Boss);
            
            _timer = 0;
        }

        public override void UpdateState()
        {
            _timer += Time.deltaTime;

            if (_timer > _data.StruggleTime)
            {
                _ownerEntity.ChangeState(_ownerEntity.GetNextGroupStartState());
            }
        }

        public override void ClearState()
        {
            _ownerEntity.Shooter.ShootPoint = _ownerEntity.ShootPointTransform;
            
            _ownerEntity.Rigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
            _ownerEntity.Animator.SetBool(StruggleAnimHash, false);
        }

    }
}
