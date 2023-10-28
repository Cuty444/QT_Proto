using System.Collections;
using System.Collections.Generic;
using QT.Core;
using QT.Sound;
using UnityEngine;

namespace QT.InGame
{
    [FSMState((int) Saddy.States.RollingAttack)]
    public class SaddyRollingAttackState : FSMState<Saddy>
    {
        private static readonly int IsDodgeAnimHash = Animator.StringToHash("IsDodge");
        private static readonly int ChargeLevelAnimHash = Animator.StringToHash("SwingLevel");

        private SaddyData _data;
        
        private SoundManager _soundManager;

        private Transform _targetTransform;
        private Transform _transform;

        private float _timer;
        
        public SaddyRollingAttackState(IFSMEntity owner) : base(owner)
        {
            _data = _ownerEntity.SaddyData;
            _transform = _ownerEntity.transform;
        }

        public override void InitializeState()
        {
            _soundManager = SystemManager.Instance.SoundManager;
            _targetTransform = SystemManager.Instance.PlayerManager.Player.transform;

            _ownerEntity.Shooter.ShootPoint = _ownerEntity.BatTransform;
            
            _ownerEntity.Animator.SetBool(IsDodgeAnimHash, true);
            
            _timer = 0;
        }

        public override void UpdateState()
        {
            _timer += Time.deltaTime;
            
            Vector2 dir = (_targetTransform.position - _transform.position);
            _ownerEntity.SetDir(dir,2);

            var checkDistance = _data.RollingAttackAtkDistance * _data.RollingAttackAtkDistance;
            
            if (_timer > _data.RollingAttackLengthTime || dir.sqrMagnitude < checkDistance)
            {
                _ownerEntity.Animator.SetInteger(ChargeLevelAnimHash, 1);
                _ownerEntity.Shooter.PlayEnemyAtkSequence(_data.RollingAttackAtkId, ProjectileOwner.Boss);
                
                _ownerEntity.ChangeState(Dullahan.States.Normal);
            }
        }

        public override void FixedUpdateState()
        {
            Vector2 dir = (_targetTransform.position - _transform.position).normalized;
            _ownerEntity.Rigidbody.velocity = dir * _data.RollingAttackSpeed;
        }

        public override void ClearState()
        {
            _ownerEntity.Animator.SetBool(IsDodgeAnimHash, false);
        }

    }
}