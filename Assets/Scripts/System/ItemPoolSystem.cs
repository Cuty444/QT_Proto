using System;
using System.Collections;
using System.Collections.Generic;
using QT.Core;
using QT.Core.Map;
using QT.InGame;
using UnityEngine;
using UnityEngine.Events;

namespace QT
{
    public class ItemPoolSystem : SystemBase
    {
        private List<ItemGameData> _holderItems = new List<ItemGameData>(); // 전시된 아이템 리스트

        public UnityEvent<List<ItemGameData>> HolderItemCreatedEvent { get; } = new();

        private DropGameDataBase _dropGameDataBase;
        private ItemGameDataBase _itemGameDataBase;


        public override void OnInitialized()
        {
            base.OnInitialized();
            _dropGameDataBase = SystemManager.Instance.DataManager.GetDataBase<DropGameDataBase>();
            _itemGameDataBase = SystemManager.Instance.DataManager.GetDataBase<ItemGameDataBase>();
            HolderItemCreatedEvent.AddListener(HolderAddRange);
        }

        public override void OnPostInitialized()
        {
            SystemManager.Instance.GetSystem<DungeonMapSystem>().DungeonMapGeneratedEvent.AddListener(HolderItemClear);
        }

        private void OnDestroy()
        {
            HolderItemCreatedEvent.RemoveListener(HolderAddRange);
            //SystemManager.Instance.GetSystem<DungeonMapSystem>().DungeonMapGeneratedEvent.RemoveListener(HolderItemClear);
        }

        public List<ItemGameData> GetItemsWithDropPercentage(int count,DropGameType gameType)
        {
            var percent = _dropGameDataBase.GetData((int)gameType);

            var items = _itemGameDataBase.GetItemsWithDropPercentage(percent, count,
                SystemManager.Instance.PlayerManager.Player.Inventory,_holderItems);

            return items;
        }

        private void HolderAddRange(List<ItemGameData> items)
        {
            _holderItems.AddRange(items);
        }

        public void HolderItemRemover(ItemGameData item)
        {
            _holderItems.Remove(item);
        }
        
        private void HolderItemClear()
        {
            _holderItems.Clear();
        }
        
    }
}
