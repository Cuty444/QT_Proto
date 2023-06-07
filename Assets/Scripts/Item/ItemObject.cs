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
        public DropGameType DropType;
        [SerializeField] private GameObject _itemScriptCanvas;
        [SerializeField] private SpriteRenderer _itemSprite;
        [SerializeField] private TextMeshProUGUI _itemName;
        [SerializeField] private TextMeshProUGUI _itemDesc;
        [SerializeField] private TextMeshProUGUI _itemCost;
        [SerializeField] private Transform _goldTransform;
        [SerializeField] private Transform _hpTransform;
        [SerializeField] private Image[] _hpImages;
        

        private PlayerManager _playerManager;
        public ItemGameData ItemGameData { get; private set; }
        public List<ItemEffect> ItemEffectData { get; private set; } = new ();

        private void Start()
        {
            _playerManager = SystemManager.Instance.PlayerManager;
            var dataManager = SystemManager.Instance.DataManager;

            if (DropType == DropGameType.GoldShop || DropType == DropGameType.HpShop)
            {
                ItemGameData = dataManager.GetDataBase<ItemGameDataBase>().GetData(ItemID);
                if (DropType == DropGameType.GoldShop)
                {
                    _goldTransform.gameObject.SetActive(true);
                    _hpTransform.gameObject.SetActive(false);
                }
                else
                {
                    _goldTransform.gameObject.SetActive(false);
                    _hpTransform.gameObject.SetActive(true);
                }
                
            }
            else if (DropType == DropGameType.Boss || DropType == DropGameType.ItemReward ||
                     DropType == DropGameType.Select)
            {
                var list = SystemManager.Instance.ItemDataManager.GetDropItemList(DropType, 1);
                ItemGameData = dataManager.GetDataBase<ItemGameDataBase>().GetData(list[0]);
            }
            if (ItemGameData != null)
            {
                ItemEffectData = dataManager.GetDataBase<ItemEffectGameDataBase>()
                    .GetData(ItemGameData.ItemEffectDataId);
            }

            ResourceManager resourceManager = SystemManager.Instance.ResourceManager;
            resourceManager.LoadSprite(ItemGameData.ItemIconPath, _itemSprite);
            _itemName.text = ItemGameData.Name;
            _itemCost.text = ItemGameData.CostGold;
            _itemDesc.text = ItemGameData.Desc.Replace("24","17");
            for (int i = 25; i <= ItemGameData.CostHp; i += 25)
            {
                _hpImages[i/25].gameObject.SetActive(true);
            }
            LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)_itemScriptCanvas.transform);
            LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)_itemName.transform);
            LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)_itemDesc.transform);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                _itemScriptCanvas.gameObject.SetActive(true);
                if (DropType == DropGameType.GoldShop || DropType == DropGameType.HpShop)
                {
                    _playerManager.PlayerItemInteraction.AddListener(ItemBuy);
                }
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                _itemScriptCanvas.gameObject.SetActive(false);
                if (DropType == DropGameType.GoldShop || DropType == DropGameType.HpShop)
                {
                    _playerManager.PlayerItemInteraction.RemoveListener(ItemBuy);
                }
            }
        }

        public void ItemBuy()
        {
            if (DropType == DropGameType.GoldShop)
            {
                if (!_playerManager.Player.GetGoldComparison(ItemGameData.CostGold))
                    return;
                _playerManager.OnGoldValueChanged.Invoke(_playerManager.Player.GetGoldCost() - ItemGameData.CostGold);
            }
            else if(DropType == DropGameType.HpShop)
            {
                if (!_playerManager.Player.GetHpComparision(ItemGameData.CostHp))
                    return;
                _playerManager.OnDamageEvent.Invoke(Vector2.zero, ItemGameData.CostHp);
            }

            _playerManager.Player.Inventory.AddItem(ItemID);
            Destroy(gameObject);
        }
    }
}
