using System;
using System.Collections.Generic;
using QT.Core;
using QT.InGame;

namespace QT
{
    public class ItemEffectGameData : IGameData
    {
        public int Index { get; set; }
        
        public int ApplyBuffId { get; set; }
        public int ApplySpecialEffectId { get; set; }
        
        public float CoolTime { get; set; }
        
        public TriggerTypes TriggerType { get; set; }
        
        public  string ConditionTarget { get; set; }
        public EffectConditions Condition { get; set; }
        public float ConditionValue { get; set; }
    }


    [GameDataBase(typeof(ItemEffectGameData), "ItemEffectGameData")]
    public class ItemEffectGameDataBase : IGameDataBase
    {
        private readonly Dictionary<int, List<ItemEffectGameData>> _datas = new();

        public void RegisterData(IGameData data)
        {
            if(!_datas.TryGetValue(data.Index, out var list))
            {
                _datas.Add(data.Index, list = new List<ItemEffectGameData>());
            }
            
            list.Add(data as ItemEffectGameData);
        }

        public void OnInitialize(GameDataManager manager)
        {
            
        }
        
        public List<ItemEffectGameData> GetData(int id)
        {
            if (_datas.TryGetValue(id, out var value))
            {
                return value;
            }

            return new List<ItemEffectGameData>();
        }
    }  
}