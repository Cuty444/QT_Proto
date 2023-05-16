using UnityEngine;
using QT.Core;
using QT.Core.Input;

namespace QT.InGame
{
    [FSMState((int)Player.States.Move)]
    public class PlayerMoveState : FSMState<Player>
    {
        private InputSystem _inputSystem;
        public PlayerMoveState(IFSMEntity owner) : base(owner)
        {
            _inputSystem = SystemManager.Instance.GetSystem<InputSystem>();
        }

        public override void InitializeState()
        {
            _inputSystem.OnKeyMoveEvent.AddListener(ChangeMove);
            _inputSystem.OnKeyDownAttackEvent.AddListener(OnAttackStart);
        }

        public override void ClearState()
        {
            _inputSystem.OnKeyMoveEvent.RemoveListener(ChangeMove);
            _inputSystem.OnKeyDownAttackEvent.RemoveListener(OnAttackStart);
            _ownerEntity.SetMoveDirection(Vector2.zero);
            _ownerEntity.Rigidbody.velocity = Vector2.zero;
        }

        public override void FixedUpdateState()
        {
            Move();
        }

        private void ChangeMove(Vector2 direction)
        {
            if (direction != Vector2.zero)
                return;
            //_ownerEntity.ChangeState(Player.States.Idle);
        }
        
        private void OnAttackStart()
        {
            //if (_ownerEntity.CurrentStateIndex == (int)Player.States.Dodge)
            //    return;
            //_ownerEntity.ChangeState(Player.States.Swing);
        }
        

        private void Move()
        {
            _ownerEntity.Rigidbody.velocity = _ownerEntity.MoveDirection * _ownerEntity.GetStat(PlayerStats.MovementSpd).Value;
        }
    }
}