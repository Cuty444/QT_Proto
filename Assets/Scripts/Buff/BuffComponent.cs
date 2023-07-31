using System;
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
        
        public Buff AddBuff(int buffId, object source)
        {
            var buff = new Buff(buffId, _statComponent, source);
            buff.ApplyBuff();
            
            _buffs.Add(buff);

            return buff;
        }

        public void RemoveBuff(Buff buff)
        {
            buff.RemoveBuff();
            _buffs.Remove(buff);
        }
        
        public void RemoveAllBuffsFromSource(object source)
        {
            for (int i = 0; i < _buffs.Count; i++)
            {
                if (_buffs[i].Source == source)
                {
                    RemoveBuff(_buffs[i]);
                    i--;
                }
            }
        }

        private void Update()
        {
            for (int i = 0; i < _buffs.Count; i++)
            {
                if (_buffs[i].CheckDuration(Time.deltaTime))
                {
                    RemoveBuff(_buffs[i]);
                    i--;
                }
            }
        }
        
    }
}
