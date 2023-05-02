using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QT.Player
{
    public partial class Player
    {
        public Vector2 BefereDodgeDirecton { get; private set; }
        
        public void SetBefereDodgeDirecton()
        {
            BefereDodgeDirecton = MoveDirection;
        }
    }
}
