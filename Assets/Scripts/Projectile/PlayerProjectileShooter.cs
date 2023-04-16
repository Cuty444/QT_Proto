using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using QT.Core;

namespace QT
{
    public class PlayerProjectileShooter : ProjectileShooter
    {
        public override LayerMask BounceMask => LayerMask.GetMask("Wall", "Enemy");
    }
}