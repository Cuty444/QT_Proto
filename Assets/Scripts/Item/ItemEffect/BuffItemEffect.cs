using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QT.InGame
{
    
    public class BuffItemEffect : ItemEffect
    {
        private readonly BuffComponent _buffComponent;
        private Buff _buff;
        
        public BuffItemEffect(Player player, ItemEffectGameData effectData, SpecialEffectGameData specialEffectData) : base(player, effectData, specialEffectData)
        {
            _buffComponent = player.BuffComponent;
        }

        public override void OnEquip()
        {
            _lastTime = 0;
        }

        protected override void OnTriggerAction()
        {
            if (_buff == null || _buff.Duration > 0)
            {
                _buff = _buffComponent.AddBuff(Data.ApplyBuffId, this);
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
