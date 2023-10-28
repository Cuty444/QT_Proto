using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using QT.Core;
using QT.Sound;

namespace QT.InGame
{
    [FSMState((int) Saddy.States.RollingAttack)]
    public class SaddyRollingAttackState : FSMState<Saddy>
    {
        private static readonly int MoveSpeedAnimHash = Animator.StringToHash("MoveSpeed");
        private static readonly int IsDodgeAnimHash = Animator.StringToHash("IsDodge");
        private static readonly int ChargeLevelAnimHash = Animator.StringToHash("SwingLevel");

        private SaddyData _data;
        
        private SoundManager _soundManager;

        private Transform _targetTransform;
        private Transform _transform;
        
        private Vector2 _dir;
        private float _timer;
        
        private float _targetDistance;
        
        public SaddyRollingAttackState(IFSMEntity owner) : base(owner)
        {
            _data = _ownerEntity.SaddyData;
            _transform = _ownerEntity.transform;
        }

        public override void InitializeState()
        {
            _soundManager = SystemManager.Instance.SoundManager;
            _targetTransform = SystemManager.Instance.PlayerManager.Player.transform;

            _ownerEntity.Animator.SetBool(IsDodgeAnimHash, true);
            
            _dir = (_targetTransform.position - _transform.position).normalized;
            _timer = 0;
            
            _targetDistance =  _data.RollingAttackAtkDistance * _data.RollingAttackAtkDistance;
        }

        public override void UpdateState()
        {
            _timer += Time.deltaTime;
            
            Vector2 targetDir = (_targetTransform.position - _transform.position);
            
            _dir = Vector3.RotateTowards(_dir, targetDir.normalized, _data.RollingAttackSteerAngle * Mathf.Deg2Rad * Time.deltaTime, 0);
            
            
            if (_timer > _data.RollingAttackLengthTime || 
                (_timer > _data.RollingAttackDelay && targetDir.sqrMagnitude < _targetDistance))
            {
                _ownerEntity.SetDir(_dir,4);
                
                _ownerEntity.Animator.SetInteger(ChargeLevelAnimHash, 1);
                _ownerEntity.Shooter.PlayEnemyAtkSequence(_data.RollingAttackAtkId, ProjectileOwner.Boss);
                
                _ownerEntity.ChangeState(Dullahan.States.Normal);
            }
            else
            {
                _ownerEntity.SetDir(_dir,2);
            }
        }

        public override void FixedUpdateState()
        {
            var progress = _timer / _data.RollingAttackLengthTime;
            progress *= progress; // easeInQuad
            
            var speed = Mathf.Lerp(_data.RollingAttackStartSpeed, _data.RollingAttackEndSpeed, progress);
            
            _ownerEntity.Rigidbody.velocity = _dir * speed;
            
            _ownerEntity.Animator.SetFloat(MoveSpeedAnimHash, 1 + progress);

            if (_timer > _data.RollingAttackDelay && CheckHit(speed))
            {
                _timer = _data.RollingAttackLengthTime;
            }
        }

        private bool CheckHit(float speed)
        {
            var hits = Physics2D.CircleCastAll(_transform.position, _ownerEntity.ColliderRad, _dir,
                speed * Time.deltaTime,
                _ownerEntity.HitMask);

#if UNITY_EDITOR
            Debug.DrawRay(_transform.position, _dir * (_ownerEntity.ColliderRad + speed * Time.deltaTime), Color.magenta, 1);
            Debug.DrawRay(_transform.position, new Vector3(-_dir.y, _dir.x) * (_ownerEntity.ColliderRad), Color.magenta, 1);
            Debug.DrawRay(_transform.position, new Vector3(_dir.y, -_dir.x) * (_ownerEntity.ColliderRad), Color.magenta, 1);
#endif

            return hits.Length > 0;
        }

        public override void ClearState()
        {
            _ownerEntity.Animator.SetBool(IsDodgeAnimHash, false);
        }
        
    }
}