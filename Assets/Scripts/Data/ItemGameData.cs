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
        
        public string Name { get; set;}
        public string Desc { get; set;}
        public string PlusDesc { get; set;}
        public string MinusDesc { get; set;}
        
        public GradeTypes GradeType { get; private set;}
        public int MaxStack { get; private set;}
        
        public int CostGold { get; private set;}
        public int ItemEffectDataId { get; private set;}
        public string ItemIconPath { get; private set;}
    }


    [GameDataBase(typeof(ItemGameData), "ItemGameData")]
    public class ItemGameDataBase : IGameDataBase
    {
        private const string NameFormat = "Item_Name_{0}";
        private const string DescFormat = "Item_Desc_{0}";
        private const string PlusDescFormat = "Item_PlusDesc_{0}";
        private const string MinusDescFormat = "Item_MinusDesc_{0}";
        
        private const int MaxIteration = 1000;
        
        private readonly Dictionary<ItemGameData.GradeTypes, List<ItemGameData>> _itemGradeDictionary = new();
        private readonly Dictionary<int, ItemGameData> _datas = new();
        private int _totalItemCount = 0;

        public void RegisterData(IGameData data)
        {
            var itemData = (ItemGameData)data;
            _datas.Add(data.Index, itemData);
            
            
            if(!_itemGradeDictionary.TryGetValue(itemData.GradeType, out var list))
            {
                _itemGradeDictionary.Add(itemData.GradeType, list = new List<ItemGameData>());
            }

            list.Add(itemData);
            _totalItemCount++;
        }

        public void OnInitialize(GameDataManager manager)
        {
            var localeDataBase = manager.GetDataBase<LocaleGameDataBase>(); 
            
            localeDataBase.OnLocaleChanged.AddListener(OnLocaleChanged); 
            OnLocaleChanged(localeDataBase);
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

        public List<ItemGameData> GetItemsWithDropPercentage(DropPercentage percentage, int count, Inventory inventory, List<ItemGameData> holder)
        {
            var result = new List<ItemGameData>();

            if (_totalItemCount < inventory.GetItemCount() + count)
            {
                count = _totalItemCount - inventory.GetItemCount();
            }
            
            for (int i = 0; result.Count < count && i < MaxIteration; i++)
            {
                var item = PickRandom(percentage.RandomGradeType());
                
                if (item != null && !result.Contains(item) && !inventory.Contains(item.Index) && !holder.Contains(item))
                {
                    result.Add(item);
                }
            }

            return result;
        }
        
        
        private void OnLocaleChanged(LocaleGameDataBase localeData)
        {
            foreach (var data in _datas.Values)
            {
                data.Name = string.IsNullOrWhiteSpace(data.Name) ? data.Name : localeData.GetString(string.Format(NameFormat, data.Index));
                data.Desc = string.IsNullOrWhiteSpace(data.Desc) ? data.Desc : localeData.GetString(string.Format(DescFormat, data.Index));
                data.PlusDesc = string.IsNullOrWhiteSpace(data.PlusDesc) ? data.PlusDesc : localeData.GetString(string.Format(PlusDescFormat, data.Index));
                data.MinusDesc = string.IsNullOrWhiteSpace(data.MinusDesc) ? data.MinusDesc : localeData.GetString(string.Format(MinusDescFormat, data.Index));
            }
        }
        
    }  
}