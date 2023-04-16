using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QT.Player
{
    public partial class Player
    {

        public void AttackBallInstate()
        {
            ProjectileShooter.PlayerShoot(400, AimTypes.MoveDirection, QT.Util.Math.ZAngleToGetDirection(_eyeTransform),(int)ThrowBounceCount.Value,ProjectTileList);
        }
    }
}
