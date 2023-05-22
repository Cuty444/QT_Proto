using UnityEngine;

namespace QT
{
    public class Status : Stat
    {
        public float StatusValue => _statusValue;
        
        private float _statusValue;
        
        public static implicit operator float(Status status) => status.StatusValue;
        
        public Status(float baseValue) : base(baseValue)
        {
            _statusValue = baseValue;
        }

        public void AddStatus(float amount)
        {
            _statusValue = Mathf.Clamp(_statusValue + amount, 0, Value);
        }
        
        public void SetStatus(float value)
        {
            _statusValue = Mathf.Clamp(value, 0, Value);;
        }
        
        public bool IsFull()
        {
            return _statusValue >= Value;
        }

        public override void UpdateValue()
        {
            var ratio = Value != 0 ? _statusValue / Value : 1;
            base.UpdateValue();
            
            _statusValue = Value * ratio;
        }
        
    }
}
