using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace QT.Core
{
    public enum EventType
    {
        OnPlayerCreated,
        
        OnPlayerEnterDoor,
        OnPlayerEnterRoom,
        
        OnHeal,
        OnDamage,
        
        OnGoldChanged,
        
        OnCharged,
        OnSwing,
        OnSwingHit,
        OnAttackStunEnemy,
        OnParry,
        OnDodge,
    }

    public class EventManager
    {
        private Dictionary<object, UnityAction<EventType, object>> _events = new ();


        public void AddEvent(object target, UnityAction<EventType, object> action)
        {
            _events.TryAdd(target, action);
        }
        
        public void InvokeEvent(EventType type, object data)
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
