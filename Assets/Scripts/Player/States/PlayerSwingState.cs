using UnityEngine;
using QT.Core;
namespace QT.Player
{
    [FSMState((int)Player.States.Swing)]
    public class PlayerSwingState : FSMState<Player>
    {
        public PlayerSwingState(IFSMEntity owner) : base(owner)
        {
            
        }

        public override void InitializeState()
        {
        }
    }
}