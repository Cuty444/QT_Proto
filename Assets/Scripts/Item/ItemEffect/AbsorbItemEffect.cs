using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using QT.Core;
using UnityEngine;

namespace QT.InGame
{
    // Param1 : 흡수 거리
    // Param2 : 공 최대 속도
    // Param3 : 최대 유지 시간
    // Param4 : 거리 문제
    public class AbsorbItemEffect : ItemEffect
    {
        private readonly LayerMask TargetBounceLayer = LayerMask.GetMask("Player");
        private readonly LayerMask WallBounceLayer = LayerMask.GetMask("Wall", "HardCollider");

        private readonly Player _player;
        private readonly float _absorbRange;
        private readonly float _maxPower;
        private readonly float _maxChargingDuration;
        private readonly float _distance = 2;

        private CancellationTokenSource _cancellationTokenSource;

        private List<IProjectile> _targets = new();

        private bool _isCharging;
        private float _chargingTime;
        private float _power;

        public AbsorbItemEffect(Player player, ItemEffectGameData effectData, SpecialEffectGameData specialEffectData) : base(player, effectData, specialEffectData)
        {
            _player = player;
            _absorbRange = specialEffectData.Param1;
            _maxPower = specialEffectData.Param2;
            _maxChargingDuration = specialEffectData.Param3;
            _distance = specialEffectData.Param4;
        }

        public override void OnEquip()
        {
        }

        protected override void OnTriggerAction()
        {
            _isCharging = true;
            _chargingTime = 0;
            _power = 1;
            
            _targets.Clear();
            CheckProjectiles();
            
            Orbit();
        }

        public override void OnRemoved()
        {
            _cancellationTokenSource?.Cancel();
        }

        
        private async UniTaskVoid Orbit()
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource = new CancellationTokenSource();
            
            while (_chargingTime < _maxChargingDuration)
            {
                await UniTask.NextFrame(PlayerLoopTiming.FixedUpdate, _cancellationTokenSource.Token);
                _chargingTime += Time.deltaTime;
                
                // easeOutCubic
                _power = 1 - _chargingTime / _maxChargingDuration;
                _power = (1 - _power * _power * _power) * _maxPower;

                var rotation = (_chargingTime * 2) * 360;
                var playerPos = (Vector2)_player.transform.position;
                
                for (var i = 0; i < _targets.Count; i++)
                {
                    var projectile = _targets[i];
                    var angle = ((float)i / _targets.Count * 360 + rotation) * Mathf.Deg2Rad;
                    var targetPos = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * _distance + playerPos;
                    
                    var checkDir = (targetPos - playerPos).normalized;
                    //var hit = Physics2D.Raycast(playerPos, checkDir, Distance, WallBounceLayer);
                    var hit = Physics2D.CircleCast(playerPos, projectile.ColliderRad, checkDir, _distance,
                        WallBounceLayer);
                    if (hit)
                    {
                        targetPos = hit.point + (hit.normal * (projectile.ColliderRad * 2));
                    }
                    Debug.DrawLine(playerPos, targetPos);

                    projectile.ProjectileHit((targetPos - projectile.Position).normalized, _power,
                        _player.ProjectileShooter.BounceMask,
                        ProjectileOwner.PlayerAbsorb, 0, false);
                }
            }

            Shoot();
        }

        private void CheckProjectiles()
        {
            List<IProjectile> projectiles = new List<IProjectile>();
            ProjectileManager.Instance.GetInRange(_player.transform.position, _absorbRange, ref projectiles, TargetBounceLayer);

            int damage = (int)_player.StatComponent.GetDmg(PlayerStats.EnemyProjectileDmg2);
            
            foreach (var projectile in projectiles)
            {
                if (projectile is IHitAble)
                {
                    continue;
                }
                
                ProjectileManager.Instance.UnRegister(projectile);
                projectile.ResetProjectileDamage(damage);
                projectile.ResetBounceCount(int.MaxValue);
                _targets.Add(projectile);
            }
        }

        private void Shoot()
        {
            foreach (var projectile in _targets)
            {
                ProjectileManager.Instance.Register(projectile);
                
                projectile.ResetBounceCount(1);
                projectile.ProjectileHit(GetNewProjectileDir(projectile), _power, _player.ProjectileShooter.BounceMask,
                    ProjectileOwner.PlayerAbsorb, 0, false);
            }
            _targets.Clear();
        }
        
        private Vector2 GetNewProjectileDir(IProjectile projectile)
        {
            Vector2 ownerPos = _player.transform.position;
            
            if ((projectile.Position - ownerPos).sqrMagnitude > (_player.AimPosition - ownerPos).sqrMagnitude)
            {
                return (_player.AimPosition -ownerPos).normalized;
            }
            
            return (_player.AimPosition - projectile.Position).normalized;
        }
    }
}
