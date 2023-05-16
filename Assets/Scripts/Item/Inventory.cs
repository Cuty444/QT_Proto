using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QT
{
    public class Inventory
    {
        private InGame.Player _targetPlayer;
        private List<Item> _items = new List<Item>();

        public void AddItem(int itemDataId, int index)
        {
            var item = new Item(itemDataId);
            
            _items.Add(item);
        }
        
        public void RemoveItem(int index)
        {
            
        }
    }
}
