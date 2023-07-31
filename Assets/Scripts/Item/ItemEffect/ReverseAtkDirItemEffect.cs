using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QT.InGame
{
    
    public class ReverseAtkDirItemEffect : ItemEffect
    {
        private readonly Player _player;
        
        public ReverseAtkDirItemEffect(Player player, ItemEffectGameData effectData, SpecialEffectGameData specialEffectData) : base(player, effectData, specialEffectData)
        {
            _player = player;
        }

        public override void OnEquip()
        {
            _lastTime = 0;
            OnTrigger();
        }

        protected override void OnTriggerAction()
        {
            _player.IsReverseLookDir = true;
        }

        public override void OnRemoved()
        {
            _player.IsReverseLookDir = false;
        }
    }
}
