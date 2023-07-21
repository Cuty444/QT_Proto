using System.Collections;
using System.Collections.Generic;
using QT.Core;
using UnityEngine;
using UnityEngine.Events;

namespace QT.InGame
{
    public class Inventory
    {
        private Player _targetPlayer;
        private List<Item> _items = new List<Item>();
        private PlayerManager _playerManager;
        public Inventory(Player target)
        {
            _targetPlayer = target;
            SetApplyPointEvents();
        }

        private void SetApplyPointEvents()
        {
            _playerManager = SystemManager.Instance.PlayerManager;

            
            _targetPlayer.StatComponent.GetStatus(PlayerStats.HP).OnStatusChanged
                .AddListener(() => InvokeApplyPoint(ItemEffectGameData.ApplyPoints.OnHpChanged));
            
            _playerManager.OnGoldValueChanged.AddListener((value) =>
                InvokeApplyPoint(ItemEffectGameData.ApplyPoints.OnGoldChanged));
            
            _targetPlayer.StatComponent.GetStat(PlayerStats.MovementSpd).OnValueChanged
                .AddListener(() => InvokeApplyPoint(ItemEffectGameData.ApplyPoints.OnMovementSpdChanged));
            
            _targetPlayer.StatComponent.GetStat(PlayerStats.ChargeBounceCount2).OnValueChanged
                .AddListener(() => InvokeApplyPoint(ItemEffectGameData.ApplyPoints.OnChargeBounceCountChanged));
        }

        private void InvokeApplyPoint(ItemEffectGameData.ApplyPoints applyPoints)
        {
            foreach (var item in _items)
            {
                item.InvokeApplyPoint(applyPoints);
            }
        }
        
        public void AddItem(int itemDataId)
        {
            var item = new Item(itemDataId);
            AddItem(item);
        }

        public void NextCopyItem(int itemDataID)
        {
            var item = new Item(itemDataID);
            _items.Add(item);
            item.ApplyItemEffect(_targetPlayer);
        }
        
        public void AddItem(Item item)
        {
            _items.Add(item);
            
            _playerManager.AddItemEvent.Invoke();
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
        
        public Item[] GetItemList()
        {
            var result = new Item[_items.Count];
            _items.CopyTo(result);
            
            return result;
        }
    }
}
