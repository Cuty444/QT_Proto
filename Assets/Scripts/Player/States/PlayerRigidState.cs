using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QT.Core;
namespace QT.InGame
{
    [FSMState((int)Player.States.Rigid)]
    public class PlayerRigidState : FSMState<Player>
    {

        public PlayerRigidState(IFSMEntity owner) : base(owner)
        {
        }

        public override void InitializeState()
        {
            _ownerEntity.SetRigidAnimation();
        }
        
        public override void ClearState()
        {
        }
    }
}
