// using System;
// using System.Collections.Generic;
// using System.Data;
// using QT.InGame;
// using UnityEngine;
// using ApplyTypes = QT.ItemEffectGameData.ApplyTypes;
// using ModifierType = QT.StatModifier.ModifierType;
//
// namespace QT
// {
//     public class ItemEffectStat : ItemEffectOld
//     {
//         private enum StatValueType
//         {
//             Value,
//             Base
//         }
//         
//         public override ApplyTypes ApplyType => ApplyTypes.PlayerStat;
//
//         private PlayerStats _applyStat;
//         private string _applyValue;
//         private (PlayerStats, StatValueType)[] _param;
//         private ModifierType _valueOperatorType;
//
//         private StatModifier _modifier;
//         
//         public ItemEffectStat(ItemEffectGameData effectData) : base(effectData) { }
//
//         protected override bool Process(ItemEffectGameData effectData)
//         {
//             if (!Enum.TryParse(effectData.ApplyStat, out _applyStat))
//             {
//                 Debug.LogError($" {effectData.Index} : 아이템 이펙트 데이터 스탯을 찾을 수 없음 : {effectData.ApplyStat}");
//                 return false;
//             }
//
//             _valueOperatorType = effectData.ValueOperatorType;
//             
//             _applyValue = effectData.ApplyValue;
//             var paramString = ParseApplyValue(ref _applyValue);
//
//             var temp = new List<(PlayerStats, StatValueType)>();
//             
//             foreach (var str in paramString)
//             {
//                 var parts = str.Split('_');
//
//                 if (parts.Length <= 0 || parts.Length > 2)
//                 {
//                     Debug.LogError($" {effectData.Index} : 아이템 이펙트 데이터 수식 오류 : {effectData.ApplyValue} ({str})");
//                     return false;
//                 }
//                 
//                 if (!Enum.TryParse(parts[0], out PlayerStats stat))
//                 {
//                     Debug.LogError($" {effectData.Index} : 아이템 이펙트 데이터 수식 오류 : {effectData.ApplyValue} ({str})");
//                     return false;
//                 }
//
//                 StatValueType type = StatValueType.Value;
//                 
//                 if (parts.Length == 2)
//                 {
//                     if (!Enum.TryParse(parts[1], out type))
//                     {
//                         Debug.LogError($" {effectData.Index} : 아이템 이펙트 데이터 수식 오류 : {effectData.ApplyValue} ({str})");
//                         return false;
//                     }
//                 }
//                 
//                 temp.Add((stat, type));
//             }
//
//             _param = temp.ToArray();
//             
//             return true;
//         }
//
//
//         public override void ApplyEffect(Player player, object source)
//         {
//             var target = player.GetStat(_applyStat);
//
//             string expression = _applyValue;
//             
//             for (var i = 0; i < _param.Length; i++)
//             {
//                 var param = _param[i];
//                 float value = 0;
//                 
//                 Stat stat = player.GetStat(param.Item1);
//
//                 if(stat is Status)
//                 {
//                     value = param.Item2 == StatValueType.Base ? (stat as Status).Value : (stat as Status).StatusValue;
//                 }
//                 else
//                 {
//                     value =  stat;
//                 }
//
//                 expression = expression.Replace($"[{i}]", value.ToString());
//             }
//
//             var dt = new DataTable();
//             var result = Convert.ToSingle(dt.Compute(expression, null));
//             
//             target.AddModifier(new StatModifier(result, _valueOperatorType, source));
//         }
//
//         public override void RemoveEffect(Player player, object source)
//         {
//             Stat stat = player.GetStat(_applyStat);
//             stat.RemoveAllModifiersFromSource(source);
//         }
//     }
// }
