using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using QT.InGame;
using UnityEngine;
using ModifierType = QT.StatModifier.ModifierType;

namespace QT.InGame
{
    public class BuffCalculator
    {
        private enum StatValueType
        {
            Value,
            Base
        }
        
        public readonly bool IsAvailable = false;
        
        private PlayerStats _applyStat;
        private string _applyValue;
        private (PlayerStats, StatValueType)[] _params;
        private ModifierType _valueOperatorType;
        
        private StatModifier _modifier;

        public BuffCalculator(BuffEffectGameData effectData)
        {
            IsAvailable = Process(effectData);
        }
        
        private bool Process(BuffEffectGameData effectData)
        {
            if (!Enum.TryParse(effectData.ApplyStat, out _applyStat))
            {
                Debug.LogError($" {effectData.Index} : 버프 이펙트 데이터 스탯을 찾을 수 없음 : {effectData.ApplyStat}");
                return false;
            }

            _valueOperatorType = effectData.ValueOperatorType;
            
            _applyValue = effectData.ApplyValue;
            var paramString = ParseApplyValue(ref _applyValue);

            var temp = new List<(PlayerStats, StatValueType)>();
            
            foreach (var str in paramString)
            {
                var parts = str.Split('_');

                if (parts.Length <= 0 || parts.Length > 2)
                {
                    Debug.LogError($" {effectData.Index} : 아이템 이펙트 데이터 수식 오류 : {effectData.ApplyValue} ({str})");
                    return false;
                }
                
                if (!Enum.TryParse(parts[0], out PlayerStats stat))
                {
                    Debug.LogError($" {effectData.Index} : 아이템 이펙트 데이터 수식 오류 : {effectData.ApplyValue} ({str})");
                    return false;
                }

                StatValueType type = StatValueType.Value;
                
                if (parts.Length == 2)
                {
                    if (!Enum.TryParse(parts[1], out type))
                    {
                        Debug.LogError($" {effectData.Index} : 아이템 이펙트 데이터 수식 오류 : {effectData.ApplyValue} ({str})");
                        return false;
                    }
                }
                
                temp.Add((stat, type));
            }

            _params = temp.ToArray();
            
            return true;
        }


        public void ApplyEffect(StatComponent statComponent, object source)
        {
            var target = statComponent.GetStat(_applyStat);

            string expression = _applyValue;
            
            for (var i = 0; i < _params.Length; i++)
            {
                var param = _params[i];
                float value = 0;
                
                Stat stat = statComponent.GetStat(param.Item1);

                if(stat is Status)
                {
                    value = param.Item2 == StatValueType.Base ? (stat as Status).Value : (stat as Status).StatusValue;
                }
                else
                {
                    value =  stat;
                }

                expression = expression.Replace($"[{i}]", value.ToString());
            }

            var dt = new DataTable();
            var result = Convert.ToSingle(dt.Compute(expression, null));
            
            target.AddModifier(new StatModifier(result, _valueOperatorType, source));
        }

        public void RemoveEffect(StatComponent statComponent, object source)
        {
            Stat stat = statComponent.GetStat(_applyStat);
            stat.RemoveAllModifiersFromSource(source);
        }


        private static List<string> ParseApplyValue(ref string applyValue)
        {
            bool IsAlpha(char c) => c is >= 'a' and <= 'z' or >= 'A' and <= 'Z';
            bool IsNumber(char c) => c is >= '0' and <= '9';

            var param = new List<string>();

            var value = new StringBuilder(applyValue.Length);
            var temp = new StringBuilder(20);

            bool isLastCharIsAlpha = false;
            foreach (var c in applyValue)
            {
                var isAlpha = IsAlpha(c);

                if (!isAlpha && c != '_' && !(isLastCharIsAlpha && IsNumber(c)))
                {
                    if (temp.Length > 0)
                    {
                        param.Add(temp.ToString());
                        value.Append($"[{param.Count - 1}]");

                        temp.Clear();
                    }

                    value.Append(c);
                }
                else
                {
                    temp.Append(c);
                }

                isLastCharIsAlpha = isAlpha;
            }

            if (temp.Length > 0)
            {
                param.Add(temp.ToString());
                value.Append($"[{param.Count - 1}]");
            }

            applyValue = value.ToString();
            
            return param;
        }
    }
}
