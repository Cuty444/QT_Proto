using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;


namespace QT.InGame
{
    public enum ItemEffectTypes
    {
        None,
        
        [ItemEffect(typeof(BuffItemEffect))]
        Buff,
        
        [ItemEffect(typeof(EnemyRigidItemEffect))]
        EnemyRigid,
        [ItemEffect(typeof(ReverseAtkDirItemEffect))]
        ReverseAtkDir,
        [ItemEffect(typeof(TimeScaleItemEffect))]
        TimeScale,
        
        [ItemEffect(typeof(TeleportItemEffect))]
        Teleport,
        [ItemEffect(typeof(AbsorbItemEffect))]
        Absorb,
        [ItemEffect(typeof(ChargeItemEffect))]
        Charge,
    }
    
    [AttributeUsage(AttributeTargets.Field)]
    public class ItemEffectAttribute : Attribute
    {
        public Type EffectType { get; private set; }

        public ItemEffectAttribute(Type effectType)
        {
            EffectType = effectType;
        }
    }
    
    public static class ItemEffectFactory
    {
        private static readonly Dictionary<ItemEffectTypes, Type> _effectTypes = new ();

        static ItemEffectFactory()
        {
            foreach (var field in typeof(ItemEffectTypes).GetFields())
            {
                var attribute = field.GetCustomAttribute<ItemEffectAttribute>();
                if (attribute != null)
                {
                    _effectTypes.Add((ItemEffectTypes)field.GetValue(null), attribute.EffectType);
                }
            }
        }

        public static ItemEffect GetEffect(ItemEffectTypes type, Player player, ItemEffectGameData effectData, SpecialEffectGameData specialEffectData = null)
        {
            return Activator.CreateInstance(_effectTypes[type], player, effectData, specialEffectData) as ItemEffect;
        }
    }
    
    
    public abstract class ItemEffect
    {
        public readonly ItemEffectGameData Data;
        private readonly EffectCondition _condition;
        private readonly StatComponent _ownerStatComponent;

        protected float _lastTime;

        public ItemEffect(Player player, ItemEffectGameData effectData, SpecialEffectGameData specialEffectData)
        {
            Data = effectData;
            _ownerStatComponent = player.StatComponent;
            
            if (effectData.Condition != EffectConditions.None)
            {
                _condition = EffectConditionFactory.GetCondition(effectData.Condition, effectData.ConditionTarget, effectData.ConditionValue);
            }
        }

        public void OnTrigger()
        {
            if (Time.timeSinceLevelLoad - _lastTime < Data.CoolTime)
            {
                return;
            }
            
            if (_condition == null || _condition.CheckCondition(_ownerStatComponent))
            {
                OnTriggerAction();
                _lastTime = Time.timeSinceLevelLoad;
            }
            
        }

        public abstract void OnEquip();
        
        protected abstract void OnTriggerAction();
        
        public abstract void OnRemoved();
    }
}
