using UnityEngine;
using UnityEngine.Events;

namespace QT.Enemy
{
    public partial class Enemy
    {
        public UnityEvent<Vector2, float> OnDamageEvent { get; } = new();

        public void Hit(Vector2 dir, float power)
        {
            OnDamageEvent.Invoke(dir, power);
        }
    }
}