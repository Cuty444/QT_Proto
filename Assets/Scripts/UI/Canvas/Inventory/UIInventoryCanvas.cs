using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using QT.Core;
using QT.Map;
using QT.Core.Map;
using QT.InGame;
using TMPro;
using UnityEngine;

namespace QT.UI
{
    public class UIInventoryCanvas : UIPanel
    {
        [SerializeField] private GameObject _inventoryGameobject;
        [SerializeField] private Transform _itemListParents;

        [SerializeField] private TextMeshProUGUI _itemName;
        [SerializeField] private TextMeshProUGUI _itemDesc;
        
        private UIInventoryItem[] _itemFrames;
        
        public override void PostSystemInitialize()
        {
            _itemFrames = _itemListParents.GetComponentsInChildren<UIInventoryItem>();
            gameObject.SetActive(true);
        }

        private void Update()
        {
            CheckInput();
        }

        private void CheckInput()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                _inventoryGameobject.SetActive(!_inventoryGameobject.activeInHierarchy);
                if (_inventoryGameobject.activeInHierarchy)
                {
                    SetInventoryUI();
                }
            }
        }

        private void SetInventoryUI()
        {
            var items = SystemManager.Instance.PlayerManager.Player.Inventory.GetItemList();

            for (int i = 0; i < _itemFrames.Length; i++)
            {
                _itemFrames[i].gameObject.SetActive(i < items.Length);
                
                if (i < items.Length)
                {
                    var itemData = items[i].ItemGameData;
                    var index = i;
                    _itemFrames[i].OnClick = () =>
                    {
                        _itemName.text = itemData.Name;
                        _itemDesc.text = itemData.Desc;
                    };
                }
            }
        }

    }
}
