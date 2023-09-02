using System.Timers;
using QT.Core;
using QT.Util;
using UnityEngine;

namespace QT.InGame
{
    [FSMState((int) Enemy.States.Normal)]
    public class EnemyNormalState : FSMState<Enemy>
    {
        private const float DirDampTime = 30;
        private const float AvoidDirDampTime = 20;
        private const float TurnoverLimitSpeed = 0.75f * 0.75f;
        
        private static readonly int DirXAnimHash = Animator.StringToHash("DirX");
        private static readonly int DirYAnimHash = Animator.StringToHash("DirY");
        private static readonly int IsMoveAnimHash = Animator.StringToHash("IsMove");
        
        private readonly EnemyGameData _data;

        private float _moveTargetUpdateTimer;
        private Vector2 _moveTarget;

        private float _atkCheckTimer;

        private bool _rotateSide;
        private bool _isAgro;
        
        private InputVector2Damper _dirDamper = new (DirDampTime);
        private InputVector2Damper _avoidDirDamper = new (AvoidDirDampTime);

        public EnemyNormalState(IFSMEntity owner) : base(owner)
        {
            _data = _ownerEntity.Data;

            _ownerEntity.OnDamageEvent.AddListener((dir, power, type) => { _isAgro = true; });
        }

        public override void InitializeState()
        {
            _moveTargetUpdateTimer = 9999;
            _atkCheckTimer = 0;
            _ownerEntity.Shooter.SetTarget(SystemManager.Instance.PlayerManager.Player.transform);
        }

        public override void UpdateState()
        {
            _moveTargetUpdateTimer += Time.deltaTime;

            if (!_ownerEntity.Shooter.IsAttacking)
            {
                _atkCheckTimer += Time.deltaTime;
            }
            
            if (_moveTargetUpdateTimer > _data.MoveTargetUpdatePeroid)
            {
                _moveTargetUpdateTimer = 0;
                // todo : 재시작을 위한 임시 처리
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

            if (!CheckAgro(targetDistance))
            {
                return;
            }
            
            Move(targetDistance);
            
            CheckBodyContact();
            
            if (CheckAttackStart(targetDistance))
            {
                _ownerEntity.Shooter.PlayEnemyAtkSequence(_data.AtkDataId,_ownerEntity.Owner);
            }
            
            if (_ownerEntity.Steering.IsStuck())
            {
                _ownerEntity.HP.SetStatus(0);
                _ownerEntity.ChangeState(Enemy.States.Dead);
            }
        }

        private bool CheckAgro(float targetDistance)
        {
            if (targetDistance < _data.AgroRange)
            {
                _isAgro = true;
            }

            return _isAgro;
        }

        public override void ClearState()
        {
            _ownerEntity.Shooter.StopAttack();
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


            var dampedDir = _dirDamper.GetDampedValue(currentDir, Time.deltaTime);
            _ownerEntity.Animator.SetFloat(DirXAnimHash, dampedDir.x);
            _ownerEntity.Animator.SetFloat(DirYAnimHash, dampedDir.y);
            _ownerEntity.Animator.SetBool(IsMoveAnimHash, dampedDir.sqrMagnitude > 0.1f);
        }

        private Vector2 SpacingMove(float targetDistance, bool isRotate = false)
        {
            var ownerPos = (Vector2) _ownerEntity.transform.position;
            var dir = _moveTarget - ownerPos;
            
            var danger = new DirectionWeights();
            var interest = new DirectionWeights();
            
            _ownerEntity.Steering.DetectObstacle(ref danger);

            // 타겟과의 거리 유지
            if (targetDistance > _data.SpacingRad)
            {
                interest.AddWeight(dir, 1);
            }
            else if(targetDistance < _data.SpacingRad - 1)
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

        private void CheckBodyContact()
        {
            if (_data.BodyContactDmg <= 0)
            {
                return;
            }
            
            var hit = Physics2D.OverlapCircle(_ownerEntity.transform.position, _ownerEntity.ColliderRad,
                _ownerEntity.Shooter.BounceMask);

            var ownerPos = _ownerEntity.transform.position;
            
            if (hit != null && hit.TryGetComponent(out IHitAble hitable))
            {
                hitable.Hit((ownerPos - hit.transform.position).normalized, _data.BodyContactDmg);
            }
        }
        
        private bool CheckAttackStart(float targetDistance)
        {
            if (_data.AtkDataId == 0 || _atkCheckTimer < _data.AtkCheckDelay)
            {
                return false;
            }

            switch (_data.AtkStartType)
            {
                case EnemyGameData.AtkStartTypes.AfterIdleSec:
                {
                    _atkCheckTimer = 0;
                    return true;
                }
                case EnemyGameData.AtkStartTypes.Sight:
                {
                    if (targetDistance < _data.AtkStartParam)
                    {
                        _atkCheckTimer = 0;
                        return true;
                    }
                    break;
                }
            }

            return false;
        }
    }
}