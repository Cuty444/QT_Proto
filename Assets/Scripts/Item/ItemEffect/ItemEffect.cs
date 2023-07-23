using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using ApplyTypes = QT.ItemEffectGameData.ApplyTypes;
using TriggerTypes = QT.ItemEffectGameData.TriggerTypes;


namespace QT.InGame
{
    public abstract class ItemEffect
    {
        public readonly bool IsAvailable = false;

        public readonly ApplyTypes ApplyType;
        
        
        public TriggerTypes TriggerType { get; protected set; }
        
        private IEffectCondition _condition;
        
        public ItemEffect(ItemEffectGameData effectData)
        {
            ApplyType = effectData.ApplyType;
        }

        public void OnTrigger()
        {
            
        }

        public bool CheckCondition()
        {
            return false;
        }


        protected abstract bool Process(ItemEffectGameData effectData);

    }
}
