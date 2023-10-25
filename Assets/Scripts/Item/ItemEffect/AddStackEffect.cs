using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using QT.Core;
using UnityEngine;

namespace QT.InGame
{
    // Param1: 스택 추가량
    public class AddStackEffect : ItemEffect
    {
        private readonly Item _item;
        private readonly int _amount;
        
        public AddStackEffect(Item item, Player player, ItemEffectGameData effectData, SpecialEffectGameData specialEffectData) : base(item, player, effectData, specialEffectData)
        {
            _item = item;
            _amount = (int)specialEffectData.Param1;
        }

        public override void OnEquip()
        {
        }

        
        public override void OnTrigger(bool success)
        {
            if (success)
            {
                _item.AddStack(_amount);
            }
        }

        public override void OnRemoved()
        {
        }
    }
}
