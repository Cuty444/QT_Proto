// using System;
// using System.Collections.Generic;
// using System.Data;
// using QT.InGame;
// using UnityEngine;
// using ApplyTypes = QT.ItemEffectGameData.ApplyTypes;
//
// namespace QT
// {
//     public class ItemEffectReverse : ItemEffectOld
//     {
//         public override ApplyTypes ApplyType => ApplyTypes.Reverse;
//         
//         public ItemEffectReverse(ItemEffectGameData effectData) : base(effectData)
//         {
//         }
//
//         protected override bool Process(ItemEffectGameData effectData)
//         {
//             return true;
//         }
//
//         public override void ApplyEffect(Player player, object source)
//         {
//             player.IsReverseLookDir = true;
//         }
//
//         public override void RemoveEffect(Player player, object source)
//         {
//             player.IsReverseLookDir = false;
//         }
//     }
// }
