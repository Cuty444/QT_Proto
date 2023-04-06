using UnityEngine;
using QT.Core;
namespace QT.Player
{
    [FSMState((int)Player.States.Idle)]
    public class PlayerMoveState : FSMState<Player>
    {
        public PlayerMoveState(IFSMEntity owner) : base(owner)
        {
            
        }

        public override void InitializeState()
        {
        }
    }
}