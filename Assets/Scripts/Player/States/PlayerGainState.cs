using System.Collections;
using System.Collections.Generic;
using QT.Core;
using QT.InGame;
using UnityEngine;

namespace QT
{
    [FSMState((int)Player.States.Gain)]
    public class PlayerGainState : FSMState<Player>
    {
        private readonly int GainAnimHash = Animator.StringToHash("Gain");
        public PlayerGainState(IFSMEntity owner) : base(owner)
        {

        }

        public override void InitializeState()
        {
            _ownerEntity.Animator?.SetTrigger(GainAnimHash);
            _ownerEntity.RevertToPreviousState();
        }

        public override void ClearState()
        {
        }
    }
}
