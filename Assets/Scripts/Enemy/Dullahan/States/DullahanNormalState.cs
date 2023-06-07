using System.Timers;
using QT.Core;
using UnityEngine;

namespace QT.InGame
{
    [FSMState((int) Dullahan.States.Normal)]
    public class DullahanNormalState : FSMState<Dullahan>
    {
        private const float TurnoverLimitSpeed = 0.75f * 0.75f;

        private readonly int RotationAnimHash = Animator.StringToHash("Rotation");

        private readonly EnemyGameData _data;

        private float _targetUpdateCoolTime;
        private Vector2 _moveTarget;

        private float _atkCoolTime;

        private bool _rotateSide;

        public DullahanNormalState(IFSMEntity owner) : base(owner)
        {
            _data = _ownerEntity.Data;
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
            
            if (_targetUpdateCoolTime > _data.MoveTargetUpdatePeroid)
            {
                _targetUpdateCoolTime = 0;
                _moveTarget = SystemManager.Instance.PlayerManager.Player.transform.position;
            }
        }

        public override void FixedUpdateState()
        {
            var targetDistance = (_moveTarget - (Vector2) _ownerEntity.transform.position).magnitude;

            Move(targetDistance);

            CheckAttackStart(targetDistance);
        }

        public override void ClearState()
        {
        }

        private void Move(float targetDistance)
        {
            var dir = Vector2.zero;

            switch (_data.MoveType)
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

            _ownerEntity.Rigidbody.velocity = currentDir * (_data.MovementSpd);

            SetRotation(currentDir);
        }

        private void SetRotation(Vector2 dir)
        {
            var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg + 90;

            if (angle < 0)
            {
                angle += 360;
            }

            if (angle > 180)
            {
                angle = 360 - angle;
            }

            _ownerEntity.Animator.SetFloat(RotationAnimHash, Mathf.Round(angle / 180 * 4));
            _ownerEntity.SetFlip(dir.x > 0);
        }

        private Vector2 SpacingMove(float targetDistance, bool isRotate = false)
        {
            var ownerPos = (Vector2) _ownerEntity.transform.position;
            var dir = _moveTarget - ownerPos;

            var danger = new DirectionWeights();
            var interest = new DirectionWeights();

            _ownerEntity.Steering.DetectObstacle(ref danger);

            if (targetDistance > _data.SpacingRad)
            {
                interest.AddWeight(dir, 1);
            }
            else if (targetDistance < _data.SpacingRad - 1)
            {
                interest.AddWeight(-dir, 0.5f);
            }

            if (isRotate)
            {
                interest.AddWeight(_rotateSide ? new Vector2(dir.y, -dir.x) : new Vector2(-dir.y, dir.x), 1);
            }

            var result = _ownerEntity.Steering.CalculateContexts(danger, interest);

            if (isRotate && result.sqrMagnitude < TurnoverLimitSpeed)
            {
                _rotateSide = !_rotateSide;
            }

            return result.normalized;
        }

        private bool CheckAttackStart(float targetDistance)
        {
            if (_atkCoolTime < _data.AtkCheckDelay)
            {
                return false;
            }

            _atkCoolTime = 0;

            Debug.LogError("공습 경보 공습 경보");
            //_ownerEntity.ChangeState(Dullahan.States.Attack);

            return false;
        }
    }
}