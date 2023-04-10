using UnityEngine;
using QT.Core;
namespace QT.Player
{
    [FSMState((int)Player.States.Idle)]
    public class PlayerGlobalState : FSMState<Player>
    {
        public PlayerGlobalState(IFSMEntity owner) : base(owner)
        {
            
        }

        public override void InitializeState()
        {
        }
    }

}