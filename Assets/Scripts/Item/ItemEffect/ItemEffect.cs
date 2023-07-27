using System;
using System.Collections;
using System.Collections.Generic;
using QT.Core;
using UnityEngine;
using ApplyTypes = QT.ItemEffectGameData.ApplyTypes;


namespace QT.InGame
{
    public class ItemEffect
    {
        public readonly ItemEffectGameData Data;
        
        private readonly IEffectCondition _condition;
        
        private readonly StatComponent _ownerStatComponent;
        private BuffComponent _buffComponent;

        private float _lastTime;

        private Buff _buff;

        public ItemEffect(ItemEffectGameData effectData, Player player)
        {
            Data = effectData;
            _ownerStatComponent = player.StatComponent;
            _buffComponent = player.BuffComponent;
            
            if (effectData.Condition != EffectConditions.None)
            {
                _condition = EffectConditionContainer.GetCondition(effectData.Condition);
            }
        }

        public void OnTrigger()
        {
            if (Time.timeSinceLevelLoad - _lastTime < Data.CoolTime)
            {
                return;
            }
            
            if (_condition == null || _condition.CheckCondition(_ownerStatComponent, Data.ConditionValue))
            {
                switch (Data.ApplyType)
                {
                    case ApplyTypes.Buff:
                        if (_buff == null || _buff.Duration > 0)
                        {
                            _buff = _buffComponent.AddBuff(Data.ApplyId, this);
                        }
                        else
                        {
                            _buff.RefreshBuff();
                        }
                        break;
                    
                    case ApplyTypes.Active:
                        
                        break;
                }
                _lastTime = Time.timeSinceLevelLoad;
            }
            
        }

        public void OnEquip()
        {
            _lastTime = 0;
            OnTrigger();
        }

        public void OnRemoved()
        {
            switch (Data.ApplyType)
            {
                case ApplyTypes.Buff:
                    _buffComponent.RemoveAllBuffsFromSource(this);
                    break;
                case ApplyTypes.Active:
                        
                    break;
            }
        }
    }
}
