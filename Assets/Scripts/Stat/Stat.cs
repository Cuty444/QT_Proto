using System;
using System.Collections.Generic;
using UnityEngine.Events;

namespace QT
{
    public class Stat
    {
        public readonly float BaseValue;

        public float Value
        {
            get
            {
                if (_isDirty) UpdateValue();

                return _value;
            }
        }
        
        private readonly List<StatModifier> _statModifiers = new();

        private float _value;
        protected bool _isDirty;

        public static implicit operator float(Stat stat) => stat.Value;
        public UnityEvent OnValueChanged { get; } = new();
        
        public Stat(float baseValue)
        {
            _value = BaseValue = baseValue;
        }

        public void AddModifier(StatModifier statModifier)
        {
            _statModifiers.Add(statModifier);
            _statModifiers.Sort((a, b) => a.Order - b.Order);
            _isDirty = true;
        }

        public void RemoveModifier(StatModifier statModifier)
        {
            _statModifiers.Remove(statModifier);
            _isDirty = true;
        }
        
        public void RemoveAllModifiersFromSource(object source)
        {
            _statModifiers.RemoveAll(mod => mod.Source == source);
            _isDirty = true;
        }
        
        public void ClearAllModifiers()
        {
            _statModifiers.Clear();
            _isDirty = true;
        }

        public virtual void UpdateValue()
        {
            _value = BaseValue;

            foreach (var mod in _statModifiers)
            {
                if (mod.ModType == StatModifier.ModifierType.Hard)
                {
                    _value = mod.Value;
                }
                else if (mod.ModType == StatModifier.ModifierType.Multiply)
                {
                    _value *= mod.Value;
                }
                else if (mod.ModType == StatModifier.ModifierType.Addition)
                {
                    _value += mod.Value;
                }
            }

            _value = (float) Math.Round(_value, 4);
            _isDirty = false;
            
            OnValueChanged.Invoke();
        }
    }
}