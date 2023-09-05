using System;
using System.Collections;
using System.Collections.Generic;
using QT.Core;
using UnityEngine;
using UnityEngine.Events;
using EventType = QT.Core.EventType;
using TriggerTypes = QT.ItemEffectGameData.TriggerTypes;

namespace QT.InGame
{
    public class Inventory
    {
        public Item ActiveItem { get; private set; }
        
        private Player _targetPlayer;
        private List<Item> _items = new ();
        private PlayerManager _playerManager;


        public Inventory(Player target)
        {
            _targetPlayer = target;
            SetTriggerPointEvents();
        }

        ~Inventory()
        {
            ClearInventory();
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
            
            SystemManager.Instance.EventManager.AddEvent(this, InvokeEvent);
        }

        private void InvokeEvent(EventType unityEvent, object data)
        {
            switch (unityEvent)
            {
                case EventType.OnDamage:
                    InvokeTrigger(TriggerTypes.OnHpChanged);
                    break;
                case EventType.OnGoldChanged:
                    InvokeTrigger(TriggerTypes.OnGoldChanged);
                    break;
                case EventType.OnCharged:
                    InvokeTrigger(TriggerTypes.OnCharged);
                    break;
                case EventType.OnSwing:
                    InvokeTrigger(TriggerTypes.OnSwing);
                    break;
                case EventType.OnSwingHit:
                    InvokeTrigger(TriggerTypes.OnSwingHit);
                    break;
                case EventType.OnAttackStunEnemy:
                    InvokeTrigger(TriggerTypes.OnAttackStunEnemy);
                    break;
                case EventType.OnParry:
                    InvokeTrigger(TriggerTypes.OnParry);
                    break;
                case EventType.OnDodge:
                    InvokeTrigger(TriggerTypes.OnDodge);
                    break;
            }
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

        public void ClearInventory()
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
            
            SystemManager.Instance?.EventManager.RemoveEvent(this);
        }

        public void CopyItemList(List<int> items, int activeItemId)
        {
            foreach (var id in items)
            {
                var item = new Item(id, _targetPlayer);
            
                _items.Add(item);
                item.OnEquip();
            }

            if (activeItemId <= 0)
            {
                return;
            }
            
            var active = new Item(activeItemId, _targetPlayer);
            
            if (active.ItemGameData.GradeType == ItemGameData.GradeTypes.Active)
            {
                ActiveItem?.OnRemoved();
                
                ActiveItem = active;
                ActiveItem.OnEquip();
            }
        }
        
    }
}
