using System.Collections;
using System.Collections.Generic;
using QT.Core;
using UnityEngine;
using UnityEngine.Events;
using TriggerTypes = QT.ItemEffectGameData.TriggerTypes;

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
            SetTriggerPointEvents();
        }

        private void SetTriggerPointEvents()
        {
            _playerManager = SystemManager.Instance.PlayerManager;

            _targetPlayer.SetAction(Player.ButtonActions.Active,
                (isOn) =>
                {
                    if (isOn) InvokeTrigger(TriggerTypes.OnActiveKey);
                });
            
            _targetPlayer.StatComponent.GetStatus(PlayerStats.HP).OnStatusChanged
                .AddListener(() => InvokeTrigger(ItemEffectGameData.TriggerTypes.OnHpChanged));
            
            _playerManager.OnGoldValueChanged.AddListener((value) =>
                InvokeTrigger(ItemEffectGameData.TriggerTypes.OnGoldChanged));
            
            _targetPlayer.StatComponent.GetStat(PlayerStats.MovementSpd).OnValueChanged
                .AddListener(() => InvokeTrigger(ItemEffectGameData.TriggerTypes.OnMovementSpdChanged));
            
            _targetPlayer.StatComponent.GetStat(PlayerStats.ChargeBounceCount2).OnValueChanged
                .AddListener(() => InvokeTrigger(ItemEffectGameData.TriggerTypes.OnChargeBounceCountChanged));
        }

        private void InvokeTrigger(TriggerTypes triggerTypes)
        {
            foreach (var item in _items)
            {
                item.InvokeTrigger(triggerTypes);
            }
        }
        
        public void AddItem(int itemDataId)
        {
            var item = new Item(itemDataId, _targetPlayer);
            
            _items.Add(item);
            item.OnEquip();
            
            _playerManager.AddItemEvent.Invoke();
        }
        
        public void RemoveItem(int index)
        {
            if (index < 0 || index >= _items.Count)
            {
                return;
            }
            
            _items[index].OnRemoved();
            _items.RemoveAt(index);
        }
        
        public Item[] GetItemList()
        {
            var result = new Item[_items.Count];
            _items.CopyTo(result);
            
            return result;
        }

        public void ClearItems()
        {
            foreach (var item in _items)
            {
                item.OnRemoved();
            }
            _items.Clear();
        }

        public void CopyItemList(List<int> items)
        {
            foreach (var id in items)
            {
                var item = new Item(id, _targetPlayer);
            
                _items.Add(item);
                item.OnEquip();
            }
        }
        
    }
}
