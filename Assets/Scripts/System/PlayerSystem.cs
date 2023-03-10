using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace QT.Core.Player
{
    public class PlayerSystem : SystemBase
    {
        private UnityEvent<GameObject> _ballMinSpdDestroyedEvent = new UnityEvent<GameObject>();
        public UnityEvent<GameObject> BallMinSpdDestroyedEvent { get => _ballMinSpdDestroyedEvent; }

        private UnityEvent _batSwingBallHitEvent = new UnityEvent();
        public UnityEvent BatSwingBallHitEvent { get => _batSwingBallHitEvent; }
    }
}
