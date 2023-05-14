using System;
using System.Collections;
using System.Collections.Generic;
using QT.Core;
using UnityEngine;
using QT.InGame;

using ApplyTypes = QT.ItemEffectGameData.ApplyTypes;
using ValueOperatorTypes = QT.ItemEffectGameData.ValueOperatorTypes;
using ApplyPoints = QT.ItemEffectGameData.ApplyPoints;

namespace QT
{
    public abstract class ItemEffect
    {
        public virtual ApplyTypes ApplyType => ApplyTypes.None;
        
        public ApplyPoints ApplyPoints { get; protected set; }
        public readonly bool IsAvailable = false;
        
        public ItemEffect(ItemEffectGameData effectData)
        {
            if(effectData == null || effectData.ApplyType != ApplyType)
            {
                return;
            }
            
            ApplyPoints = effectData.ApplyPoint;
            
            IsAvailable = Process(effectData);
        }

        protected abstract bool Process(ItemEffectGameData effectData);
        
        public abstract void ApplyEffect(Player player);
    }
    
    
    public class ItemEffectStat : ItemEffect
    {
        private enum StatValueType
        {
            Value,
            Base
        }
        
        private static readonly char[] seperator = {' ', '+', '-', '*', '/', '(', ')'};
        
        public override ApplyTypes ApplyType => ApplyTypes.PlayerStat;

        private PlayerStats _applyStat;
        private string _applyValue;
        private (PlayerStats, StatValueType)[] _param;
        private ValueOperatorTypes _valueOperatorType;
        
        public ItemEffectStat(ItemEffectGameData effectData) : base(effectData) { }
        
        protected override bool Process(ItemEffectGameData effectData)
        {
            if (!Enum.TryParse(effectData.ApplyStat, out _applyStat))
            {
                return false;
            }
            
            _applyValue = effectData.ApplyValue;

            
            var param = new List<(PlayerStats, StatValueType)>();

            foreach (var str in _applyValue.Split(seperator, StringSplitOptions.RemoveEmptyEntries))
            {
                var parts = str.Split('.');

                if (parts.Length <= 0 || parts.Length > 2)
                {
                    return false;
                }
                
                if (!Enum.TryParse(parts[0], out PlayerStats stat))
                {
                    return false;
                }

                StatValueType type = StatValueType.Value;
                
                if (parts.Length == 2)
                {
                    if (!Enum.TryParse(parts[1], out type))
                    {
                        return false;
                    }
                }
                
                param.Add((stat, type));
            }

            return true;
        }

        public override void ApplyEffect(Player player)
        {
            
        }
    }
}
