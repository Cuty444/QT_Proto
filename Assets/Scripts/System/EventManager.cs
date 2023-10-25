using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace QT.Core
{
    
    [Flags]
    public enum TriggerTypes
    {
        Equip = 1 << 0,
            
        Update = 1 << 1,
            
        OnActiveKey = 1 << 2,
            
        OnClearRoom = 1 << 3,
            
        OnCharged = 1 << 4,
        OnGoldChanged = 1 << 5,
            
        OnDamage = 1 << 6,
        OnHeal = 1 << 7,
            
        OnSwing = 1 << 8,
        OnSwingHit = 1 << 9,
        OnChargedSwingHit = 1 << 10,
        OnAttackStunEnemy = 1 << 11,
        OnParry = 1 << 12,
            
        OnDodge = 1 << 13,
        OnDodgeEnd = 1 << 14,
            
        OnMovementSpdChanged = 1 << 20,
        OnChargeBounceCountChanged = 1 << 21
    }

    public class EventManager
    {
        private Dictionary<object, UnityAction<TriggerTypes, object>> _events = new ();


        public void AddEvent(object target, UnityAction<TriggerTypes, object> action)
        {
            _events.TryAdd(target, action);
        }
        
        public void InvokeEvent(TriggerTypes type, object data)
        {
            foreach (var target in _events.Values)
            {
                target.Invoke(type, data);
            }
        }

        public void RemoveEvent(object target)
        {
            if (target != null && _events.ContainsKey(target))
            {
                _events.Remove(target);
            }
        }

    }
}
