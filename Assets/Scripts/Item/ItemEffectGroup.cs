using System.Collections;
using System.Collections.Generic;
using QT.Core;
using UnityEngine;

namespace QT.InGame
{
    public class ItemEffectGroup
    {
        private readonly Item _item;
        
        public readonly ItemEffectGameData Data;
        private readonly EffectCondition _condition;
        private readonly StatComponent _ownerStatComponent;
        
        private float _lastTime;

        private List<ItemEffect> _effects = new ();

        private PlayerManager _playerManager;
        
        public ItemEffectGroup(Item item, Player player, ItemEffectGameData effectData)
        {
            _item = item;
            Data = effectData;
            _ownerStatComponent = player.StatComponent;

            if (effectData.Condition != EffectConditions.None)
            {
                _condition = EffectConditionFactory.GetCondition(effectData.Condition, effectData.ConditionTarget, effectData.ConditionValue);
            }

            _playerManager = SystemManager.Instance.PlayerManager;
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
            
            bool isTrigger = _condition == null || _condition.CheckCondition(_ownerStatComponent, _item.Stack);
            
            
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
            _playerManager.PlayerNextFloor.AddListener(ResetLastTime);
        }

        public void OnRemoved()
        {
            foreach (var effect in _effects)
            {
                effect.OnRemoved();
            }
            _playerManager.PlayerNextFloor.RemoveListener(ResetLastTime);
        }
        
        public float GetCoolTimeProgress()
        {
            if (Data.CoolTime == 0)
            {
                return 0;
            }
            
            return 1 - Mathf.Min(1, (Time.timeSinceLevelLoad - _lastTime) / Data.CoolTime);
        }

        private void ResetLastTime()
        {
            _lastTime = 0f;
        }
        
    }
}
