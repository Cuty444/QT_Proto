using System.Timers;
using QT.Core;
using UnityEngine;

namespace QT.InGame
{
    [FSMState((int) Dullahan.States.Stun)]
    public class DullahanStunState : FSMState<Dullahan>
    {
        public DullahanStunState(IFSMEntity owner) : base(owner)
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