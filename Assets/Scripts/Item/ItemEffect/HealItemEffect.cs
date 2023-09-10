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
        private Status _hpStatus;
        private Player _player;
        
        private float _amount;
        
        public HealItemEffect(Player player, ItemEffectGameData effectData, SpecialEffectGameData specialEffectData) : base(player, effectData, specialEffectData)
        {
            _player = player;
            _hpStatus = player.StatComponent.GetStatus(PlayerStats.HP);
            _amount = specialEffectData.Param1;
        }

        public override void OnEquip()
        {
            _lastTime = 0;
        }

        protected override void OnTriggerAction()
        {
            if (_amount < 0)
            {
                _player.Hit(Vector2.zero, _amount, AttackType.Ball);
            }
            else
            {
                _hpStatus.AddStatus(_amount);
            }
        }

        public override void OnRemoved()
        {
        }
    }
}
