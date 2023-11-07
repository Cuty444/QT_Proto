using System.Collections;
using System.Collections.Generic;
using QT.Core;
using QT.InGame;
using UnityEngine;
using UnityEngine.Events;

namespace QT
{
    public class ItemPoolSystem
    {
        private List<ItemGameData> _holderItems; // 전시된 아이템 리스트

        public UnityEvent<List<ItemGameData>> HolderItemCreatedEvent = new UnityEvent<List<ItemGameData>>();

        private DropGameDataBase _dropGameDataBase;
        private ItemGameDataBase _itemGameDataBase;
        
        private void Awake()
        {
            _dropGameDataBase = SystemManager.Instance.DataManager.GetDataBase<DropGameDataBase>();
            _itemGameDataBase = SystemManager.Instance.DataManager.GetDataBase<ItemGameDataBase>();
            HolderItemCreatedEvent.AddListener(HolderAddRange);
        }

        private void OnDestroy()
        {
            HolderItemCreatedEvent.RemoveListener(HolderAddRange);
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
        
    }
}
