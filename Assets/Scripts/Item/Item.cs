using System.Collections;
using System.Collections.Generic;
using QT.Core;
using UnityEngine;

namespace QT
{
    public class Item
    {
        private readonly int _itemDataId;
        public ItemGameData ItemGameData { get; private set; }
        public List<ItemEffect> ItemEffectData { get; private set; }
        
        public Item(int itemDataId)
        {
            var dataManager = SystemManager.Instance.DataManager;
            
            _itemDataId = itemDataId;
            ItemGameData = dataManager.GetDataBase<ItemGameDataBase>().GetData(_itemDataId);
            ItemEffectData = dataManager.GetDataBase<ItemEffectGameDataBase>().GetData(ItemGameData.ItemEffectDataId);
        }
    }
}
