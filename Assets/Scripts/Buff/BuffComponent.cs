using System.Collections;
using System.Collections.Generic;
using QT.Core;
using UnityEngine;

namespace QT.InGame
{
    public class BuffComponent : MonoBehaviour
    {
        private List<Buff> _buffs = new ();
        private StatComponent _statComponent;

        public void Init(StatComponent statComponent)
        {
            _statComponent = statComponent;
        }
        
        public Buff AddBuff(int buffId)
        {
            var buff = new Buff(buffId, _statComponent);
            buff.ApplyBuff();
            
            _buffs.Add(buff);
            
            return buff;
        }
        
        
    }
}
