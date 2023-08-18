using System.Collections.Generic;
using System.Linq;
using QT.Core;
using QT.InGame;
using QT.Util;
using UnityEngine;

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
        
        public int Index { get; private set;}
        
        public string Name { get; private set;}
        public string Desc { get; private set;}
        public string PlusDesc { get; private set;}
        public string MinusDesc { get; private set;}
        
        public GradeTypes GradeType { get; private set;}
        public int Value { get; private set;}
        public int CostHp { get; private set;}
        public int CostGold { get; private set;}
        public int ItemEffectDataId { get; private set;}
        public string ItemIconPath { get; private set;}
    }


    [GameDataBase(typeof(ItemGameData), "ItemGameData")]
    public class ItemGameDataBase : IGameDataBase
    {
        private const int MaxIteration = 1000;
        
        private readonly Dictionary<ItemGameData.GradeTypes, List<ItemGameData>> _itemGradeDictionary = new();
        private readonly Dictionary<int, ItemGameData> _datas = new();

        public void RegisterData(IGameData data)
        {
            var itemData = (ItemGameData)data;
            
            _datas.Add(data.Index, itemData);
            
            
            if(!_itemGradeDictionary.TryGetValue(itemData.GradeType, out var list))
            {
                _itemGradeDictionary.Add(itemData.GradeType, list = new List<ItemGameData>());
            }

            list.Add(itemData);
        }

        public ItemGameData GetData(int id)
        {
            if (_datas.TryGetValue(id, out var value))
            {
                return value;
            }

            return null;
        }
        
        public ItemGameData[] GetGradeDatas(ItemGameData.GradeTypes gradeType)
        {
            if (_itemGradeDictionary.TryGetValue(gradeType, out var value))
            {
                return value.ToArray();
            }

            return null;
        }

        public ItemGameData PickRandom(ItemGameData.GradeTypes gradeType)
        {
            if (_itemGradeDictionary.TryGetValue(gradeType, out var list))
            {
                if (list.Count > 0)
                {
                    return list[Random.Range(0, list.Count)];
                }
            }

            return null;
        }

        public List<ItemGameData> GetItemsWithDropPercentage(DropPercentage percentage, int count, Inventory inventory)
        {
            var result = new List<ItemGameData>();

            for (int i = 0; result.Count < count && i < MaxIteration; i++)
            {
                var item = PickRandom(percentage.RandomGradeType());
                
                if (item != null && !result.Contains(item) && !inventory.Contains(item.Index))
                {
                    result.Add(item);
                }
            }

            return result;
        }
        
    }  
}