using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using QT.Core;

namespace QT
{
    public class PlayerProjectileShooter : ProjectileShooter
    {
        protected override LayerMask _bounceMask => LayerMask.GetMask("Wall", "Enemy");
    }
}