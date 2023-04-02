using UnityEngine;

namespace QT
{
    public class Status : Stat
    {
        public float StatusValue => _statusValue;
        
        private float _statusValue;
        
        public Status(float baseValue) : base(baseValue)
        {
            _statusValue = baseValue;
        }

        public void AddStatus(float amount)
        {
            _statusValue = Mathf.Clamp(_statusValue + amount, 0, Value);
        }

        public override void UpdateValue()
        {
            var ratio = _statusValue / Value;
            base.UpdateValue();
            
            _statusValue = Value * ratio;
        }
    }
}
