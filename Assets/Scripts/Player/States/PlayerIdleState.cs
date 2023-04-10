using UnityEngine;
using QT.Core;
namespace QT.Player
{
    [FSMState((int)Player.States.Idle)]
    public class PlayerIdleState : FSMState<Player>
    {
        public PlayerIdleState(IFSMEntity owner) : base(owner)
        {
            
        }

        public override void InitializeState()
        {
        }
    }

}