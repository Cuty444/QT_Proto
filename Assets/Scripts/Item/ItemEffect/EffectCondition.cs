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

    [AttributeUsage(AttributeTargets.Class)]
    public class EffectConditionAttribute : Attribute
    {
        public EffectConditions ConditionType { get; private set; }

        public EffectConditionAttribute(EffectConditions conditionType)
        {
            ConditionType = conditionType;
        }
    }

    public abstract class EffectCondition
    {
        protected float _value;
        
        public EffectCondition(string target, float value)
        {
            _value = value;
        }

        public abstract bool CheckCondition(StatComponent statComponent);
    }
    
    public static class EffectConditionFactory
    {
        private static readonly Dictionary<EffectConditions, Type> _conditionTypes = new ();
        
        static EffectConditionFactory()
        {
            var types = Assembly.GetExecutingAssembly().GetTypes().Where(t => typeof(EffectCondition) != t && typeof(EffectCondition).IsAssignableFrom(t));
            foreach (var type in types)
            {
                var attribute = type.GetCustomAttribute<EffectConditionAttribute>();
                _conditionTypes.Add(attribute.ConditionType, type);
            }
        }

        public static EffectCondition GetCondition(EffectConditions condition, string target, float value)
        {
            return Activator.CreateInstance(_conditionTypes[condition], target, value) as EffectCondition;
        }
    }
    
    
    [EffectCondition(EffectConditions.LessThan)]
    public class LessThanCondition : EffectCondition
    {
        private StatParameter _statParameter;
        
        public LessThanCondition(string target, float value) : base(target, value)
        {
            StatParameter.ParseStatParam(target, out _statParameter);
        }

        public override bool CheckCondition(StatComponent statComponent)
        {
            return StatParameter.GetStatValue(statComponent, _statParameter) < _value;
        }
    }
    
    [EffectCondition(EffectConditions.GreaterThan)]
    public class GreaterThanCondition : EffectCondition
    {
        private StatParameter _statParameter;
        
        public GreaterThanCondition(string target, float value) : base(target, value)
        {
            StatParameter.ParseStatParam(target, out _statParameter);
        }

        public override bool CheckCondition(StatComponent statComponent)
        {
            return StatParameter.GetStatValue(statComponent, _statParameter) > _value;
        }
    }
    
    [EffectCondition(EffectConditions.Equal)]
    public class EqualCondition : EffectCondition
    {
        private StatParameter _statParameter;
        
        public EqualCondition(string target, float value) : base(target, value)
        {
            StatParameter.ParseStatParam(target, out _statParameter);
        }
        
        public override bool CheckCondition(StatComponent statComponent)
        {
            return _value.CompareTo(StatParameter.GetStatValue(statComponent, _statParameter)) == 0;
        }
    }
    
    [EffectCondition(EffectConditions.Random)]
    public class RandomCondition : EffectCondition
    {
        public RandomCondition(string target, float value) : base(target, value)
        {
        }
        
        public override bool CheckCondition(StatComponent statComponent)
        {
            return UnityEngine.Random.Range(0, 100) < _value;
        }
    }
    
}
