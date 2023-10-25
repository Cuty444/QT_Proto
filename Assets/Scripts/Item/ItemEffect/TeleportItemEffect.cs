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

        public TeleportItemEffect(Item item, Player player, ItemEffectGameData effectData, SpecialEffectGameData specialEffectData) : base(item, player, effectData, specialEffectData)
        {
            _player = player;
            _teleportDistance = specialEffectData.Param1;
        }

        public override void OnEquip()
        {
        }

        
        public override void OnTrigger(bool success)
        {
            if (!success)
            {
                return;
            }
            
            (IEnemy tpTarget, IEnemy target) = GetTeleportTarget();

            if (tpTarget == null || target == null || tpTarget == target)
            {
                return;
            }

            var damage = (int)_player.StatComponent.GetDmg(PlayerStats.EnemyProjectileDmg);
            var shootSpeed = _player.StatComponent.GetStat(PlayerStats.ChargeShootSpd).Value;

            _player.StatComponent.GetStatus(PlayerStats.MercyInvincibleTime).SetStatus(0);
            
            _player.transform.position = tpTarget.Position;
            
            var aimDir = ((Vector2) _player.transform.position - target.Position).normalized;
            _player.TeleportImpulseSource.GenerateImpulse(aimDir * _player.TeleportImpulseForce);
            
            _player.OnAim.Invoke(aimDir);
            
            ((IProjectile) tpTarget).ResetProjectileDamage(damage);
            ((IProjectile) tpTarget).ProjectileHit((target.Position - tpTarget.Position).normalized, shootSpeed,
                BounceMask, ProjectileOwner.PlayerTeleport, 0, false);
            
            Debug.DrawLine(tpTarget.Position, target.Position, Color.green,3);
            
            SystemManager.Instance.ResourceManager.EmitParticle(SwingProjectileHitPath, tpTarget.Position);
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


        private (IEnemy, IEnemy) GetTeleportTarget()
        {
            var hitAbles = HitAbleManager.Instance.GetAllHitAble();
            
            IEnemy tpTarget = null;
            IEnemy target = null;
            float tpTargetPriority = float.MaxValue;
            float targetPriority = float.MaxValue;
            
            foreach (var hitAble in hitAbles)
            {
                if (hitAble is not IEnemy)
                {
                    continue;
                }
                
                var enemy = (IEnemy) hitAble;
                var enemyDistance = ((Vector2)_player.transform.position - enemy.Position).sqrMagnitude;

                if(enemy.HP <= 0 && enemy.IsRigid && enemyDistance < _teleportDistance * _teleportDistance)
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
