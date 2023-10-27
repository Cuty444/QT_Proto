using UnityEngine;
using UnityEngine.Events;

namespace QT.InGame
{
    public partial class Saddy
    {
        public UnityEvent<Vector2, float,AttackType> OnDamageEvent { get; } = new();
        
        public void Hit(Vector2 dir, float power, AttackType attackType)
        {
            OnDamageEvent.Invoke(dir, power,attackType);
        }
    }
}
