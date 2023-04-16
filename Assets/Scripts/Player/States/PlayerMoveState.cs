using UnityEngine;
using QT.Core;
using QT.Core.Input;

namespace QT.Player
{
    [FSMState((int)Player.States.Move)]
    public class PlayerMoveState : FSMState<Player>
    {
        private InputSystem _inputSystem;
        protected Vector2 _moveDirection;
        public PlayerMoveState(IFSMEntity owner) : base(owner)
        {
            _inputSystem = SystemManager.Instance.GetSystem<InputSystem>();
        }

        public override void InitializeState()
        {
            _inputSystem.OnKeyMoveEvent.AddListener(MoveDirection);
            _inputSystem.OnKeyDownAttackEvent.AddListener(OnAttackStart);
        }

        public override void ClearState()
        {
            _inputSystem.OnKeyMoveEvent.RemoveListener(MoveDirection);
            _inputSystem.OnKeyDownAttackEvent.RemoveListener(OnAttackStart);
            _ownerEntity.Rigidbody.velocity = Vector2.zero;
        }

        public override void FixedUpdateState()
        {
            Move();
        }

        private void OnAttackStart()
        {
            _ownerEntity.ChangeState(Player.States.Swing);
        }
        
        protected virtual void MoveDirection(Vector2 direction)
        {
            _moveDirection = direction.normalized;
            if (_moveDirection == Vector2.zero)
                _ownerEntity.ChangeState(Player.States.Idle);
        }

        private void Move()
        {
            _ownerEntity.Rigidbody.velocity = _moveDirection * (_ownerEntity.MovementSpd.Value * Time.fixedDeltaTime);
        }
    }
}