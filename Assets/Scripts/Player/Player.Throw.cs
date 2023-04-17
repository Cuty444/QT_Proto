using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QT.Player
{
    public partial class Player
    {

        public void AttackBallInstate()
        {
            ProjectileShooter.ShootProjectile(200, Util.Math.ZAngleToGetDirection(EyeTransform), ThrowSpd, (int)ThrowBounceCount, 2.5f);
        }
    }
}
