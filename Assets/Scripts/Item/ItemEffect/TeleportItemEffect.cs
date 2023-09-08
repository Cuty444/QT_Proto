using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using QT.Core;
using UnityEngine;
using EventType = QT.Core.EventType;

namespace QT.InGame
{
    // Param1 : 텔레포트 허용 범위
    public class TeleportItemEffect : ItemEffect
    {
        private readonly int SwingAnimHash = Animator.StringToHash("Swing");
        private const string SwingProjectileHitPath = "Effect/Prefabs/FX_Ball_Attack.prefab";
        private LayerMask BounceMask => LayerMask.GetMask("Wall", "Enemy");

        private const int EffectDuration = 200;
        
        
        private readonly Player _player;
        private readonly float _teleportDistance;
        
        private CancellationTokenSource _cancellationTokenSource;

        public TeleportItemEffect(Player player, ItemEffectGameData effectData, SpecialEffectGameData specialEffectData) : base(player, effectData, specialEffectData)
        {
            _player = player;
            _teleportDistance = specialEffectData.Param1;
        }

        public override void OnEquip()
        {
            _lastTime = 0;
        }

        protected override void OnTriggerAction()
        {
            var target = GetTeleportTarget();

            if (target.Item1 == null || target.Item2 == null || target.Item1 == target.Item2)
            {
                return;
            }

            var damage = (int)_player.StatComponent.GetDmg(PlayerStats.EnemyProjectileDmg);
            var shootSpeed = _player.StatComponent.GetStat(PlayerStats.ChargeShootSpd).Value;

            _player.StatComponent.GetStatus(PlayerStats.MercyInvincibleTime).SetStatus(0);
            
            _player.transform.position = target.Item1.Position;
            
            var aimDir = ((Vector2) _player.transform.position - target.Item2.Position).normalized;
            _player.TeleportImpulseSource.GenerateImpulse(aimDir * _player.TeleportImpulseForce);
            
            _player.OnAim.Invoke(aimDir);
            
            target.Item1.ResetProjectileDamage(damage);
            target.Item1.ProjectileHit((target.Item2.Position - target.Item1.Position).normalized, shootSpeed,
                BounceMask, ProjectileOwner.PlayerTeleport, 0, false);
            
            Debug.DrawLine(target.Item1.Position, target.Item2.Position, Color.green,3);
            
            SystemManager.Instance.ResourceManager.EmitParticle(SwingProjectileHitPath, target.Item1.Position);
            _player.Animator.SetTrigger(SwingAnimHash);

            PlayEffect();
            
            SystemManager.Instance.EventManager.InvokeEvent(EventType.OnAttackStunEnemy, null);
        }
        
        private async UniTaskVoid PlayEffect()
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource = new CancellationTokenSource();
            
            SystemManager.Instance.SoundManager.PlayOneShot(SystemManager.Instance.SoundManager.SoundData
                .Player_TeleportAttackSFX);
            
            _player.TeleportEffect(true);
            
            await UniTask.Delay(EffectDuration, cancellationToken: _cancellationTokenSource.Token);
            
            OnRemoved();
        }

        public override void OnRemoved()
        {
            _player.TeleportEffect(false);
        }


        private (Enemy, Enemy) GetTeleportTarget()
        {
            var hitAbles = HitAbleManager.Instance.GetAllHitAble();
            
            Enemy tpTarget = null;
            Enemy target = null;
            float tpTargetPriority = float.MaxValue;
            float targetPriority = float.MaxValue;
            
            foreach (var hitAble in hitAbles)
            {
                if (hitAble is not Enemy)
                {
                    continue;
                }
                
                var enemy = (Enemy) hitAble;
                var enemyDistance = (_player.transform.position- enemy.transform.position).sqrMagnitude;

                if(enemy.HP <= 0 && enemy.CurrentStateIndex == (int)Enemy.States.Rigid && enemyDistance < _teleportDistance * _teleportDistance)
                {
                    if (enemyDistance < tpTargetPriority)
                    {
                        tpTargetPriority = enemyDistance;
                        tpTarget = enemy;
                    }
                }
                else
                {
                    var priority = (1000 * enemy.HP) + enemyDistance;
                    if (!enemy.IsDead && priority < targetPriority)
                    {
                        targetPriority = priority;
                        target = enemy;
                    }
                }
            }

            return (tpTarget, target);
        }
        
    }
}
