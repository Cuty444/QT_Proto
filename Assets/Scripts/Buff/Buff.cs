using System.Collections;
using System.Collections.Generic;
using QT.Core;
using QT.InGame;

namespace QT.InGame
{
    public class Buff
    {
        private readonly List<BuffCalculator> _calculators;
        private readonly StatComponent _statComponent;

        public readonly float Duration;
        public readonly object Source;

        private float _timer;

        public Buff(int buffId, StatComponent statComponent, object source)
        {
            _calculators = SystemManager.Instance.DataManager.GetDataBase<BuffEffectGameDataBase>().GetData(buffId);
            Duration = _calculators[0].Duration;
            
            _statComponent = statComponent;
            Source = source;
        }
        
        public void ApplyBuff()
        {
            foreach (var calculator in _calculators)
            {
                calculator.ApplyEffect(_statComponent, this);
            }
            _timer = 0;
        }

        public void RemoveBuff()
        {
            foreach (var calculator in _calculators)
            {
                calculator.RemoveEffect(_statComponent, this);
            }
        }
        
        public void RefreshBuff()
        {
            foreach (var calculator in _calculators)
            {
                calculator.RemoveEffect(_statComponent, this);
                calculator.ApplyEffect(_statComponent, this);
            }
            
            _timer = 0;
        }

        public bool CheckDuration(float deltaTime)
        {
            if (Duration <= 0)
            {
                return false;
            }
            
            _timer += deltaTime;
            return _timer > Duration;
        }
    }
}
