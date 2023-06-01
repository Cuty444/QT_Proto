using UnityEngine;
using UnityEngine.Events;

namespace QT.InGame
{
    public partial class Enemy
    {
        public UnityEvent<Vector2, float> OnDamageEvent { get; } = new();
        public UnityEvent<Vector2, float, LayerMask> OnHitEvent { get; } = new();

        public void Hit(Vector2 dir, float power)
        {
            OnDamageEvent.Invoke(dir, power);
        }
        
        public void ProjectileHit(Vector2 dir, float power, LayerMask bounceMask, ProjectileOwner owner, float reflectCorrection)
        {
            OnDamageEvent.Invoke(dir, power);
            OnHitEvent.Invoke(dir, power, bounceMask);
        }
        
        public void ResetBounceCount(int maxBounce)
        {
            
        }

        public void ResetProjectileDamage(int damage)
        {
            
        }

        public LayerMask GetLayerMask()
        {
            return LayerMask.GetMask("Wall") | LayerMask.GetMask("Enemy"); // TODO : 임시 추후 수정 필요
        }
    }
}