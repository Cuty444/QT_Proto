using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using UnityEngine;
using ModifierType = QT.StatModifier.ModifierType;


namespace QT.InGame
{
    public class BuffCalculator
    {
        public readonly bool IsAvailable = false;
        public readonly float Duration;
        
        private PlayerStats _applyStat;
        private string _applyValue;
        private StatParameter[] _params;
        private ModifierType _valueOperatorType;
        
        private StatModifier _modifier;

        public BuffCalculator(BuffEffectGameData effectData)
        {
            Duration = effectData.Duration;
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

            var temp = new List<StatParameter>();
            
            foreach (var str in paramString)
            {
                if (!StatParameter.ParseStatParam(str, out var param))
                {
                    Debug.LogError($" {effectData.Index} : 아이템 이펙트 데이터 수식 오류 : {effectData.ApplyValue} ({str})");
                    return false;
                }
                temp.Add(param);
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
                Stat stat = statComponent.GetStat(param.Stat);

                float value = StatParameter.GetStatValue(stat, param.Type);

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


        // 스탯 파라미터와 수식을 분리
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
