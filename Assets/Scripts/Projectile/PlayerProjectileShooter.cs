using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using QT.InGame;

namespace QT
{
    public class PlayerProjectileShooter : ProjectileShooter
    {
        public override LayerMask BounceMask => LayerMask.GetMask("Wall","HardCollider","ProjectileCollider", "Enemy","InteractionCollider");
        
        protected override Vector2 GetDirection(float angle, AimTypes aimType)
        {
            if (aimType == AimTypes.Target)
            {
                aimType = AimTypes.MoveDirection;
            }
            
            return base.GetDirection(angle, aimType);
        }
    }
}