using System.Collections.Generic;
using QT.Core;

namespace QT
{
    public class ItemEffectGameData : IGameData
    { 
        public enum ApplyTypes
        {
            ResourceChange,
            Stat,
        }
        
        public enum ValueOperatorTypes
        {
            None,
            Hard,
            Addition,
            Multiply
        }
        
        public enum ApplyPoints
        {
            Equip,
            OnCharging,
            GoldChanged,
            HpChanged,
            OnSwing
        }
        
        public int Index { get; set; }
        
        public ApplyTypes ApplyType { get; set; }
        public string ApplyValue { get; set; }
        public ValueOperatorTypes ValueOperatorType { get; set; }
        public ApplyPoints ApplyPoint { get; set; }
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
            
            list.Add((ItemEffectGameData)data);
        }

        public List<ItemEffectGameData> GetData(int id)
        {
            if (_datas.TryGetValue(id, out var value))
            {
                return value;
            }

            return null;
        }
    }  
}