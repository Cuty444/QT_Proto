using System;
using System.Collections;
using System.Collections.Generic;
using QT.Core;
using QT.Core.Data;
using UnityEngine;

namespace QT.Enemy
{

    public class EnemyAttack : MonoBehaviour
    {
        private int _atkDmg;

        private void Start()
        {
            _atkDmg = SystemManager.Instance.GetSystem<GlobalDataSystem>().EnemyTable.ATKDmg;
        }

        public int GetDamage()
        {
            return _atkDmg;
        }
    }
}