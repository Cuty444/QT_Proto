using System.Timers;
using QT.Core;
using UnityEngine;

namespace QT.InGame
{
    [FSMState((int) Dullahan.States.Jump)]
    public class DullahanJumpState : FSMState<Dullahan>
    {
        public DullahanJumpState(IFSMEntity owner) : base(owner)
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