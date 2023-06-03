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
            Hp,
            Gold,
            Weapon,
        }
        
        public int Index { get; set; }
        
        public string Name { get; set; }
        public string Desc { get; set; }
        public GradeTypes GradeType { get; set; }
        public int Value { get; set; }
        public int CostHp { get; set; }
        public int CostGold { get; set; }
        public int ItemEffectDataId { get; set; }
        public string ItemIconPath { get; set; }
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