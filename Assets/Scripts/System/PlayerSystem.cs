using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace QT.Core.Player
{
    public class PlayerSystem : SystemBase
    {
        private Util.Flags.ChargeAtkPierce _chargeAtkPierce;
        public Util.Flags.ChargeAtkPierce ChargeAtkPierce { get => _chargeAtkPierce; set => _chargeAtkPierce = value; }

        private UnityEvent<GameObject> _ballMinSpdDestroyedEvent = new UnityEvent<GameObject>();
        public UnityEvent<GameObject> BallMinSpdDestroyedEvent => _ballMinSpdDestroyedEvent;

        private UnityEvent _batSwingBallHitEvent = new UnityEvent();
        public UnityEvent BatSwingBallHitEvent => _batSwingBallHitEvent;
    }
}
