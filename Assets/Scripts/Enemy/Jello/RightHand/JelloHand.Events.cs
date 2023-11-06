using UnityEngine;
using UnityEngine.Events;

namespace QT.InGame
{
    public partial class JelloHand
    {
        public UnityEvent<Vector2, float,AttackType> OnDamageEvent { get; } = new();
        public UnityEvent<(Vector2, float), LayerMask, ProjectileProperties, Transform> OnProjectileHitEvent { get; } = new();

        public void Hit(Vector2 dir, float power,AttackType attackType)
        {
            OnDamageEvent.Invoke(dir, power,attackType);
        }

        public void ProjectileHit(Vector2 dir, float power, LayerMask bounceMask, ProjectileOwner owner, ProjectileProperties properties, Transform target = null)
        {
            OnProjectileHitEvent.Invoke((dir, power), bounceMask, properties, target);
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