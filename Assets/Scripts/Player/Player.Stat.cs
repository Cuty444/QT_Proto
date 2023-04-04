using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QT.Player
{
    public partial class Player
    {
        public float AtkRadius { get; private set; }
        public float AtkCentralAngle { get; private set; }
        
        public float MercyInvincibleTime { get; private set; }
        public float DodgeInvincibleTime { get; private set; }
        
        public int HPMax { get; private set; }
        public int HP { get; private set; }
    }

}