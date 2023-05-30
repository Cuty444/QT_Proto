using System;
using System.Collections;
using System.Collections.Generic;
using QT.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace QT
{
    public class ItemObject : MonoBehaviour
    {
        public int ItemID;
        [SerializeField] private GameObject _itemScriptCanvas;
        [SerializeField] private SpriteRenderer _itemSprite;
        [SerializeField] private TextMeshProUGUI _itemName;
        [SerializeField] private TextMeshProUGUI _itemCost;
        [SerializeField] private TextMeshProUGUI _itemEffect;

        private PlayerManager _playerManager;
        public ItemGameData ItemGameData { get; private set; }
        public List<ItemEffect> ItemEffectData { get; private set; } = new ();

        private void Start()
        {
            _playerManager = SystemManager.Instance.PlayerManager;
            var dataManager = SystemManager.Instance.DataManager;

            ItemGameData = dataManager.GetDataBase<ItemGameDataBase>().GetData(ItemID);
            if (ItemGameData != null)
            {
                ItemEffectData = dataManager.GetDataBase<ItemEffectGameDataBase>()
                    .GetData(ItemGameData.ItemEffectDataId);
            }
            
            SystemManager.Instance.ResourceManager.LoadSprite(ItemGameData.ItemIconPath, _itemSprite);
            _itemName.text = ItemGameData.Name;
            _itemCost.text = "가격 : " + ItemGameData.CostGold;
            _itemEffect.text = ItemGameData.Desc;
            LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)_itemScriptCanvas.transform);

        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                _itemScriptCanvas.gameObject.SetActive(true);
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                _itemScriptCanvas.gameObject.SetActive(false);
            }
        }

        public void ItemBuy()
        {
            if (!_playerManager.Player.GetGoldComparison(ItemGameData.CostGold))
                return;
            
            _playerManager.Player.Inventory.AddItem(ItemID);
            _playerManager.GoldValueChanged.Invoke(_playerManager.Player.GetGoldCost() - ItemGameData.CostGold);
            Destroy(gameObject);
        }
    }
}
