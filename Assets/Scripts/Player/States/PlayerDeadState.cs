using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QT.Core;
using QT.UI;
using QT.Util;

namespace QT.InGame
{
    [FSMState((int)Player.States.Dead)]

    public class PlayerDeadState : FSMState<Player>
    {
        private const float DeadDelayTime = 3.5f;
        private readonly int DeadAnimHash = Animator.StringToHash("IsDead");
        
        public PlayerDeadState(IFSMEntity owner) : base(owner)
        {
            
        }

        public override void InitializeState()
        {
            _ownerEntity.Rigidbody.isKinematic = true;
            _ownerEntity.gameObject.layer = LayerMask.NameToLayer("Default");
            _ownerEntity.Rigidbody.velocity = Vector2.zero;
            
            _ownerEntity.Animator.SetBool(DeadAnimHash, true);
            _ownerEntity.OnAim.RemoveAllListeners();
            
            _ownerEntity.StartCoroutine(UnityUtil.WaitForFunc(() => SystemManager.Instance.UIManager.SetState(UIState.GameOver), DeadDelayTime));
        }
        

        public override void ClearState()
        {
            _ownerEntity.Animator.SetBool(DeadAnimHash, false);
        }
    }
}
