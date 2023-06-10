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
        private readonly int AnimationGainHash = Animator.StringToHash("PlayerGain");
        public PlayerGainState(IFSMEntity owner) : base(owner)
        {

        }

        public override void InitializeState()
        {
            _ownerEntity.Animator.SetTrigger(AnimationGainHash);
            _ownerEntity.RevertToPreviousState();
        }

        public override void ClearState()
        {
        }
    }
}
