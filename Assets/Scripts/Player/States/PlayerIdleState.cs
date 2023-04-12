using UnityEngine;
using QT.Core;
using QT.Core.Input;

namespace QT.Player
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
        }

        public override void ClearState()
        {
            _inputSystem.OnKeyMoveEvent.RemoveListener(ChangeMove);
        }

        private void ChangeMove(Vector2 direction)
        {
            if (direction == Vector2.zero)
                return;
            _ownerEntity.ChangeState(Player.States.Move);
        }
    }

}