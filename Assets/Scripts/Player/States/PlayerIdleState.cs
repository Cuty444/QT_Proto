using UnityEngine;
using QT.Core;
using QT.Core.Input;

namespace QT.InGame
{
    [FSMState((int)Player.States.Idle)]
    public class PlayerIdleState : FSMState<Player>
    {
        private InputSystem _inputSystem;
        public PlayerIdleState(IFSMEntity owner) : base(owner)
        {
            _inputSystem = SystemManager.Instance.GetSystem<InputSystem>();
        }

        public override void InitializeState()
        {
            _inputSystem.OnKeyMoveEvent.AddListener(ChangeMove);
            _inputSystem.OnKeyDownAttackEvent.AddListener(OnAttackStart);
            _ownerEntity.SetMoveDirection(Vector2.zero);
            _ownerEntity.Rigidbody.velocity = Vector2.zero;
        }

        public override void ClearState()
        {
            _inputSystem.OnKeyMoveEvent.RemoveListener(ChangeMove);
            _inputSystem.OnKeyDownAttackEvent.RemoveListener(OnAttackStart);
        }

        private void ChangeMove(Vector2 direction)
        {
            if (direction == Vector2.zero)
                return;
            _ownerEntity.ChangeState(Player.States.Move);
        }
        
        private void OnAttackStart()
        {
            //if (_ownerEntity.CurrentStateIndex == (int)Player.States.Dodge)
            //    return;
            //_ownerEntity.ChangeState(Player.States.Swing);
        }
    }

}