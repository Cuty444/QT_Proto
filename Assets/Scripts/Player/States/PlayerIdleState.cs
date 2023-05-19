using UnityEngine;
using QT.Core;
using UnityEngine.PlayerLoop;

namespace QT.InGame
{
    [FSMState((int)Player.States.Idle)]
    public class PlayerIdleState : FSMState<Player>
    {
        private readonly int AnimationIdleHash = Animator.StringToHash("PlayerIdle");
        
        public PlayerIdleState(IFSMEntity owner) : base(owner)
        {
        }

        public override void InitializeState()
        {
            _ownerEntity.OnMove = OnMove;
            _ownerEntity.SetAction(Player.ButtonActions.Swing, OnSwing);
            
            _ownerEntity.Rigidbody.velocity = Vector2.zero;

            _ownerEntity.Animator.SetBool(AnimationIdleHash, true);
        }

        public override void ClearState()
        {
            _ownerEntity.OnMove = null;
            _ownerEntity.ClearAction(Player.ButtonActions.Swing);
            
            _ownerEntity.Animator.SetBool(AnimationIdleHash, false);
        }

        private void OnMove(Vector2 direction)
        {
            if (direction != Vector2.zero)
            {
                _ownerEntity.ChangeState(Player.States.Move);
            }
        }
        
        private void OnSwing(bool isOn)
        {
            if (isOn)
            {
                _ownerEntity.ChangeState(Player.States.Swing);
            }
        }
    }

}