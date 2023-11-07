using System;
using System.Collections;
using System.Collections.Generic;
using QT.Core;
using QT.Core.Map;
using UnityEngine;

namespace QT
{
    public class RewardMapData : SpecialMapData
    {
        [SerializeField] private Transform _rewardItemTransform;
        [SerializeField] private GameObject _itemObject;
        
        private void Awake()
        {
            SystemManager.Instance.PlayerManager.PlayerMapPosition.AddListener(ItemCreate);
        }

        private void OnDestroy()
        {
            SystemManager.Instance.PlayerManager.PlayerMapPosition.RemoveListener(ItemCreate);
        }

        private void ItemCreate(Vector2Int position)
        {
            if (position == MapPosition)
            {
                var items = SystemManager.Instance.ItemPoolSystem.GetItemsWithDropPercentage(1,
                    DropGameType.Shop);
                
                var holder = Instantiate(_itemObject, _rewardItemTransform).GetComponent<ItemHolder>();
                    
                holder.gameObject.SetActive(true);
                holder.Init(items[0]);
                SystemManager.Instance.ItemPoolSystem.HolderItemCreatedEvent.Invoke(items);
                SystemManager.Instance.PlayerManager.PlayerMapPosition.RemoveListener(ItemCreate);
            }
        }
    }
}