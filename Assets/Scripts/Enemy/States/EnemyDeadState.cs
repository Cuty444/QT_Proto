using System.Collections;
using System.Collections.Generic;
using QT.Core;
using UnityEngine;

namespace QT.Enemy
{
    [FSMState((int)Enemy.States.Dead)]
    public class EnemyDeadState : FSMState<Enemy>
    {
        private static readonly int DeadAnimHash = Animator.StringToHash("Dead");
        
        public EnemyDeadState(IFSMEntity owner) : base(owner)
        {
        }

        public override void InitializeState()
        {
            _ownerEntity.Animator.SetTrigger(DeadAnimHash);
            _ownerEntity.Rigidbody.velocity = Vector2.zero;
        }
    }
}
