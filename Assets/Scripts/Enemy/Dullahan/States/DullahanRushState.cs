using System.Timers;
using QT.Core;
using UnityEngine;

namespace QT.InGame
{
    [FSMState((int) Dullahan.States.Rush)]
    public class DullahanRushState : FSMState<Dullahan>
    {
        public DullahanRushState(IFSMEntity owner) : base(owner)
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