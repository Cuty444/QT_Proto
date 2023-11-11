using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using QT.Core;
using QT.Util;
using UnityEngine;

namespace QT.InGame
{
    [FSMState((int) Jello.States.Normal)]
    public class JelloNormalState : FSMState<Jello>
    { 
        private static readonly int IsMoveAnimHash = Animator.StringToHash("IsMove");
        private static readonly int MoveDirAnimHash = Animator.StringToHash("MoveDir");
        private static readonly int MoveSpeedAnimHash = Animator.StringToHash("MoveSpeed");
        
        private const float AvoidDirDampTime = 30;
        private const float TurnoverLimitSpeed = 0.75f * 0.75f;

        private readonly EnemyGameData _enemyData;
        private readonly JelloData _data;

        private Transform _transform;
        
        private float _targetUpdateCoolTime;
        public Transform _target;
        private Vector2 _currentTargetPos;
        private Vector2 _centerPos;

        private float _atkCoolTime;

        private bool _rotateSide;
        
        private InputVector2Damper _dirDamper = new ();
        private InputVector2Damper _avoidDirDamper = new (AvoidDirDampTime);

        private List<Jello.States> _attackStates = new() {Jello.States.Shoot, Jello.States.Rush};
        private int _attackStateIndex;

        private Jello.States _nextState;
        private float _targetDistance;
        
        public JelloNormalState(IFSMEntity owner) : base(owner)
        {
            _transform = _ownerEntity.transform;
            _enemyData = _ownerEntity.Data;
            _data = _ownerEntity.JelloData;

            _attackStates.Shuffle();
            _attackStateIndex = -1;
        }

        public override void InitializeState()
        {
            _target = SystemManager.Instance.PlayerManager.Player.transform;
            _currentTargetPos = _target.position;
            _centerPos = _ownerEntity.MapData.MapCenter.position;
            
            _ownerEntity.Shooter.SetTarget(_target);
            _ownerEntity.Animator.SetBool(IsMoveAnimHash, true);

            _nextState = PickAttackState();
        }

        public override void UpdateState()
        {
            _atkCoolTime += Time.deltaTime;
            _targetUpdateCoolTime += Time.deltaTime;
            
            if (_targetUpdateCoolTime > _enemyData.MoveTargetUpdatePeroid)
            {
                _targetUpdateCoolTime = 0;
                _currentTargetPos = _target.position;
            }
        }

        public override void FixedUpdateState()
        {
            var currentDir = Move();
            
            var dampedDir = _dirDamper.GetDampedValue(currentDir, Time.deltaTime);
            
            var speed = dampedDir.sqrMagnitude;
            _ownerEntity.Animator.SetFloat(MoveSpeedAnimHash, speed);
            _ownerEntity.Animator.SetBool(IsMoveAnimHash, speed > 0.1f);
            
            
            var targetDir = (Vector2) _target.position - (Vector2) _transform.position;
            var moveDir = Vector2.Dot(targetDir, dampedDir) <= 0 ? -1f : 1f;
            
            _ownerEntity.SetDir(dampedDir * moveDir, 4);
            _ownerEntity.Animator.SetFloat(MoveDirAnimHash, moveDir * speed);
            
            CheckAttackStart();
        }

        private Vector2 Move()
        {
            var dir = SpacingMove();

            var currentDir = Vector2.zero;
            var speed = _enemyData.MovementSpd;

            if (dir != Vector2.zero)
            {
                currentDir = _ownerEntity.Rigidbody.velocity.normalized;
                currentDir = Vector2.Lerp(currentDir, dir, 0.4f);
            }
            
            // 발사 패턴에서는 플레이어 위쪽으로 이동할 필요가 있음
            if (_nextState == Jello.States.Shoot && _transform.position.y < _currentTargetPos.y + _data.ShootDistance)
            {
                speed *= _data.ShootPositionCorrectionSpeedMultiplier;
            }
            
            _ownerEntity.Rigidbody.velocity = currentDir * speed;

            return currentDir;
        }
        
        
        private Vector2 SpacingMove()
        {
            var ownerPos = (Vector2)_transform.position;
            
            var targetDistance = (_currentTargetPos - ownerPos).magnitude;
            var dir = _currentTargetPos - ownerPos;
            
            var danger = new DirectionWeights();
            var interest = new DirectionWeights();
            
            _ownerEntity.Steering.DetectObstacle(ref danger);
            
            // 발사 패턴에서는 플레이어 위쪽으로 이동할 필요가 있음
            if (_nextState == Jello.States.Shoot && ownerPos.y < _currentTargetPos.y + _data.ShootDistance)
            {
                interest.AddWeight(Vector2.up, 1);
            }
            else
            {
                // 타겟과의 거리 유지
                if (targetDistance > _targetDistance)
                {
                    interest.AddWeight(dir, 1);
                    interest.AddWeight(_centerPos - ownerPos, 1);
                }
                else if(targetDistance < _targetDistance - 1)
                {
                    interest.AddWeight(-dir, 1);
                    interest.AddWeight(_centerPos - ownerPos, 1);
                }
            }
            
            
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
                
                interest.AddWeight(Math.Rotate90Degree(dir, _rotateSide), 1);
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
                _ownerEntity.ChangeState(_nextState);
            }
        }

        private Jello.States PickAttackState()
        {
            _attackStateIndex++;
            
            if(_attackStateIndex < _attackStates.Count)
            {
                _targetDistance = GetStateTargetDistance(_attackStates[_attackStateIndex]);
                return _attackStates[_attackStateIndex];
            }
            
            _attackStates.Shuffle();
            _attackStateIndex = 0;
            
            _targetDistance = GetStateTargetDistance(_attackStates[_attackStateIndex]);
            return _attackStates[_attackStateIndex];
        }
        
        private float GetStateTargetDistance(Jello.States states)
        {
            switch (states)
            {
                case Jello.States.Rush:
                    return _data.RushDistance;
                case Jello.States.Shoot:
                    return _data.ShootDistance;
            }

            return _enemyData.SpacingRad;
        }

    }
}
