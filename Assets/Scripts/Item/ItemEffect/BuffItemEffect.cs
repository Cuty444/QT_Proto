using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QT.InGame
{
    
    public class BuffItemEffect : ItemEffect
    {
        private readonly BuffComponent _buffComponent;
        private Buff _buff;
        
        public BuffItemEffect(ItemEffectGameData effectData, Player player) : base(effectData, player)
        {
            _buffComponent = player.BuffComponent;
        }

        public override void OnEquip()
        {
            _lastTime = 0;
            OnTrigger();
        }

        protected override void OnTriggerAction()
        {
            if (_buff == null || _buff.Duration > 0)
            {
                _buff = _buffComponent.AddBuff(Data.ApplyId, this);
            }
            else
            {
                _buff.RefreshBuff();
            }
        }

        public override void OnRemoved()
        {
            _buffComponent.RemoveAllBuffsFromSource(this);
        }
    }
}
