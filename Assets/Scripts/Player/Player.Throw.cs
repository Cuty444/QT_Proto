using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QT.Player
{
    public partial class Player
    {

        public void AttackBallInstate()
        {
            ProjectileShooter.ShootProjectile(400, Util.Math.ZAngleToGetDirection(EyeTransform), 1, (int)ThrowBounceCount);
        }
    }
}
