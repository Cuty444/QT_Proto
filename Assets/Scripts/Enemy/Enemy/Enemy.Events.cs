using UnityEngine;
using UnityEngine.Events;

namespace QT.InGame
{
    public partial class Enemy
    {
        public UnityEvent<Vector2, float,AttackType> OnDamageEvent { get; } = new();
        public UnityEvent<Vector2, float, LayerMask> OnHitEvent { get; } = new();

        public void Hit(Vector2 dir, float power,AttackType attackType)
        {
            OnDamageEvent.Invoke(dir, power,attackType);
        }
        
        public Vector2 GetPosition()
        {
            return transform.position;
        }
        
        public void ProjectileHit(Vector2 dir, float power, LayerMask bounceMask, ProjectileOwner owner, float reflectCorrection,bool isPierce)
        {
            OnDamageEvent.Invoke(dir, power,AttackType.Swing);
            OnHitEvent.Invoke(dir, power, bounceMask);
            if (owner == ProjectileOwner.PlayerTeleport)
                IsTeleportProjectile = true;
        }
        
        public void ResetBounceCount(int maxBounce)
        {
            
        }

        public void ResetProjectileDamage(int damage)
        {
            
        }

        public LayerMask GetLayerMask()
        {
            return LayerMask.GetMask("Wall") | LayerMask.GetMask("Player"); // TODO : 임시 추후 수정 필요
        }
    }
}