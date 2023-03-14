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

        private UnityEvent<float> _chargeAtkShootEvent = new UnityEvent<float>();
        public UnityEvent<float> ChargeAtkShootEvent => _chargeAtkShootEvent;

        private UnityEvent<int> _batSwingRigidHitEvent = new UnityEvent<int>();
        public UnityEvent<int> BatSwingRigidHitEvent => _batSwingRigidHitEvent;
    }
}
