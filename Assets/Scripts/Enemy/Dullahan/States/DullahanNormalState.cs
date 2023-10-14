using System;
using System.Collections.Generic;
using System.Timers;
using QT.Core;
using UnityEngine;
using System.Linq;
using QT.Util;
using Math = QT.Util.Math;

namespace QT.InGame
{
    [FSMState((int) Dullahan.States.Normal)]
    public class DullahanNormalState : FSMState<Dullahan>
    {
        private static readonly int MoveSpeedAnimHash = Animator.StringToHash("MoveSpeed");
        
        private const float AvoidDirDampTime = 30;
        private const float TurnoverLimitSpeed = 0.75f * 0.75f;

        private readonly EnemyGameData _enemyData;
        private readonly DullahanData _dullahanData;

        private float _targetUpdateCoolTime;
        private Vector2 _moveTarget;

        private float _atkCoolTime;

        private bool _rotateSide;
        
        private InputVector2Damper _dirDamper = new ();
        private InputVector2Damper _avoidDirDamper = new (AvoidDirDampTime);

        private List<Dullahan.States> _attackStates = new (){Dullahan.States.Rush, Dullahan.States.Attack, Dullahan.States.Throw};
        private int _attackStateIndex;

        private float _targetDistance;

        public DullahanNormalState(IFSMEntity owner) : base(owner)
        {
            _enemyData = _ownerEntity.Data;
            _dullahanData = _ownerEntity.DullahanData;

            _attackStates.Shuffle();
            _attackStateIndex = 0;
            _targetDistance = GetStateTargetDistance(_attackStates[_attackStateIndex]);
        }

        public override void InitializeState()
        {
            _targetUpdateCoolTime = 0;
            _ownerEntity.Shooter.SetTarget(SystemManager.Instance.PlayerManager.Player.transform);
        }

        public override void UpdateState()
        {
            _atkCoolTime += Time.deltaTime;
            _targetUpdateCoolTime += Time.deltaTime;
            
            if (_targetUpdateCoolTime > _enemyData.MoveTargetUpdatePeroid)
            {
                _targetUpdateCoolTime = 0;
                _moveTarget = SystemManager.Instance.PlayerManager.Player.transform.position;
            }
        }

        public override void FixedUpdateState()
        {
            var targetDistance = (_moveTarget - (Vector2) _ownerEntity.transform.position).magnitude;

            var speed = _ownerEntity.Rigidbody.velocity.sqrMagnitude /
                        (_enemyData.MovementSpd * _enemyData.MovementSpd);
            _ownerEntity.Animator.SetFloat(MoveSpeedAnimHash, speed);
            
            Move(targetDistance);
            CheckAttackStart(targetDistance);
        }

        public override void ClearState()
        {
        }

        private void Move(float targetDistance)
        {
            var dir = Vector2.zero;

            switch (_enemyData.MoveType)
            {
                case EnemyGameData.MoveTypes.Spacing:
                    dir = SpacingMove(targetDistance, true);
                    break;
                case EnemyGameData.MoveTypes.SpacingLeft:
                    dir = SpacingMove(targetDistance, true);
                    break;
            }

            var currentDir = Vector2.zero;

            if (dir != Vector2.zero)
            {
                currentDir = _ownerEntity.Rigidbody.velocity.normalized;
                currentDir = Vector2.Lerp(currentDir, dir, 0.4f);
            }

            _ownerEntity.Rigidbody.velocity = currentDir * (_enemyData.MovementSpd);

            _ownerEntity.SetDir(_dirDamper.GetDampedValue(currentDir, Time.deltaTime), 4);
        }
        
        
        private Vector2 SpacingMove(float targetDistance, bool isRotate = false)
        {
            var ownerPos = (Vector2) _ownerEntity.transform.position;
            var dir = _moveTarget - ownerPos;
            
            var danger = new DirectionWeights();
            var interest = new DirectionWeights();
            
            _ownerEntity.Steering.DetectObstacle(ref danger);

            // 타겟과의 거리 유지
            if (targetDistance > _targetDistance)
            {
                interest.AddWeight(dir, 1);
            }
            else if(targetDistance < _targetDistance - 1)
            {
                interest.AddWeight(-dir, 1);
            }

            // 타겟 주위 회전
            if (isRotate)
            {
                interest.AddWeight(Math.Rotate90Degree(dir, _rotateSide), 1);
            }

            // 1차 결과 계산
            var result = _ownerEntity.Steering.CalculateContexts(danger, interest);

            
            // 1차 결과 계산 후 장애물 때문에 속도가 너무 느리면 반대로 회전 및 해당 방향 피하기 (회전하지 않는 경우 interest가 없으면 회피 계산 무시)
            if (danger.AddedWeightCount > 0 && (isRotate || interest.AddedWeightCount > 0))
            {
                if (result.sqrMagnitude < TurnoverLimitSpeed)
                {
                    _rotateSide = !_rotateSide;

                    if (isRotate)
                    {
                        _avoidDirDamper.ResetCurrentValue(-result);
                    }
                    else
                    {
                        _avoidDirDamper.ResetCurrentValue((-result + Math.Rotate90Degree(dir, _rotateSide)).normalized);
                    }
                }
            }

            // 회피 방향 적용해 2차 결과 계산
            var avoidDir = _avoidDirDamper.GetDampedValue(Vector2.zero, Time.deltaTime);
            if (avoidDir != Vector2.zero)
            {
                interest.AddWeight(_avoidDirDamper.GetDampedValue(avoidDir, Time.deltaTime), 1);
                result = _ownerEntity.Steering.CalculateContexts(danger, interest);
            }

            return result.normalized;
        }

        private void CheckAttackStart(float targetDistance)
        {
            if (_atkCoolTime < _enemyData.AtkCheckDelay)
            {
                return;
            }

            if (targetDistance >= _dullahanData.JumpDistance)
            {
                _atkCoolTime = _enemyData.AtkCheckDelay * 0.5f;
                _ownerEntity.ChangeState(Dullahan.States.Jump);
                return;
            }
            
            _atkCoolTime = 0;

            _ownerEntity.ChangeState(PickAttackState());
        }

        private Dullahan.States PickAttackState()
        {
            _attackStateIndex++;
            
            if(_attackStateIndex < _attackStates.Count)
            {
                return _attackStates[_attackStateIndex];
            }
            
            _attackStates.Shuffle();
            _attackStateIndex = 0;

            _targetDistance = GetStateTargetDistance(_attackStates[_attackStateIndex]);

            return _attackStates[_attackStateIndex];
        }
        
        private float GetStateTargetDistance(Dullahan.States states)
        {
            switch (states)
            {
                case Dullahan.States.Rush:
                    return _dullahanData.RushDistance;
                case Dullahan.States.Attack:
                    return _dullahanData.AttackDistance;
                case Dullahan.States.Throw:
                    return _dullahanData.ThrowDistance;
            }

            return _enemyData.SpacingRad;
        }
    }
}