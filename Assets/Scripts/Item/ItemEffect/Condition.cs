using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Conditions = QT.ItemEffectGameData.Conditions;

namespace QT.InGame
{
    public interface ICondition
    {
        public Conditions ConditionType { get; }

        public bool CheckCondition(StatComponent statComponent, float target);
    }
    
    
    public class LessThanCondition : ICondition
    {
       public Conditions ConditionType => Conditions.LessThan;
       
       public bool CheckCondition(StatComponent statComponent, float target)
       {
           return true;
       }
    }
}
