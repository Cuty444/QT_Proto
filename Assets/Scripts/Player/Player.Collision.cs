using System;
using System.Collections;
using System.Collections.Generic;
using QT.Core;
using QT.Core.Map;
using QT.Map;
using UnityEngine;

namespace QT.InGame
{
    public partial class Player
    {
        private LayerMask FallLayerMask => LayerMask.GetMask("Fall");

        public bool CheckFall()
        {
            var collider = Physics2D.OverlapPoint(transform.position, FallLayerMask);
            return collider != null;
        }
    }
}
