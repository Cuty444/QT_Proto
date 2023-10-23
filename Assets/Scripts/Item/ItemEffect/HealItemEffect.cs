using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using QT.Core;
using UnityEngine;

namespace QT.InGame
{
    // Param1: 회복량
    public class HealItemEffect : ItemEffect
    {
        private readonly Player _player;
        
        private readonly float _amount;
        
        public HealItemEffect(Player player, ItemEffectGameData effectData, SpecialEffectGameData specialEffectData) : base(player, effectData, specialEffectData)
        {
            _player = player;
            _amount = specialEffectData.Param1;
        }

        public override void OnEquip()
        {
        }

        
        public override void OnTriggerAction(bool success)
        {
            if (!success)
            {
                return;
            }
            
            if (_amount < 0)
            {
                _player.Hit(Vector2.zero, -_amount, AttackType.Ball);
            }
            else
            {
                _player.Heal(_amount);
            }
        }

        public override void OnRemoved()
        {
        }
    }
}
