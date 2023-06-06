using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QT
{
    public enum AttackType
    {
        Ball,
        Swing,
        Teleport,
    }
    public interface IHitable
    {
        public void Hit(Vector2 dir, float power,AttackType attackType = AttackType.Ball);
    }
}
