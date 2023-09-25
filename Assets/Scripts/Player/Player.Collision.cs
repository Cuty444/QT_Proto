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

        private void OnTriggerEnter2D(Collider2D other)
        {
            if(other.gameObject.layer == LayerMask.NameToLayer("GardenCollider"))
            {
                IsGarden = true;
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if(other.gameObject.layer == LayerMask.NameToLayer("GardenCollider"))
            {
                IsGarden = false;
            }
        }

        public bool CheckFall()
        {
            var collider = Physics2D.OverlapPoint(transform.position, FallLayerMask);
            return collider != null;
        }
    }
}
