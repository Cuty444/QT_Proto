using UnityEngine;
using UnityEngine.Events;

namespace QT.InGame
{
    public partial class Enemy
    {
        public UnityEvent<Vector2, float,AttackType> OnDamageEvent { get; } = new();
        public UnityEvent<Vector2, float, LayerMask> OnProjectileHitEvent { get; } = new();

        public void Hit(Vector2 dir, float power,AttackType attackType)
        {
            OnDamageEvent.Invoke(dir, power,attackType);
        }

        public void ProjectileHit(Vector2 dir, float power, LayerMask bounceMask, ProjectileOwner owner, float reflectCorrection,bool isPierce)
        {
            OnProjectileHitEvent.Invoke(dir, power, bounceMask);
        }
        
        public void ResetBounceCount(int maxBounce)
        {
            
        }

        public void ResetProjectileDamage(int damage)
        {
            _damage = damage;

        }
        public LayerMask GetLayerMask()
        {
            return LayerMask.GetMask("Wall") | LayerMask.GetMask("Player"); // TODO : 임시 추후 수정 필요
        }
    }
}