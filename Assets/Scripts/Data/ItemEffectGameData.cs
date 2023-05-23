using System.Collections.Generic;
using QT.Core;
using UnityEngine;

namespace QT
{
    public class ItemEffectGameData : IGameData
    { 
        public enum ApplyTypes
        {
            None,
            ResourceChange,
            PlayerStat,
        }

        public enum ApplyPoints
        {
            Equip,
            OnCharging,
            OnGoldChanged,
            OnHpChanged,
            OnSwing
        }
        
        public int Index { get; set; }
        
        public ApplyTypes ApplyType { get; set; }
        
        public string ApplyStat { get; set; }
        public string ApplyValue { get; set; }
        
        public StatModifier.ModifierType ValueOperatorType { get; set; }
        public ApplyPoints ApplyPoint { get; set; }
    }


    [GameDataBase(typeof(ItemEffectGameData), "ItemEffectGameData")]
    public class ItemEffectGameDataBase : IGameDataBase
    {
        private readonly Dictionary<int, List<ItemEffect>> _datas = new();

        public void RegisterData(IGameData data)
        {
            ItemEffect effect;
            
            var effectData = data as ItemEffectGameData;
            switch (effectData.ApplyType)
            {
                case ItemEffectGameData.ApplyTypes.PlayerStat:
                    effect = new ItemEffectStat(effectData);
                    break;
                default:
                    return;
            }
            
            if(!effect.IsAvailable)
            {
                Debug.LogError($"아이템 이펙트 데이터 오류 : {effectData.Index}");
                return;
            }
            
            if(!_datas.TryGetValue(data.Index, out var list))
            {
                _datas.Add(data.Index, list = new List<ItemEffect>());
            }
            
            list.Add(effect);
        }

        public List<ItemEffect> GetData(int id)
        {
            if (_datas.TryGetValue(id, out var value))
            {
                return value;
            }

            return null;
        }
    }  
}