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
        OnGoldChanged = 1 << 4,
            
        OnDamage = 1 << 5,
        OnHeal = 1 << 6,
        
        OnSwingStart = 1 << 7,
        OnCharged = 1 << 8,
        
        OnSwing = 1 << 9,
        OnSwingHit = 1 << 10,
        OnChargedSwingHit = 1 << 11,
        OnAttackStunEnemy = 1 << 12,
        OnParry = 1 << 13,
            
        OnDodge = 1 << 14,
        OnDodgeEnd = 1 << 15,
            
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
