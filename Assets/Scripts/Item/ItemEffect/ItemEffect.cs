using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using ApplyTypes = QT.ItemEffectGameData.ApplyTypes;
using TriggerTypes = QT.ItemEffectGameData.TriggerTypes;
using Conditions = QT.ItemEffectGameData.Conditions;


namespace QT.InGame
{
    public abstract class ItemEffect
    {
        private static readonly Dictionary<Conditions, ICondition> _conditionTypes = new ();
        
        static ItemEffect()
        {
            var types = Assembly.GetExecutingAssembly().GetTypes().Where(t => typeof(ICondition) != t && typeof(ICondition).IsAssignableFrom(t));
            foreach (var type in types)
            {
                var condition = Activator.CreateInstance(type) as ICondition;
                _conditionTypes.Add(condition.ConditionType, condition);
            }
        }
        
        
        
        public virtual ApplyTypes ApplyType => ApplyTypes.Buff;
        
        public TriggerTypes TriggerType { get; protected set; }
        public readonly bool IsAvailable = false;
        
        public ItemEffect(ItemEffectGameData effectData)
        {
            if(effectData == null || effectData.ApplyType != ApplyType)
            {
                return;
            }
            
            TriggerType = effectData.TriggerType;
            
            IsAvailable = Process(effectData);
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
