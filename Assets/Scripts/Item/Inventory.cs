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
        public Item ActiveItem { get; private set; }
        
        private Player _targetPlayer;
        private List<Item> _items = new List<Item>();
        private PlayerManager _playerManager;


        public Inventory(Player target)
        {
            _targetPlayer = target;
            SetTriggerPointEvents();
        }

        ~Inventory()
        {
            ClearItems();
        }

        private void SetTriggerPointEvents()
        {
            _playerManager = SystemManager.Instance.PlayerManager;

            _targetPlayer.OnActive.AddListener((isOn) => { if (isOn) InvokeTrigger(TriggerTypes.OnActiveKey); });
            
            _targetPlayer.StatComponent.GetStatus(PlayerStats.HP).OnStatusChanged
                .AddListener(() => InvokeTrigger(TriggerTypes.OnHpChanged));

            _targetPlayer.StatComponent.GetStat(PlayerStats.MovementSpd).OnValueChanged
                .AddListener(() => InvokeTrigger(TriggerTypes.OnMovementSpdChanged));
            
            _targetPlayer.StatComponent.GetStat(PlayerStats.ChargeBounceCount).OnValueChanged
                .AddListener(() => InvokeTrigger(TriggerTypes.OnChargeBounceCountChanged));
            
            
            _playerManager.OnGoldValueChanged.AddListener((value) => InvokeTrigger(TriggerTypes.OnGoldChanged));
            
            _playerManager.OnSwing.AddListener(() => InvokeTrigger(TriggerTypes.OnSwing));
            _playerManager.OnSwingHit.AddListener(() => InvokeTrigger(TriggerTypes.OnSwingHit));
            _playerManager.OnAttackStunEnemy.AddListener(() => InvokeTrigger(TriggerTypes.OnAttackStunEnemy));
            _playerManager.OnParry.AddListener(() => InvokeTrigger(TriggerTypes.OnParry));
            _playerManager.OnDodge.AddListener(() => InvokeTrigger(TriggerTypes.OnDodge));
        }

        private void InvokeTrigger(TriggerTypes triggerTypes)
        {
            ActiveItem?.InvokeTrigger(triggerTypes);
            foreach (var item in _items)
            {
                item.InvokeTrigger(triggerTypes);
            }
        }
        
        public void AddItem(int itemDataId)
        {
            var item = new Item(itemDataId, _targetPlayer);

            if (item.ItemGameData == null)
            {
                return;
            }

            if (item.ItemGameData.GradeType == ItemGameData.GradeTypes.Active)
            {
                ActiveItem?.OnRemoved();
                
                ActiveItem = item;
                ActiveItem.OnEquip();
            }
            else
            {
                _items.Add(item);
                item.OnEquip();
            }

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

        public bool Contains(int itemId)
        {
            foreach (var item in _items)
            {
                if(item.ItemGameData.Index == itemId)
                    return true;
            }

            return false;
        }
        
        public Item[] GetItemList()
        {
            var result = new Item[_items.Count];
            _items.CopyTo(result);
            
            return result;
        }

        public void ClearItems()
        {
            if (ActiveItem != null)
            {
                ActiveItem.OnRemoved();
                ActiveItem = null;
            }

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
