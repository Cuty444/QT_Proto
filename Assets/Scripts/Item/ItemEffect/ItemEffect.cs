using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ApplyTypes = QT.ItemEffectGameData.ApplyTypes;


namespace QT.InGame
{
    public class ItemEffect
    {
        public readonly ItemEffectGameData Data;
        
        private IEffectCondition _condition;
        
        private StatComponent _statComponent;
        private BuffComponent _buffComponent;

        private float _lastTime;
        
        public ItemEffect(ItemEffectGameData effectData, StatComponent statComponent)
        {
            Data = effectData;
            _statComponent = statComponent;
            
            if (effectData.Condition != EffectConditions.None)
            {
                _condition = EffectConditionFactory.GetCondition(effectData.Condition);
            }
        }

        public void OnTrigger()
        {
            if (Time.timeSinceLevelLoad - _lastTime < Data.CoolTime)
            {
                return;
            }
            
            if (_condition == null || _condition.CheckCondition(_statComponent, Data.ConditionTarget))
            {
                switch (Data.ApplyType)
                {
                    case ApplyTypes.Buff:
                        _buffComponent.AddBuff(Data.ApplyId);
                        break;
                    case ApplyTypes.Active:
                        
                        break;
                }
                _lastTime = Time.timeSinceLevelLoad;
            }
        }
    }
}
