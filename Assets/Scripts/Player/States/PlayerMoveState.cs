using UnityEngine;
using QT.Core;

namespace QT.InGame
{
    [FSMState((int)Player.States.Move)]
    public class PlayerMoveState : FSMState<Player>
    {
        private Vector2 _moveDirection;
        
        public PlayerMoveState(IFSMEntity owner) : base(owner)
        {
        }

        public override void InitializeState()
        {
            _ownerEntity.OnMove = OnMove;
            _ownerEntity.SetAction(Player.ButtonActions.Swing, OnSwing);
            _ownerEntity.SetAction(Player.ButtonActions.Dodge, OnDodge);

            _moveDirection = Vector2.zero;
        }

        public override void ClearState()
        {
            _ownerEntity.OnMove = null;
            _ownerEntity.ClearAction(Player.ButtonActions.Swing);
            _ownerEntity.ClearAction(Player.ButtonActions.Dodge);
            
            _ownerEntity.Rigidbody.velocity = Vector2.zero;
        }

        public override void FixedUpdateState()
        {
            _ownerEntity.Rigidbody.velocity = _moveDirection * _ownerEntity.GetStat(PlayerStats.MovementSpd).Value;
        }

        protected virtual void OnMove(Vector2 direction)
        {
            if (direction == Vector2.zero)
            {
                _ownerEntity.ChangeState(Player.States.Idle);
            }

            _moveDirection = direction;
        }

        protected virtual void OnSwing(bool isOn)
        {
            if (isOn)
            {
                _ownerEntity.ChangeState(Player.States.Swing);
            }
        }
        
        protected virtual void OnDodge(bool isOn)
        {
            if (isOn)
            {
                _ownerEntity.ChangeState(Player.States.Dodge);
            }
        }
    }
}