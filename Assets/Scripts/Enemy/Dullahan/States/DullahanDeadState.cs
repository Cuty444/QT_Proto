using System.Timers;
using QT.Core;
using UnityEngine;

namespace QT.InGame
{
    [FSMState((int) Dullahan.States.Dead)]
    public class DullahanDeadState : FSMState<Dullahan>
    {
        private readonly int IsDeadAnimHash = Animator.StringToHash("IsDead");
        
        public DullahanDeadState(IFSMEntity owner) : base(owner)
        {
        }

        public override void InitializeState()
        {
            _ownerEntity.Animator.SetBool(IsDeadAnimHash, true);
        }

        public override void ClearState()
        {
            _ownerEntity.Animator.SetBool(IsDeadAnimHash, false);
        }
    }
}