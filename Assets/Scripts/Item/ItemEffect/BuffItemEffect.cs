using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QT.InGame
{
    
    public class BuffItemEffect : ItemEffect
    {
        private readonly BuffComponent _buffComponent;
        private Buff _buff;
        
        private readonly int _buffId;
        private readonly Item _item;

        public BuffItemEffect(Item item, Player player, ItemEffectGameData effectData, SpecialEffectGameData specialEffectData) : base(item, player, effectData, specialEffectData)
        {
            _buffComponent = player.BuffComponent;

            _item = item;
            _buffId = effectData.ApplyBuffId;
        }

        public override void OnEquip()
        {
        }

        public override void OnTrigger(bool success)
        {
            if (!success)
            {
                OnRemoved();
                return;
            }
            
            if (_buff == null || _buff.Duration > 0)
            {
                _buff = _buffComponent.AddBuff(_buffId, _item.Stack, this);
            }
            else
            {
                _buff.RefreshBuff(_item.Stack);
            }
        }

        public override void OnRemoved()
        {
            _buffComponent.RemoveAllBuffsFromSource(this);
            _buff = null;
        }
    }
}
