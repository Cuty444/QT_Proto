using System.Collections;
using System.Collections.Generic;
using QT.Core;
using QT.InGame;
using UnityEngine;

namespace QT.InGame
{
    public class Buff
    {
        private readonly BuffCalculator _calculator;
        private readonly StatComponent _statComponent;
        
        public Buff(int buffId, StatComponent statComponent)
        {
            _calculator = SystemManager.Instance.DataManager.GetDataBase<BuffEffectGameDataBase>().GetData(buffId);
            _statComponent = statComponent;
        }
        
        public void ApplyBuff()
        {
            _calculator.ApplyEffect(_statComponent, this);
        }
        
        public void RemoveBuff()
        {
            _calculator.RemoveEffect(_statComponent, this);
        }
        
        public void RefreshBuff()
        {
            _calculator.RemoveEffect(_statComponent, this);
            _calculator.ApplyEffect(_statComponent, this);
        }
    }
}
