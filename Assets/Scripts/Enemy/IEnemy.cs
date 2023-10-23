using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QT.InGame
{
    public interface IEnemy : IHitAble
    {
        public string PrefabPath { get; set; }
        
        public bool IsRigid { get; }
        public Status HP { get; }

        public void initialization(int enemyId);
    }
}
