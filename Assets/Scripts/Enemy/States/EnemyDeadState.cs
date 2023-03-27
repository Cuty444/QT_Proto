using System.Collections;
using System.Collections.Generic;
using QT.Core;
using UnityEngine;

namespace QT.Enemy
{
    [FSMState((int)Enemy.States.Dead)]
    public class EnemyDeadState : FSMState<Enemy>
    {
        public EnemyDeadState(IFSMEntity owner) : base(owner)
        {
        }

        public override void InitializeState()
        {
            
        }

        public override void UpdateState()
        {
            
        }

        public override void FixedUpdateState()
        {
            
        }

        public override void ClearState()
        {
            
        }
    }
}
