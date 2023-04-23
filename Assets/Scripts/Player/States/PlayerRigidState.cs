using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QT.Core;
namespace QT.Player
{
    [FSMState((int)Player.States.Rigid)]
    public class PlayerRigidState : FSMState<Player>
    {

        public PlayerRigidState(IFSMEntity owner) : base(owner)
        {
        }

        public override void InitializeState()
        {
            _ownerEntity.SetRigidAninimation();
        }
        
        public override void ClearState()
        {
        }
    }
}
