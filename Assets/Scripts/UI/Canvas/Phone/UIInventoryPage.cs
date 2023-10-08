using System;
using System.Collections;
using QT.Core;
using QT.InGame;
using UnityEngine;
using UnityEngine.UI;

namespace QT.UI
{
    public class UIInventoryPage : MonoBehaviour
    {
        [SerializeField] private Transform _itemListParents;
        
        [SerializeField] private UIItemDesc _desc;
        
        [Header("액티브 설명")]
        [SerializeField] private GameObject _activeItemArea;
        [SerializeField] private UIItemDesc _activeDesc;
        [SerializeField] private Image _activeItemImage;
        
        private UIInventoryItem[] _itemFrames;

        public void Initialize()
        {
            _itemFrames = _itemListParents.GetComponentsInChildren<UIInventoryItem>();
            _activeItemArea.SetActive(false);
            _desc.Hide();
        }

        private void OnDisable()
        {
            _desc.Hide();
        }

        public void SetInventoryUI()
        {
            var inventory = SystemManager.Instance.PlayerManager.Player.Inventory;
            
            var items = inventory.GetItemList();

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

            if (inventory.ActiveItem != null)
            {
                _activeItemArea.SetActive(true);
                SetActiveDesc(inventory.ActiveItem.ItemGameData);
            }
            else
            {
                _activeItemArea.SetActive(false);
            }
        }

        private async void SetActiveDesc(ItemGameData itemData)
        {
            _activeItemArea.SetActive(true);
            _activeDesc.SetData(itemData);

            _activeItemImage.sprite =
                await SystemManager.Instance.ResourceManager.LoadAsset<Sprite>(itemData.ItemIconPath, true);
        }
        
        
        private void OnClickItem(UIInventoryItem item)
        {
            if (item.ItemGameData != null)
            {
                _desc.transform.position = item.transform.position;
                _desc.SetData(item.ItemGameData);
                _desc.Show();
            }
            else
            {
                _desc.Hide();
            }
        }
    }
}
