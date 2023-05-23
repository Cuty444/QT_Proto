using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QT.InGame
{
    public class Inventory
    {
        private Player _targetPlayer;
        private List<Item> _items = new List<Item>();

        public Inventory(Player target)
        {
            _targetPlayer = target;
        }
        
        public void AddItem(int itemDataId)
        {
            var item = new Item(itemDataId);

            AddItem(item);
        }

        public void AddItem(Item item)
        {
            _items.Add(item);
            
            item.ApplyItemEffect(_targetPlayer);
        }
        
        public void RemoveItem(int index)
        {
            if (index < 0 || index >= _items.Count)
            {
                return;
            }
            
            _items[index].RemoveItemEffect(_targetPlayer);
            _items.RemoveAt(index);
        }

        private void SetApplyPoint()
        {
            
        }

        public Item[] GetItemList()
        {
            var result = new Item[_items.Count];
            _items.CopyTo(result);
            
            return result;
        }
    }
}
