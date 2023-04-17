using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QT
{
    public interface IHitable
    {
        public void Hit(Vector2 dir, float power);
    }
}
