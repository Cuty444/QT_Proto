using System.Collections;
using System.Collections.Generic;
using QT.Core;
using QT.Sound;
using QT.Util;
using UnityEngine;

namespace QT.InGame
{
    [FSMState((int) Saddy.States.PingPongReady)]
    public class SaddyPingPongReadyState : FSMState<Saddy>
    {
        private static readonly int IsMoveAnimHash = Animator.StringToHash("IsMove");
        private static readonly int MoveDirAnimHash = Animator.StringToHash("MoveDir");
        private static readonly int MoveSpeedAnimHash = Animator.StringToHash("MoveSpeed");
        
        private const float AvoidDirDampTime = 30;
        private const float TurnoverLimitSpeed = 0.75f * 0.75f;

        private readonly EnemyGameData _enemyData;
        private readonly SaddyData _data;

        private Vector2 _targetPos;

        private bool _rotateSide;
        
        private InputVector2Damper _dirDamper = new ();
        private InputVector2Damper _avoidDirDamper = new (AvoidDirDampTime);

        private Player _player;
        
        public SaddyPingPongReadyState(IFSMEntity owner) : base(owner)
        {
            _enemyData = _ownerEntity.Data;
            _data = _ownerEntity.SaddyData;
        }

        public override void InitializeState()
        {
            _targetPos = _ownerEntity.MapData.PingPongReadyPoint.position;
            
            _ownerEntity.Animator.SetBool(IsMoveAnimHash, true);

            _player = SystemManager.Instance.PlayerManager.Player;
        }

        public override void FixedUpdateState()
        {
            var targetDistance = (_targetPos - (Vector2) _ownerEntity.transform.position).sqrMagnitude;
            if (targetDistance < 0.5f)
            {
                _ownerEntity.MapData.BarrierObject.SetActive(true);
                _ownerEntity.ChangeState(Saddy.States.PingPong); 
                return;
            }

            var currentDir = Move();
            
            var dampedDir = _dirDamper.GetDampedValue(currentDir, Time.deltaTime);
            _ownerEntity.SetDir(dampedDir, 4);
            
            var speed = dampedDir.sqrMagnitude;
            _ownerEntity.Animator.SetFloat(MoveSpeedAnimHash, speed);
            _ownerEntity.Animator.SetBool(IsMoveAnimHash, speed > 0.1f);
            _ownerEntity.Animator.SetFloat(MoveDirAnimHash, speed);
        }

        private Vector2 Move()
        {
            var dir = CalculateWeights();
            var currentDir = Vector2.zero;

            if (dir != Vector2.zero)
            {
                currentDir = _ownerEntity.Rigidbody.velocity.normalized;
                currentDir = Vector2.Lerp(currentDir, dir, 0.4f);
            }

            _ownerEntity.Rigidbody.velocity = currentDir * (_enemyData.MovementSpd);

            return currentDir;
        }
        
        
        private Vector2 CalculateWeights()
        {
            var ownerPos = (Vector2) _ownerEntity.transform.position;
            var dir = _targetPos - ownerPos;
            
            var danger = new DirectionWeights();
            var interest = new DirectionWeights();
            
            _ownerEntity.Steering.DetectObstacle(ref danger);

            // 타겟으로 이동
            interest.AddWeight(dir, 1);

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
        
    }
}