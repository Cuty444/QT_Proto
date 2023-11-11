using System.Collections.Generic;
using System.Timers;
using QT.Core;
using QT.Util;
using UnityEngine;

namespace QT.InGame
{
    [FSMState((int) JelloLeftHand.States.Normal)]
    public class JelloLeftHandNormalState : FSMState<JelloLeftHand>
    {
        private const float AvoidDirDampTime = 30;
        private const float TurnoverLimitSpeed = 0.75f * 0.75f;

        private readonly EnemyGameData _enemyData;
        private readonly JelloHandData _data;

        private Transform _transform;
        
        private Transform _target;
        private Transform _JelloTransform;
        private Vector2 _currentTargetPos;

        private float _atkCoolTime;

        private bool _rotateSide;
        
        private InputVector2Damper _avoidDirDamper = new (AvoidDirDampTime);

        private float _targetDistance;
        
        public JelloLeftHandNormalState(IFSMEntity owner) : base(owner)
        {
            _transform = _ownerEntity.transform;
            _enemyData = _ownerEntity.Data;
            _data = _ownerEntity.JelloData;
        }

        public override void InitializeState()
        {
            _JelloTransform = _ownerEntity.Jello.transform;
            _currentTargetPos = _JelloTransform.position;
            
            _target = SystemManager.Instance.PlayerManager.Player.transform;
            _targetDistance = _enemyData.SpacingRad;
            
            _ownerEntity.Shooter.SetTarget(_target);
        }

        public override void UpdateState()
        {
            _atkCoolTime += Time.deltaTime;
            _currentTargetPos = _JelloTransform.position;
        }

        public override void FixedUpdateState()
        {
            Move();
            
            var targetDir = (Vector2) _target.position - (Vector2) _transform.position;
            _ownerEntity.SetDir(targetDir, 4);
            
            CheckAttackStart();
        }

        private Vector2 Move()
        {
            var distance = (_currentTargetPos - (Vector2) _transform.position).magnitude;
            var dir = SpacingMove(distance);

            var currentDir = Vector2.zero;
            var speed = _enemyData.MovementSpd;

            // 타겟과의 거리 유지
            if (distance - _targetDistance > 1)
            {
                speed *= _data.PositionCorrectionSpeedMultiplier;
            }

            if (dir != Vector2.zero)
            {
                currentDir = _ownerEntity.Rigidbody.velocity.normalized;
                currentDir = Vector2.Lerp(currentDir, dir, 0.4f);
            }
            
            _ownerEntity.Rigidbody.velocity = currentDir * speed;

            return currentDir;
        }
        
        
        private Vector2 SpacingMove(float targetDistance)
        {
            var ownerPos = (Vector2)_transform.position;
            
            var dir = _currentTargetPos - ownerPos;
            
            var danger = new DirectionWeights();
            var interest = new DirectionWeights();
            
            _ownerEntity.Steering.DetectObstacle(ref danger);
            
            // 타겟과의 거리 유지
            if (targetDistance > _targetDistance)
            {
                interest.AddWeight(dir, 1);
            }
            else if (targetDistance < _targetDistance - 1)
            {
                interest.AddWeight(-dir, 1);
            }
            
            interest.AddWeight(Math.Rotate90Degree(dir, _rotateSide), 1);
            
                // 1차 결과 계산
            var result = _ownerEntity.Steering.CalculateContexts(danger, interest);
            
            // 장애물이 있으면 회전
            if (danger.AddedWeightCount > 0)
            {
                if (result.sqrMagnitude < TurnoverLimitSpeed)
                {
                    _rotateSide = !_rotateSide;
                    _avoidDirDamper.ResetCurrentValue(-result);
                }
            }
            
            
            // 회피 방향 적용해 2차 결과 계산
            var avoidDir = _avoidDirDamper.GetDampedValue(Vector2.zero, Time.deltaTime);
            if (avoidDir != Vector2.zero)
            {
                interest.AddWeight(avoidDir, 1);
                result = _ownerEntity.Steering.CalculateContexts(danger, interest);
            }

            return result.normalized;
        }
        
        
        private void CheckAttackStart()
        {
            if (_atkCoolTime > _data.AttackCoolTime)
            {
                _atkCoolTime = 0;
                _ownerEntity.Shooter.PlayEnemyAtkSequence(_data.ShootAtkId,_ownerEntity.Owner);
            }
        }
    }
}