using System;
using System.Collections;
using System.Collections.Generic;
using QT.Core;
using UnityEngine;
using UnityEngine.Events;

namespace QT
{
    public class ShopMapData : SpecialMapData
    {
        [SerializeField] private Transform[] _shopItemTransforms;
        [SerializeField] private GameObject _itemObject;
        [Space]
        [Header("리롤 비용")]
        [SerializeField] private int _rerollGoldValue = 5;
        [Header("리롤 1회당 증가하는 비용")]
        [SerializeField] private int _rerollGoldAmountIncreases = 1;
        
        public UnityEvent _rerollEvnet { get; } = new();

        private List<ItemHolder> _itemHolders = new List<ItemHolder>();

        private ItemPoolSystem _itemPoolSystem;
        private void Awake()
        {
            SystemManager.Instance.PlayerManager.PlayerMapPosition.AddListener(ItemCreate);
            _rerollEvnet.AddListener(ReRoll);
            _itemPoolSystem = SystemManager.Instance.GetSystem<ItemPoolSystem>();
        }

        private void OnDestroy()
        {
            //SystemManager.Instance.PlayerManager.PlayerMapPosition.RemoveListener(ItemCreate);
        }
        
        private void ItemCreate(Vector2Int position)
        {
            if (position == MapPosition)
            {
                var items = _itemPoolSystem.GetItemsWithDropPercentage(_shopItemTransforms.Length,
                    DropGameType.Shop);
                
                for (int i = 0; i < items.Count; i++)
                {
                    var holder = Instantiate(_itemObject, _shopItemTransforms[i]).GetComponent<ItemHolder>();
                    
                    holder.gameObject.SetActive(true);
                    holder.Init(items[i]);
                    _itemHolders.Add(holder);
                }
                _itemPoolSystem.HolderItemCreatedEvent.Invoke(items);
                SystemManager.Instance.PlayerManager.PlayerMapPosition.RemoveListener(ItemCreate);
            }
        }

        private void ReRoll()
        {
            _rerollGoldValue += _rerollGoldAmountIncreases;
            for (int i = 0; i < _itemHolders.Count; i++)
            {
                if (_itemHolders[i].IsUsed())
                    continue;
                _itemPoolSystem.HolderItemRemover(_itemHolders[i].ItemGameData);
            }
            var items = _itemPoolSystem.GetItemsWithDropPercentage(_shopItemTransforms.Length,
                DropGameType.Shop);

            List<ItemGameData> createdHolders = new List<ItemGameData>();
            for (int i = 0; i < _itemHolders.Count; i++)
            {
                if (_itemHolders[i].IsUsed())
                    continue;
                _itemHolders[i].Init(items[i]);
                createdHolders.Add(items[i]);
            }
            _itemPoolSystem.HolderItemCreatedEvent.Invoke(createdHolders);
        }

        public int GetReRollGold()
        {
            return _rerollGoldValue;
        }
    }
}
