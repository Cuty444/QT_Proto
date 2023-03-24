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

        public EnemyNormalState(IFSMEntity owner) : base(owner)
        {
            _data = _ownerEntity.Data;
        }

        public override void InitializeState()
        {
            _lastMoveTargetUpdateTime = 0;
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
            var dir = Vector2.zero;
            
            if (_data.MoveType != EnemyGameData.MoveTypes.None)
            {
                //SpacingMove(_data.MoveType == EnemyGameData.MoveTypes.SpacingLeft);
                dir = Move(_data.MoveType == EnemyGameData.MoveTypes.SpacingLeft);
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

        public override void ClearState()
        {
        }

        private Vector2 Move(bool isRotate)
        {
            var ownerPos = (Vector2) _ownerEntity.transform.position;
            var dir = _moveTarget - ownerPos;
            var sqrDistance = dir.sqrMagnitude;
            
            var danger = new DirectionWeights();
            var interest = new DirectionWeights();
            
            _ownerEntity.DetectObstacle(danger);

            if (sqrDistance > _data.SpacingRad * _data.SpacingRad)
            {
                interest.AddWeight(dir, 1);
            }
            else if(sqrDistance < (_data.SpacingRad - 1) * (_data.SpacingRad - 1))
            {
                interest.AddWeight(-dir, 1);
            }
            
            if (isRotate)
            {
                interest.AddWeight(new Vector2(dir.y, -dir.x), 1);
            }
            
            dir = Vector2.zero;
            for (int i = 0; i < 8; i++)
            {
                interest.Weights[i] = Mathf.Clamp01(interest.Weights[i] - danger.Weights[i]);
                dir += DirectionWeights.Directions[i] * interest.Weights[i];
            }
            dir.Normalize();

            
            interest.ShowDebugRays(ownerPos, Color.green);
            danger.ShowDebugRays(ownerPos, Color.red);
            Debug.DrawRay(ownerPos, dir, Color.yellow);
            
            return dir;
        }
    }
}