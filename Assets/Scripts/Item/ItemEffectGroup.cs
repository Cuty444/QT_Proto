using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QT.InGame
{
    public class ItemEffectGroup
    {
        public readonly ItemEffectGameData Data;
        private readonly EffectCondition _condition;
        private readonly StatComponent _ownerStatComponent;
        
        private float _lastTime;

        private List<ItemEffect> _effects = new ();
        
        public ItemEffectGroup(Player player, ItemEffectGameData effectData)
        {
            Data = effectData;
            _ownerStatComponent = player.StatComponent;
            
            if (effectData.Condition != EffectConditions.None)
            {
                _condition = EffectConditionFactory.GetCondition(effectData.Condition, effectData.ConditionTarget, effectData.ConditionValue);
            }
        }

        public void Add(ItemEffect effect)
        {
            _effects.Add(effect);
        }
        
        public void OnTrigger()
        {
            if (Time.timeSinceLevelLoad - _lastTime < Data.CoolTime)
            {
                return;
            }
            
            bool isTrigger = _condition == null || _condition.CheckCondition(_ownerStatComponent);
            
            
            foreach (var effect in _effects)
            {
                effect.OnTrigger(isTrigger);
            }
            
            if (isTrigger)
            {
                _lastTime = Time.timeSinceLevelLoad;
            }
            
        }

        public void OnEquip()
        {
            foreach (var effect in _effects)
            {
                effect.OnEquip();
            }

            _lastTime = 0;
        }

        public void OnRemoved()
        {
            foreach (var effect in _effects)
            {
                effect.OnRemoved();
            }
        }
        
        public float GetCoolTimeProgress()
        {
            if (Data.CoolTime == 0)
            {
                return 0;
            }
            
            return 1 - Mathf.Min(1, (Time.timeSinceLevelLoad - _lastTime) / Data.CoolTime);
        }
        
    }
}
