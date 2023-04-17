using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QT.Core;
using QT.Core.Input;

namespace QT.Player
{
    [FSMState((int)Player.States.Dodge)]
    public class PlayerDodgeState : FSMState<Player>
    {
        public PlayerDodgeState(IFSMEntity owner) : base(owner)
        {
        }
        public override void InitializeState()
        {
        }
        
        public override void ClearState()
        {
        }
        public override void FixedUpdateState()
        {
        }
        
    }
}
