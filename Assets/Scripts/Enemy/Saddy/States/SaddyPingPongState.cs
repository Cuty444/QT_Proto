using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using QT.Core;
using QT.Sound;
using QT.Util;
using UnityEngine;

namespace QT.InGame
{
    [FSMState((int) Saddy.States.PingPong)]
    public class SaddyPingPongState : FSMState<Saddy>
    {
        private enum PingPongState
        {
            Ready,
            LoadProjectile,
            
            PingPong,
            End
        }
        
        private static readonly int IsMoveAnimHash = Animator.StringToHash("IsMove");
        private static readonly int MoveDirAnimHash = Animator.StringToHash("MoveDir");
        private static readonly int MoveSpeedAnimHash = Animator.StringToHash("MoveSpeed");
        private static readonly int AttackAnimHash = Animator.StringToHash("Attack");
        
        private readonly EnemyGameData _enemyData;
        private readonly SaddyData _data;

        private Vector2 _currentPos;

        private Bounds _pingPongArea;
        private Player _player;
        
        private ProjectileGameData _projectileData;
        private Projectile _projectile;
        
        private PingPongState _state;
        private float _timer;

        private int _retryCount;
        private int _currentShootIndex;

        private InputVector2Damper _dirDamper = new();
        
        public SaddyPingPongState(IFSMEntity owner) : base(owner)
        {
            _enemyData = _ownerEntity.Data;
            _data = _ownerEntity.SaddyData;
            
            _projectileData = SystemManager.Instance.DataManager.GetDataBase<ProjectileGameDataBase>().GetData(_data.PingPongBallId);
        }

        public override void InitializeState()
        {
            _player = SystemManager.Instance.PlayerManager.Player;
            _pingPongArea = _ownerEntity.MapData.PingPongAreaCollider.bounds;
            
            var playerTargetPos = _ownerEntity.MapData.PingPongPlayerReadyPoint.position;
            
            if (_pingPongArea.Contains(_player.Position))
            {
                (_player.ChangeState(Player.States.KnockBack) as PlayerKnockBackState).InitializeState(playerTargetPos, _data.PingPongReadyTime);
            }

            _retryCount = 0;
            Ready();
        }

        public override void UpdateState()
        {
            _timer += Time.deltaTime;
            _currentPos = _ownerEntity.transform.position;

            switch (_state)
            {
                case PingPongState.Ready:
                    if (_timer > _data.PingPongReadyTime)
                    {
                        _timer = 0;
                        
                        _state = PingPongState.LoadProjectile;
                        LoadProjectile();
                    }
                    break;

                case PingPongState.PingPong:

                    if (!_projectile.gameObject.activeInHierarchy)
                    {
                        _retryCount++;
                        if (_retryCount >= _data.PingPongRetryCount || _projectile.LastHitAble == (IHitAble)_player)
                        {
                            _ownerEntity.ChangeState(_ownerEntity.GetNextGroupStartState());
                        }
                        
                        Ready();
                        return;
                    }

                    if (_projectile.Owner != ProjectileOwner.Boss)
                    {
                        if ((_projectile.Position - _currentPos).sqrMagnitude < (_data.BallHitDistance * _data.BallHitDistance))
                        {
                            _currentShootIndex++;
                            if (_currentShootIndex < _data.PingPongBallSpeed.Length)
                            {
                                Shoot(_currentShootIndex);
                            }
                            else
                            {
                                _projectile.ForceToRelease();
                                Explosion.MakeExplosion(_projectile.Position, _player);
                                
                                _ownerEntity.Hit(Vector2.zero, _ownerEntity.HP.Value * _data.PingPongSuccessDamagePer, AttackType.Ball);
                                
                                _projectile = null;
                                
                                _ownerEntity.ChangeState(Saddy.States.Stun);
                            }
                        }

                    }
                    break;

                case PingPongState.End:
                    break;
            }

        }

        public override void FixedUpdateState()
        {
            if (_state != PingPongState.PingPong)
            {
                return;
            }
            
            var targetPos = GetTargetDir();
            var dir = (targetPos - _currentPos);

            Debug.DrawLine(_ownerEntity.Position, targetPos, Color.red);
            
            _ownerEntity.Rigidbody.velocity = dir.normalized * (_enemyData.MovementSpd);
            
            if (dir.sqrMagnitude > 0.5f)
            {
                var dampedDir = _dirDamper.GetDampedValue(dir.normalized, Time.deltaTime);
                var targetDir = _projectile.Position - _currentPos;
                var lookDir = Vector2.Dot(targetDir, dampedDir) <= 0 ? -1f : 1f;
            
                _ownerEntity.SetDir(dampedDir * lookDir, 4);
                _ownerEntity.Animator.SetFloat(MoveDirAnimHash, lookDir);
                _ownerEntity.Animator.SetFloat(MoveSpeedAnimHash, 1);
                _ownerEntity.Animator.SetBool(IsMoveAnimHash, true);
            }
            else
            {
                _ownerEntity.Rigidbody.velocity = Vector2.zero;
                _ownerEntity.Animator.SetBool(IsMoveAnimHash, false);
            }
        }

        public override void ClearState()
        {
            _ownerEntity.MapData.BarrierObject.SetActive(false);
        }


        private void Ready()
        {
            _ownerEntity.Rigidbody.velocity = Vector2.zero;
            _ownerEntity.Animator.SetBool(IsMoveAnimHash, false);
            
            _state = PingPongState.Ready;
            _timer = 0;

            _currentShootIndex = 0;
        }
        
        
        private Vector2 GetTargetDir()
        {
            if (_projectile.Owner == ProjectileOwner.Boss)
            {
                return _ownerEntity.MapData.PingPongReadyPoint.position;
            }
            
            if ((_projectile.Speed - _enemyData.MovementSpd) == 0)
            {
                return _projectile.Position;
            }
            
            float t = (_projectile.Position - _ownerEntity.Position).magnitude / (_projectile.Speed - _enemyData.MovementSpd);
            var result = _projectile.Position + _projectile.Direction * (t + 0.1f);

            result.x = Mathf.Clamp(result.x, _pingPongArea.min.x, _pingPongArea.max.x);
            result.y = Mathf.Clamp(result.y, _pingPongArea.min.y, _pingPongArea.max.y);

            return result;
        }

        private async UniTaskVoid LoadProjectile()
        {
            _projectile = await SystemManager.Instance.ResourceManager.GetFromPool<Projectile>(_projectileData.PrefabPath);
            
            Shoot(_currentShootIndex);
            
            _timer = 0;
            _state = PingPongState.PingPong;
        }

        private void Shoot(int index)
        {
            var dir = (_player.Position - _ownerEntity.Position).normalized;
            var bounceMask = _ownerEntity.Shooter.BounceMask;
            var properties = ProjectileProperties.Guided | ProjectileProperties.Explosion;
            
            _ownerEntity.SetDir(dir, 4);
            _ownerEntity.Animator.SetTrigger(AttackAnimHash);
            
            _projectile.transform.position = _ownerEntity.ShootPointTransform.position;
            
            _projectile.Init(_projectileData, dir, _data.PingPongBallSpeed[index], 0, bounceMask, ProjectileOwner.Boss, properties, _player.transform, 0, _projectileData.PrefabPath);
            _projectile.ResetSpeedDecay(_data.BallSpeedDecay);
            _projectile.LockProperties(true);
        }
    }
}