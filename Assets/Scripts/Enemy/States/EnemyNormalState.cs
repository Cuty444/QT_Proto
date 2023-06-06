using System.Timers;
using QT.Core;
using UnityEngine;

namespace QT.InGame
{
    [FSMState((int) Enemy.States.Normal)]
    public class EnemyNormalState : FSMState<Enemy>
    {
        private const float TurnoverLimitSpeed = 0.75f * 0.75f;
        private readonly int DirXAnimHash = Animator.StringToHash("DirX");
        private readonly int DirYAnimHash = Animator.StringToHash("DirY");
        
        private readonly EnemyGameData _data;

        private float _lastMoveTargetUpdateTime;
        private Vector2 _moveTarget;

        private float _lastAtkCheckTime;

        private bool _rotateSide;

        public EnemyNormalState(IFSMEntity owner) : base(owner)
        {
            _data = _ownerEntity.Data;
        }

        public override void InitializeState()
        {
            _lastMoveTargetUpdateTime = 0;
            _lastAtkCheckTime = Time.time;
            _ownerEntity.Shooter.SetTarget(SystemManager.Instance.PlayerManager.Player.transform);
        }

        public override void UpdateState()
        {
            if (_lastMoveTargetUpdateTime + _data.MoveTargetUpdatePeroid < Time.time)
            {
                _lastMoveTargetUpdateTime = Time.time;
                Player player = SystemManager.Instance.PlayerManager.Player;
                if (player == null)
                {
                    GameObject.Destroy(_ownerEntity.gameObject);
                    return;
                }
                _moveTarget = SystemManager.Instance.PlayerManager.Player.transform.position;
            }
        }

        public override void FixedUpdateState()
        {
            var targetDistance = (_moveTarget-(Vector2) _ownerEntity.transform.position).magnitude;
            
            Move(targetDistance);
            
            if (CheckAttackStart(targetDistance))
            {
                _ownerEntity.Shooter.PlayEnemyAtkSequence(_data.AtkDataId);
            }
            
            if (_ownerEntity.IsFall)
            {
                _ownerEntity.HP.SetStatus(0);
                _ownerEntity.ChangeState(Enemy.States.Dead);
            }
        }

        public override void ClearState()
        {
        }

        private void Move(float targetDistance)
        {
            var dir = Vector2.zero;

            switch (_data.MoveType)
            {
                case EnemyGameData.MoveTypes.Spacing :
                    dir = SpacingMove(targetDistance,false);
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

            _ownerEntity.Rigidbody.velocity = currentDir * (_data.MovementSpd);
            
            _ownerEntity.Animator.SetFloat(DirXAnimHash, currentDir.x);
            _ownerEntity.Animator.SetFloat(DirYAnimHash, currentDir.y);
        }

        private Vector2 SpacingMove(float targetDistance, bool isRotate = false)
        {
            var ownerPos = (Vector2) _ownerEntity.transform.position;
            var dir = _moveTarget - ownerPos;
            
            var danger = new DirectionWeights();
            var interest = new DirectionWeights();
            
            _ownerEntity.DetectObstacle(ref danger);

            if (targetDistance > _data.SpacingRad)
            {
                interest.AddWeight(dir, 1);
            }
            else if(targetDistance < _data.SpacingRad - 1)
            {
                interest.AddWeight(-dir, 1);
            }
            
            if (isRotate)
            {
                interest.AddWeight(_rotateSide ? new Vector2(dir.y, -dir.x) : new Vector2(-dir.y, dir.x), 1);
            }

            var result = _ownerEntity.CalculateContexts(danger, interest);
            
            if(isRotate && result.sqrMagnitude < TurnoverLimitSpeed)
            {
                _rotateSide = !_rotateSide;
            }
            
            return result.normalized;
        }

        private bool CheckAttackStart(float targetDistance)
        {
            if (_data.AtkDataId == 0 || _lastAtkCheckTime + _data.AtkCheckDelay > Time.time)
            {
                return false;
            }

            switch (_data.AtkStartType)
            {
                case EnemyGameData.AtkStartTypes.AfterIdleSec:
                {
                    _lastAtkCheckTime = Time.time;
                    return true;
                }
                case EnemyGameData.AtkStartTypes.Sight:
                {
                    if (targetDistance < _data.AtkStartParam)
                    {
                        _lastAtkCheckTime = Time.time;
                        return true;
                    }
                    break;
                }
            }

            return false;
        }
    }
}