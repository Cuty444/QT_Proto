using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using QT.Core;
using UnityEngine;

namespace QT.InGame
{
    // Param1: 추가하는 골드량
    public class AddGoldItemEffect : ItemEffect
    {
        private readonly int _amount;
        
        public AddGoldItemEffect(Player player, ItemEffectGameData effectData, SpecialEffectGameData specialEffectData) : base(player, effectData, specialEffectData)
        {
            _amount = (int)specialEffectData.Param1;
        }

        public override void OnEquip()
        {
        }

        public override void OnTrigger(bool success)
        {
            if (success)
            {
                SystemManager.Instance.PlayerManager.OnGoldValueChanged.Invoke(_amount);
            }
        }

        public override void OnRemoved()
        {
        }
    }
}
