using System;
using System.Collections;
using QT.Core;
using UnityEngine;

namespace QT.UI
{
    public class UIInventoryPage : MonoBehaviour
    {
        [SerializeField] private Transform _itemListParents;
        [SerializeField] private UIItemDesc _desc;
        
        private UIInventoryItem[] _itemFrames;

        public void Initialize()
        {
            _itemFrames = _itemListParents.GetComponentsInChildren<UIInventoryItem>();
            _desc.Hide();
        }

        private void OnDisable()
        {
            _desc.Hide();
        }

        public void SetInventoryUI()
        {
            var items = SystemManager.Instance.PlayerManager.Player.Inventory.GetItemList();

            for (int i = 0; i < _itemFrames.Length; i++)
            {
                if (i < items.Length)
                {
                    var itemData = items[i].ItemGameData;
                    
                    _itemFrames[i].SetItem(i, itemData);
                }
                else
                {
                    _itemFrames[i].ClearItem();
                }
                
                _itemFrames[i].OnClick = OnClickItem;
            }
        }

        private void OnClickItem(UIInventoryItem item)
        {
            if (item.ItemGameData != null)
            {
                _desc.Show(item);
            }
            else
            {
                _desc.Hide();
            }
        }
    }
}
