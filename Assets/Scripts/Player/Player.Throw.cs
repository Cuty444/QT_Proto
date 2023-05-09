using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QT.Player
{
    public partial class Player
    {

        public void AttackBallInstate()
        {
            float throwSpd = GetStat(PlayerStats.ThrowSpd);
            int throwBounceCount = (int)GetStat(PlayerStats.ThrowBounceCount);
            
            ProjectileShooter.ShootProjectile(200, Util.Math.ZAngleToGetDirection(EyeTransform), throwSpd, 0, throwBounceCount, 2.5f);
        }
    }
}
