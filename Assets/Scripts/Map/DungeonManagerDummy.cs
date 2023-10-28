using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QT
{
    public class DungeonManagerDummy : DungeonManager
    {
        public override bool IsBattle => true;
        
        private new void Start()
        {
        }
        
        private new void OnDestroy()
        {
        }
    }
}
