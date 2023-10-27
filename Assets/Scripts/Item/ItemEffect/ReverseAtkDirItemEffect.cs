using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QT.InGame
{
    
    public class ReverseAtkDirItemEffect : ItemEffect
    {
        private readonly Player _player;
        
        public ReverseAtkDirItemEffect(Item item, Player player, ItemEffectGameData effectData, SpecialEffectGameData specialEffectData) : base(item, player, effectData, specialEffectData)
        {
            _player = player;
        }

        public override void OnEquip()
        {
        }

        public override void OnTrigger(bool success)
        {
            if (!success)
            {
                return;
            }
            
            _player.IsReverseLookDir = true;
        }

        public override void OnRemoved()
        {
            _player.IsReverseLookDir = false;
        }
    }
}
