using UnityEngine;
using UnityEngine.Events;

namespace QT.InGame
{
    public class Status : Stat
    {
        public float StatusValue
        {
            get
            {
                if (_isDirty) UpdateValue();

                return _statusValue;
            }
        }
        
        private float _statusValue;
        
        public static implicit operator float(Status status) => status.StatusValue;
        
        public UnityEvent OnStatusChanged { get; } = new();
        
        public Status(float baseValue) : base(baseValue)
        {
            _statusValue = baseValue;
        }

        public void AddStatus(float amount)
        {
            if (amount == 0)
            {
                return;
            }
            
            _statusValue = Mathf.Clamp(_statusValue + amount, 0, Value);
            OnStatusChanged?.Invoke();
        }
        
        public void SetStatus(float value)
        {
            if (value == _statusValue)
            {
                return;
            }
            
            _statusValue = Mathf.Clamp(value, 0, Value);
            OnStatusChanged?.Invoke();
        }
        
        public bool IsFull()
        {
            return _statusValue >= Value;
        }

        public override void UpdateValue()
        {
            base.UpdateValue();
            
            _statusValue = Mathf.Clamp(_statusValue, 0, Value);
        }
        
    }
}
