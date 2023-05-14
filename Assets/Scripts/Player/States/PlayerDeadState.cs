using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QT.Core;

namespace QT.InGame
{
    [FSMState((int)Player.States.Dead)]

    public class PlayerDeadState : FSMState<Player>
    {
        public PlayerDeadState(IFSMEntity owner) : base(owner)
        {
            
        }

        public override void InitializeState()
        {
            _ownerEntity.Rigidbody.isKinematic = true;
            _ownerEntity.gameObject.layer = LayerMask.NameToLayer("Default");
            _ownerEntity.Rigidbody.velocity = Vector2.zero;
            _ownerEntity.SetDeadAnimation();
            SystemManager.Instance.LoadingManager.GameOverOpen();
        }
    }
}
