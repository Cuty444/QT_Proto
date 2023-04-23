using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace QT.Player
{
    public partial class Player
    {
        public UnityEvent<Vector2, float> OnDamageEvent { get; } = new();

        public void Hit(Vector2 dir, float power)
        {
            OnDamageEvent.Invoke(dir, power);
        }
    }
}