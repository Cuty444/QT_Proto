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
        
        [ItemEffect(typeof(ReverseAtkDirItemEffect))]
        ReverseAtkDir,
        [ItemEffect(typeof(TimeScaleItemEffect))]
        TimeScale,
        [ItemEffect(typeof(ShootItemEffect))]
        Shoot,
        [ItemEffect(typeof(HealItemEffect))]
        Heal,
        [ItemEffect(typeof(AddGoldItemEffect))]
        AddGold,

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
        public ItemEffect(Player player, ItemEffectGameData effectData, SpecialEffectGameData specialEffectData)
        {
        }

        public abstract void OnEquip();
        
        public abstract void OnTrigger(bool success);
        
        public abstract void OnRemoved();
    }
}
