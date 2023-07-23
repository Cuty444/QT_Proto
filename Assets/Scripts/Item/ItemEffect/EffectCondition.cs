using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace QT.InGame
{
    public enum EffectConditions
    {
        None,
        LessThan,
        GreaterThan,
        Equal,
        
        Random
    }
    
    public interface IEffectCondition
    {
        public EffectConditions EffectConditionType { get; }

        public bool CheckCondition(StatComponent statComponent, float target);
    }
    
    public static class EffectConditionFactory
    {
        private static readonly Dictionary<EffectConditions, IEffectCondition> _conditionTypes = new ();
        
        static EffectConditionFactory()
        {
            var types = Assembly.GetExecutingAssembly().GetTypes().Where(t => typeof(IEffectCondition) != t && typeof(IEffectCondition).IsAssignableFrom(t));
            foreach (var type in types)
            {
                var condition = Activator.CreateInstance(type) as IEffectCondition;
                _conditionTypes.Add(condition.EffectConditionType, condition);
            }
        }

        public static IEffectCondition GetCondition(string condition)
        {
            if (EffectConditions.TryParse(condition, out EffectConditions type))
            {
                return _conditionTypes[type];
            }
            
            return null;
        }
    }
    
    
    public class LessThanCondition : IEffectCondition
    {
        public EffectConditions EffectConditionType => EffectConditions.LessThan;
       
        public bool CheckCondition(StatComponent statComponent, float target)
        {
            return statComponent.GetStat(PlayerStats.HP) < target;
        }
    }
    
    public class GreaterThanCondition : IEffectCondition
    {
        public EffectConditions EffectConditionType => EffectConditions.GreaterThan;
       
        public bool CheckCondition(StatComponent statComponent, float target)
        {
            return statComponent.GetStat(PlayerStats.HP) > target;
        }
    }
    
    public class EqualCondition : IEffectCondition
    {
        public EffectConditions EffectConditionType => EffectConditions.Equal;
       
        public bool CheckCondition(StatComponent statComponent, float target)
        {
            return target.CompareTo(statComponent.GetStat(PlayerStats.HP)) == 0;
        }
    }
    
    public class RandomCondition : IEffectCondition
    {
        public EffectConditions EffectConditionType => EffectConditions.Random;
       
        public bool CheckCondition(StatComponent statComponent, float target)
        {
            return UnityEngine.Random.Range(0, 100) < target;
        }
    }
}
