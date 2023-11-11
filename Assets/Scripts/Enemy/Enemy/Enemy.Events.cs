using UnityEngine;
using UnityEngine.Events;

namespace QT.InGame
{
    public partial class Enemy
    {
        public UnityEvent<Vector2, float,AttackType> OnDamageEvent { get; } = new();
        public UnityEvent<ProjectileHitData> OnProjectileHitEvent { get; } = new();

        public void Hit(Vector2 dir, float power,AttackType attackType)
        {
            OnDamageEvent.Invoke(dir, power,attackType);
        }

        public void ProjectileHit(ProjectileHitData data)
        {
            OnProjectileHitEvent.Invoke(data);
        }
        
        public void ResetBounceCount(int maxBounce)
        {
            
        }

        public void ResetProjectileDamage(int damage)
        {
            ProjectileDamage = damage;

        }
    }
}