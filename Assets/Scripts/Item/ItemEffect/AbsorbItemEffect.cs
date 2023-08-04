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
    public class AbsorbItemEffect : ItemEffect
    {
        private LayerMask TargetBounceLayer = LayerMask.GetMask("Player");
        
        private readonly Player _player;
        private readonly float _absorbRange;
        private readonly float _maxPower;
        private readonly float _maxChargingDuration;

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
        }

        public override void OnEquip()
        {
            _player.OnActive.AddListener(OnActive);
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
            _player.OnActive.RemoveListener(OnActive);
            _cancellationTokenSource?.Cancel();
        }

        
        private async UniTaskVoid Orbit()
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource = new CancellationTokenSource();
            
            while (_chargingTime < _maxChargingDuration)
            {
                // easeOutCubic
                _power = 1 - _chargingTime / _maxChargingDuration;
                _power = (1 - _power * _power * _power) * _maxPower;

                var rotation = (_chargingTime * 2) * 360;
                
                for (var i = 0; i < _targets.Count; i++)
                {
                    var projectile = _targets[i];
                    var angle = ((float)i / _targets.Count * 360 + rotation) * Mathf.Deg2Rad;
                    var targetPos = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * 2 + (Vector2)_player.transform.position;
                    
                    projectile.ProjectileHit((targetPos - projectile.Position).normalized, _power, _player.ProjectileShooter.BounceMask,
                        ProjectileOwner.PlayerAbsorb, 0, false);
                }

                await UniTask.NextFrame();
                _chargingTime += Time.deltaTime;
            }

            Shoot();
        }

        private void OnActive(bool isOn)
        {
            // if (isOn)
            // {
            //     return;
            // }
            //
            // _cancellationTokenSource?.Cancel();
            // Shoot();
        }

        private void CheckProjectiles()
        {
            List<IProjectile> projectiles = new List<IProjectile>();
            ProjectileManager.Instance.GetInRange(_player.transform.position, _absorbRange, ref projectiles, TargetBounceLayer);

            int damage = (int)_player.StatComponent.GetDmg(PlayerStats.EnemyProjectileDmg1);
            
            foreach (var projectile in projectiles)
            {
                if (projectile is IHitAble {IsDead: true})
                {
                    continue;
                }
                
                ProjectileManager.Instance.UnRegister(projectile);
                projectile.ResetProjectileDamage(damage);
                _targets.Add(projectile);
            }
        }

        private void Shoot()
        {
            foreach (var projectile in _targets)
            {
                ProjectileManager.Instance.Register(projectile);
                
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
