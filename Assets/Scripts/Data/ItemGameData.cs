using System.Collections.Generic;
using QT.Core;

namespace QT
{
    public class ItemGameData : IGameData
    {
        public enum GradeTypes
        {
            Normal,
            Rare,
            Cursed,
            Active,
            Hp,
            Gold,
            
            Max
        }
        
        public int Index { get; }
        
        public string Name { get; }
        public string Desc { get; }
        public GradeTypes GradeType { get; }
        public int Value { get; }
        public int CostHp { get; }
        public int CostGold { get; }
        public int ItemEffectDataId { get; }
        public string ItemIconPath { get; }
    }


    [GameDataBase(typeof(ItemGameData), "ItemGameData")]
    public class ItemGameDataBase : IGameDataBase
    {
        private readonly Dictionary<int, ItemGameData> _datas = new();

        public void RegisterData(IGameData data)
        {
            _datas.Add(data.Index, (ItemGameData)data);
        }

        public ItemGameData GetData(int id)
        {
            if (_datas.TryGetValue(id, out var value))
            {
                return value;
            }

            return null;
        }
    }  
}