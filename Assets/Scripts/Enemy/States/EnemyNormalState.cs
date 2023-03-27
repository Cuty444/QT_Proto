using System.Timers;
using QT.Core;
using QT.Core.Enemy;
using UnityEngine;

namespace QT.Enemy
{
    [FSMState((int) Enemy.States.Normal)]
    public class EnemyNormalState : FSMState<Enemy>
    {
        private readonly EnemyGameData _data;

        private float _lastMoveTargetUpdateTime;
        private Vector2 _moveTarget;

        private float _lastAtkCheckTime;

        public EnemyNormalState(IFSMEntity owner) : base(owner)
        {
            _data = _ownerEntity.Data;
        }

        public override void InitializeState()
        {
            _lastMoveTargetUpdateTime = 0;
            _lastAtkCheckTime = Time.time;
        }

        public override void UpdateState()
        {
            if (_lastMoveTargetUpdateTime + _data.MoveTargetUpdatePeroid < Time.time)
            {
                _lastMoveTargetUpdateTime = Time.time;
                _moveTarget = SystemManager.Instance.GetSystem<EnemySystem>().PlayerTransform.position;
            }
        }

        public override void FixedUpdateState()
        {
            var targetDistance = (_moveTarget-(Vector2) _ownerEntity.transform.position).magnitude;
            
            Move(targetDistance);
            if (CheckAttackStart(targetDistance))
            {
                Debug.Log("빵야 빵야");
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
            
            if (dir != Vector2.zero)
            {
                var currentDir = _ownerEntity.Rigidbody.velocity.normalized;
                currentDir = Vector2.Lerp(currentDir, dir, 0.4f);
                _ownerEntity.Rigidbody.velocity = currentDir * (_data.MoveSpd * Time.fixedDeltaTime);
            }
            else
            {
                _ownerEntity.Rigidbody.velocity = Vector2.zero;
            }
        }

        private Vector2 SpacingMove(float targetDistance, bool isRotate = false)
        {
            var ownerPos = (Vector2) _ownerEntity.transform.position;
            var dir = _moveTarget - ownerPos;
            
            var danger = new DirectionWeights();
            var interest = new DirectionWeights();
            
            _ownerEntity.DetectObstacle(danger);

            if (targetDistance > _data.SpacingRad)
            {
                interest.AddWeight(dir, 1);
            }
            else if(targetDistance < _data.SpacingRad - 1)
            {
                interest.AddWeight(-dir, 1);
            }
            else if (isRotate)
            {
                interest.AddWeight(new Vector2(dir.y, -dir.x), 1);
            }

            dir = Vector2.zero;
            for (int i = 0; i < DirectionWeights.DirCount; i++)
            {
                interest.Weights[i] = Mathf.Clamp01(interest.Weights[i] - danger.Weights[i]);
                dir += DirectionWeights.Directions[i] * interest.Weights[i];
            }
            dir.Normalize();

            // Debug
            #if UNITY_EDITOR
            interest.ShowDebugRays(ownerPos, Color.green);
            danger.ShowDebugRays(ownerPos, Color.red);
            Debug.DrawRay(ownerPos, dir, Color.yellow);
            #endif
            
            return dir;
        }

        private bool CheckAttackStart(float targetDistance)
        {
            if (_data.AtkDataId == 0 || _lastAtkCheckTime + _data.AtkCheakDelay > Time.time)
            {
                return false;
            }

            switch (_data.AtkStartType)
            {
                case EnemyGameData.AtkStartTypes.Sight:
                    if (targetDistance < _data.AtkStartParam)
                    {
                        _lastAtkCheckTime = Time.time;
                        return true;
                    }
                    break;
            }
            
            return false;
        }
    }
}