using System.Collections.Generic;
using System.Timers;
using QT.Core;
using UnityEngine;
using System.Linq;

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

        public DullahanNormalState(IFSMEntity owner) : base(owner)
        {
            _enemyData = _ownerEntity.Data;
            _dullahanData = _ownerEntity.DullahanData;
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
                    dir = SpacingMove(targetDistance, false);
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

            if (targetDistance > _enemyData.SpacingRad)
            {
                interest.AddWeight(dir, 1);
            }
            else if (targetDistance < _enemyData.SpacingRad - 1)
            {
                interest.AddWeight(-dir, 0.5f);
            }
            
            var avoidDir = _avoidDirDamper.GetDampedValue(Vector2.zero, Time.deltaTime);
            if (avoidDir != Vector2.zero)
            {
                interest.AddWeight(_avoidDirDamper.GetDampedValue(avoidDir, Time.deltaTime), 1);
            }
            
            if (isRotate)
            {
                interest.AddWeight(_rotateSide ? new Vector2(dir.y, -dir.x) : new Vector2(-dir.y, dir.x), 1);
            }

            var result = _ownerEntity.Steering.CalculateContexts(danger, interest);

            if (result.sqrMagnitude < TurnoverLimitSpeed)
            {
                if (isRotate)
                {
                    _rotateSide = !_rotateSide;
                }

                _avoidDirDamper.ResetCurrentValue(-result);
            }

            return result.normalized;
        }

        private void CheckAttackStart(float targetDistance)
        {
            if (_atkCoolTime < _enemyData.AtkCheckDelay)
            {
                return;
            }

            _atkCoolTime = 0;
            
            var passableStates = new List<Dullahan.States>();

            //  if (_dullahanData.AttackRangeMin <= targetDistance && _dullahanData.AttackRangeMax >= targetDistance)
            //  {
            //      passableStates.Add(Dullahan.States.Attack);
            //  }
            //  if (_dullahanData.RushRangeMin <= targetDistance && _dullahanData.RushRangeMax >= targetDistance)
            //  {
            //      passableStates.Add(Dullahan.States.Rush);
            //  }
            //  if (_dullahanData.JumpRangeMin <= targetDistance && _dullahanData.JumpRangeMax >= targetDistance)
            //  {
            //      passableStates.Add(Dullahan.States.Jump);
            //  }
            // if (_dullahanData.ThrowRangeMin <= targetDistance && _dullahanData.ThrowRangeMax >= targetDistance)
            //  {
            //      passableStates.Add(Dullahan.States.Throw);
            //  }
            //
            
            passableStates.Add(Dullahan.States.Jump);

            if (passableStates.Count == 0)
            {
                return;
            }
            
            var rand = new System.Random();
            _ownerEntity.ChangeState(passableStates[rand.Next(passableStates.Count)]);
        }
    }
}