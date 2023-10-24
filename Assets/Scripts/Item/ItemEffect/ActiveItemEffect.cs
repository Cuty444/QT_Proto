using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QT.InGame
{
    public class ActiveItemEffect : ItemEffect
    {
        private readonly Action _onTriggerAction;

        public ActiveItemEffect(Action onTriggerAction) : base(null, null, null)
        {
            _onTriggerAction = onTriggerAction;
        }

        public override void OnEquip()
        {
        }

        public override void OnTrigger(bool success)
        {
            if (success)
            {
                _onTriggerAction.Invoke();
            }
        }

        public override void OnRemoved()
        {
        }
    }
}
