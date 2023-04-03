using System.Collections;
using System.Collections.Generic;
using QT.Core;
using QT.Core.Data;
using UnityEngine;

namespace QT.Enemy
{
    [FSMState((int)Enemy.States.Projectile)]
    public class EnemyProjectileState : FSMState<Enemy>
    {
        public EnemyProjectileState(IFSMEntity owner) : base(owner)
        {
        }

        public override void InitializeState()
        {
        }

        public override void UpdateState()
        {
        }
    }
}
