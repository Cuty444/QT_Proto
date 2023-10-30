using System;
using System.Collections;
using System.Collections.Generic;
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
            Serve,
            Toss,
            End
        }
        
        private static readonly int IsMoveAnimHash = Animator.StringToHash("IsMove");
        private static readonly int MoveDirAnimHash = Animator.StringToHash("MoveDir");
        private static readonly int MoveSpeedAnimHash = Animator.StringToHash("MoveSpeed");
        private static readonly int AttackAnimHash = Animator.StringToHash("Attack");
        
        private readonly EnemyGameData _enemyData;
        private readonly SaddyData _data;

        private Vector2 _targetPos;
        private Vector2 _playerTargetPos;

        
        private Player _player;


        private ProjectileGameData _projectileData;
        private Projectile _projectile;
        
        private PingPongState _state;
        private float _timer;

        
        public SaddyPingPongState(IFSMEntity owner) : base(owner)
        {
            _enemyData = _ownerEntity.Data;
            _data = _ownerEntity.SaddyData;
            
            _projectileData = SystemManager.Instance.DataManager.GetDataBase<ProjectileGameDataBase>().GetData(_data.SaddyBallId);
        }

        public override void InitializeState()
        {
            SystemManager.Instance.ResourceManager.CacheAsset(_projectileData.PrefabPath);
            
            _ownerEntity.Animator.SetBool(IsMoveAnimHash, false);
            
            _player = SystemManager.Instance.PlayerManager.Player;
            _playerTargetPos = _ownerEntity.MapData.PingPongPlayerReadyPoint.position;
            
            ReadyProjectile();
            
            if (_ownerEntity.MapData.PingPongAreaCollider.bounds.Contains(_player.transform.position))
            {
                (_player.ChangeState(Player.States.KnockBack) as PlayerKnockBackState).InitializeState(_playerTargetPos, _data.ReadyTime);
                _state = PingPongState.Ready;
            }
            else
            {
                _state = PingPongState.Serve;
                _ownerEntity.Animator.SetTrigger(AttackAnimHash);
            }
            _timer = 0;
        }

        public override void UpdateState()
        {
            _timer += Time.deltaTime;

            switch (_state)
            {
                case PingPongState.Ready:
                    if (_timer > _data.ReadyTime)
                    {
                        _ownerEntity.Animator.SetTrigger(AttackAnimHash);
                        _timer = 0;
                        _state = PingPongState.Serve;
                    }
                    break;
                case PingPongState.Serve:
                    if (_timer > _data.ServeDelayTime)
                    {
                        Shoot();
                        
                        _timer = 0;
                        _state = PingPongState.Toss;
                    }
                    break;
                
                case PingPongState.Toss:

                    if (!_projectile.gameObject.activeInHierarchy)
                    {
                        _ownerEntity.MapData.BarrierObject.SetActive(false);
                        _ownerEntity.ChangeState(_ownerEntity.GetNextGroupStartState());
                    }
                    break;
                
                case PingPongState.End:
                    break;
            }
            
        }

        private async void ReadyProjectile()
        {
            if (_projectileData == null)
            {
                return;
            }

            if (_projectile == null)
            {
                _projectile = await SystemManager.Instance.ResourceManager.GetFromPool<Projectile>(_projectileData.PrefabPath);
            }

            _projectile.gameObject.SetActive(false);
            _projectile.transform.parent = null;
        }
        
        private void Shoot()
        {
            if (_projectile == null)
            {
                ReadyProjectile();
            }
            
            _projectile.gameObject.SetActive(true);
            _projectile.transform.position = _ownerEntity.ShootPointTransform.position;

            var dir = (_player.Position - _ownerEntity.Position).normalized;
            var bounceMask = _ownerEntity.Shooter.BounceMask;
            var properties = ProjectileProperties.Guided | ProjectileProperties.Explosion;
            
            _projectile.Init(_projectileData, dir, _data.SaddyBallSpeed[0], 0, bounceMask, ProjectileOwner.Boss, properties, _player.transform, 0, _projectileData.PrefabPath);
            _projectile.ResetSpeedDecay(_data.BallSpeedDecay);
        }
    }
}