using UnityEngine;
using UnityEngine.Events;

namespace QT.InGame
{
    public partial class Jello
    {
        public UnityEvent<Vector2, float,AttackType> OnDamageEvent { get; } = new();
        public UnityEvent<float> OnHealEvent { get; } = new();
        
        public void Hit(Vector2 dir, float power, AttackType attackType)
        {
            OnDamageEvent.Invoke(dir, power,attackType);
        }
        
        public void Heal(float amount)
        {
            OnHealEvent.Invoke(amount);
        }
    }
}
