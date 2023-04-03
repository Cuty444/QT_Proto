using UnityEngine;
using UnityEngine.Events;

namespace QT.Enemy
{
    public partial class Enemy
    {
        public UnityEvent<float, Vector2> OnDamageEvent { get; } = new();

        public void Hit(float damage, Vector2 hitPoint)
        {
            OnDamageEvent.Invoke(damage, hitPoint);
        }
    }
}