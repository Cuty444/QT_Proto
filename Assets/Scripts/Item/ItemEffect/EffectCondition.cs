using System;
using System.Collections.Generic;
using System.Reflection;
using QT.Core;
using UnityEngine;

namespace QT.InGame
{
    public enum EffectConditions
    {
        None,
        
        [EffectCondition(typeof(LessThanCondition))]
        LessThan,
        [EffectCondition(typeof(GreaterThanCondition))]
        GreaterThan,
        [EffectCondition(typeof(EqualCondition))]
        Equal,
        
        [EffectCondition(typeof(RandomCondition))]
        Random,
        
        [EffectCondition(typeof(ProjectileInRange))]
        ProjectileInRange,
        [EffectCondition(typeof(HitAbleInRange))]
        HitAbleInRange
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class EffectConditionAttribute : Attribute
    {
        public Type ConditionType { get; private set; }

        public EffectConditionAttribute(Type conditionType)
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
            foreach (var field in typeof(EffectConditions).GetFields())
            {
                var attribute = field.GetCustomAttribute<EffectConditionAttribute>();
                if (attribute != null)
                {
                    _conditionTypes.Add((EffectConditions)field.GetValue(null), attribute.ConditionType);
                }
            }
        }

        public static EffectCondition GetCondition(EffectConditions condition, string target, float value)
        {
            return Activator.CreateInstance(_conditionTypes[condition], target, value) as EffectCondition;
        }
    }
    
    
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
    
    public class ProjectileInRange : EffectCondition
    {
        private readonly LayerMask _bounceMask;
        public ProjectileInRange(string target, float value) : base(target, value)
        {
            _bounceMask = LayerMask.GetMask(target.Split(','));
        }
        
        public override bool CheckCondition(StatComponent statComponent)
        {
            var playerPos = SystemManager.Instance.PlayerManager.Player.transform.position;
            var list = new List<IProjectile>();

            ProjectileManager.Instance.GetInRange(playerPos, _value, ref list, _bounceMask);

            return list.Count > 0;
        }
    }
    
    public class HitAbleInRange : EffectCondition
    {
        public HitAbleInRange(string target, float value) : base(target, value)
        {
        }
        
        public override bool CheckCondition(StatComponent statComponent)
        {
            var playerPos = SystemManager.Instance.PlayerManager.Player.transform.position;
            var list = new List<IHitAble>();
            
            HitAbleManager.Instance.GetInRange(playerPos, _value, ref list);
            
            return list.Count > 0;
        }
    }
    
}
