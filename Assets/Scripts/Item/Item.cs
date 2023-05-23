using System.Collections;
using System.Collections.Generic;
using QT.Core;
using QT.InGame;
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
            if (ItemGameData != null)
            {
                ItemEffectData = dataManager.GetDataBase<ItemEffectGameDataBase>()
                    .GetData(ItemGameData.ItemEffectDataId);
            }
        }

        public void ApplyItemEffect(Player player)
        {
            foreach (var effect in ItemEffectData)
            {
                effect.ApplyEffect(player, this);
            }
        }

        public void RemoveItemEffect(Player player)
        {
            foreach (var effect in ItemEffectData)
            {
                effect.RemoveEffect(player, this);
            }
        }
    }
}
