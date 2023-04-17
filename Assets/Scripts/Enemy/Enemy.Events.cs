using UnityEngine;
using UnityEngine.Events;

namespace QT.Enemy
{
    public partial class Enemy
    {
        public UnityEvent<Vector2, float> OnDamageEvent { get; } = new();
        public UnityEvent<Vector2, float, LayerMask> OnHitEvent { get; } = new();

        public void Hit(Vector2 dir, float power)
        {
            OnDamageEvent.Invoke(dir, power);
        }
        
        public void ProjectileHit(Vector2 dir, float power, LayerMask bounceMask)
        {
            OnDamageEvent.Invoke(dir, power);
            OnHitEvent.Invoke(dir, power, bounceMask);
        }
        
        public void ResetBounceCount(int maxBounce)
        {
            
        }
        
        public LayerMask GetLayerMask()
        {
            return LayerMask.GetMask("Enemy"); // TODO : 임시 추후 수정 필요
        }
    }
}