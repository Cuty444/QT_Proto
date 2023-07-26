using System.Collections;
using System.Collections.Generic;
using QT.Core;
using QT.InGame;

namespace QT.InGame
{
    public class Buff
    {
        private readonly BuffCalculator _calculator;
        private readonly StatComponent _statComponent;

        public readonly float Duration;
        public readonly object Source;

        private float _timer;

        public Buff(int buffId, StatComponent statComponent, object source)
        {
            _calculator = SystemManager.Instance.DataManager.GetDataBase<BuffEffectGameDataBase>().GetData(buffId);
            Duration = _calculator.Duration;
            
            _statComponent = statComponent;
            Source = source;
        }
        
        public void ApplyBuff()
        {
            _calculator.ApplyEffect(_statComponent, this);
            _timer = 0;
        }

        public void RemoveBuff()
        {
            _calculator.RemoveEffect(_statComponent, this);
        }
        
        public void ReapplyItemEffect()
        {
            _calculator.RemoveEffect(_statComponent, this);
            _calculator.ApplyEffect(_statComponent, this);
            _timer = 0;
        }

        public bool CheckDuration(float deltaTime)
        {
            _timer += deltaTime;
            return _timer > Duration;
        }
    }
}
