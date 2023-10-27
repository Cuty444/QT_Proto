using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using QT.Core;
using UnityEngine;

namespace QT.InGame
{
    public class ClearStackEffect : ItemEffect
    {
        private readonly Item _item;
        
        public ClearStackEffect(Item item, Player player, ItemEffectGameData effectData, SpecialEffectGameData specialEffectData) : base(item, player, effectData, specialEffectData)
        {
            _item = item;
        }

        public override void OnEquip()
        {
        }

        
        public override void OnTrigger(bool success)
        {
            if (success)
            {
                _item.ClearStack();
            }
        }

        public override void OnRemoved()
        {
        }
    }
}
