using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using QT.Core;
using QT.Util;
using UnityEngine;

namespace QT.InGame
{
    [FSMState((int) Saddy.States.Normal)]
    public class SaddyNormalState : FSMState<Saddy>
    { 
        private static readonly int SwingLevelAnimHash = Animator.StringToHash("SwingLevel");
        private static readonly int ChargeLevelAnimHash = Animator.StringToHash("ChargeLevel");
        private static readonly int IsMoveAnimHash = Animator.StringToHash("IsMove");
        private static readonly int MoveDirAnimHash = Animator.StringToHash("MoveDir");
        private static readonly int MoveSpeedAnimHash = Animator.StringToHash("MoveSpeed");
        
        private const float AvoidDirDampTime = 30;
        private const float TurnoverLimitSpeed = 0.75f * 0.75f;

        private readonly EnemyGameData _enemyData;
        private readonly SaddyData _data;

        private float _targetUpdateCoolTime;
        public Transform _target;
        private Vector2 _currentTargetPos;

        private float _swingCoolTime;
        private float _atkCoolTime;

        private bool _rotateSide;
        
        private InputVector2Damper _dirDamper = new ();
        private InputVector2Damper _avoidDirDamper = new (AvoidDirDampTime);

        private List<Saddy.States> _attackStates = new() {Saddy.States.RollingAttack, Saddy.States.SideStep};
        private int _attackStateIndex;

        private Saddy.States _nextState;
        private float _targetDistance;
        
        public SaddyNormalState(IFSMEntity owner) : base(owner)
        {
            _enemyData = _ownerEntity.Data;
            _data = _ownerEntity.SaddyData;

            _attackStates.Shuffle();
            _attackStateIndex = -1;
        }

        public override void InitializeState()
        {
            _target = SystemManager.Instance.PlayerManager.Player.transform;
            _currentTargetPos = _target.position;
            
            _ownerEntity.Shooter.SetTarget(SystemManager.Instance.PlayerManager.Player.transform);
            _ownerEntity.Animator.SetBool(IsMoveAnimHash, true);

            _nextState = PickAttackState();
        }

        public override void UpdateState()
        {
            _swingCoolTime += Time.deltaTime;
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
            var targetDistance = (_currentTargetPos - (Vector2) _ownerEntity.transform.position).magnitude;
            var currentDir = Move(targetDistance);
            
            var dampedDir = _dirDamper.GetDampedValue(currentDir, Time.deltaTime);
            
            var speed = dampedDir.sqrMagnitude;
            _ownerEntity.Animator.SetFloat(MoveSpeedAnimHash, speed);
            _ownerEntity.Animator.SetBool(IsMoveAnimHash, speed > 0.1f);
            
            
            var targetDir = (Vector2) _target.position - (Vector2) _ownerEntity.transform.position;
            var moveDir = Vector2.Dot(targetDir, dampedDir) <= 0 ? -1f : 1f;
            
            _ownerEntity.SetDir(dampedDir * moveDir, 4);
            _ownerEntity.Animator.SetFloat(MoveDirAnimHash, moveDir * speed);
            
            CheckAttackStart(targetDistance);
        }

        public override void ClearState()
        {
            _ownerEntity.Animator.SetFloat(ChargeLevelAnimHash, 0);
        }

        private Vector2 Move(float targetDistance)
        {
            var dir = SpacingMove(targetDistance);

            var currentDir = Vector2.zero;

            if (dir != Vector2.zero)
            {
                currentDir = _ownerEntity.Rigidbody.velocity.normalized;
                currentDir = Vector2.Lerp(currentDir, dir, 0.4f);
            }

            _ownerEntity.Rigidbody.velocity = currentDir * (_enemyData.MovementSpd);

            return currentDir;
        }
        
        
        private Vector2 SpacingMove(float targetDistance)
        {
            var ownerPos = (Vector2) _ownerEntity.transform.position;
            var dir = _currentTargetPos - ownerPos;
            
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
                interest.AddWeight(_avoidDirDamper.GetDampedValue(avoidDir, Time.deltaTime), 1);
                result = _ownerEntity.Steering.CalculateContexts(danger, interest);
            }

            return result.normalized;
        }
        
        
        private void CheckAttackStart(float targetDistance)
        {
            if (_swingCoolTime > _data.SwingCoolTime)
            {
                Swing();
              
                _swingCoolTime = 0;
                _atkCoolTime -= _data.SwingPlayTime;
            }
            
            if (_atkCoolTime > _data.AttackCoolTime)
            {
                _atkCoolTime = 0;
                _ownerEntity.ChangeState(_nextState);
            }
        }

        private void Swing()
        {
            var swingLevel = _ownerEntity.Animator.GetFloat(ChargeLevelAnimHash);
            DOTween.To(() => swingLevel, x =>
            {
                swingLevel = x;
                _ownerEntity.Animator.SetFloat(ChargeLevelAnimHash, swingLevel);
            }, 1, _data.SwingDelayTime).SetEase(Ease.OutExpo).onComplete += () =>
            {
                _ownerEntity.Animator.SetFloat(ChargeLevelAnimHash, 0);
                _ownerEntity.Animator.SetInteger(SwingLevelAnimHash, 0);
                
                _ownerEntity.Shooter.PlayEnemyAtkSequence(_data.SwingAtkId, ProjectileOwner.Boss);
            };
        }

        private Saddy.States PickAttackState()
        {
            _attackStateIndex++;
            
            if(_attackStateIndex < _attackStates.Count)
            {
                _targetDistance = GetStateTargetDistance(_attackStates[_attackStateIndex]);
                return _attackStates[_attackStateIndex];
            }
            
            _attackStates.Shuffle();
            _attackStateIndex = -1;

            _targetDistance = 5;//_enemyData.SpacingRad;
            return _ownerEntity.GetNextGroupStartState();
        }
        
        private float GetStateTargetDistance(Saddy.States states)
        {
            switch (states)
            {
                case Saddy.States.RollingAttack:
                    return _data.RollingAttackDistance;
                case Saddy.States.SideStep:
                    return _data.SideStepDistance;
            }

            return _enemyData.SpacingRad;
        }

    }
}
