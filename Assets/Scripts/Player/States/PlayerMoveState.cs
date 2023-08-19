using UnityEngine;
using QT.Core;

namespace QT.InGame
{
    [FSMState((int)Player.States.Move)]
    public class PlayerMoveState : FSMState<Player>
    {
        private static readonly int AnimationIdleHash = Animator.StringToHash("PlayerIdle");
        private static readonly int AnimationMoveSpeedHash = Animator.StringToHash("MoveSpeed");
        
        private Vector2 _moveDirection;
        
        protected Stat _moveSpeed;
        
        public PlayerMoveState(IFSMEntity owner) : base(owner)
        {
            _moveSpeed = _ownerEntity.StatComponent.GetStat(PlayerStats.MovementSpd);
        }

        public override void InitializeState()
        {
            _ownerEntity.OnMove = OnMove;
            _ownerEntity.SetAction(Player.ButtonActions.Swing, OnSwing);
            //_ownerEntity.SetAction(Player.ButtonActions.Throw, OnThrow);
            _ownerEntity.SetAction(Player.ButtonActions.Dodge, OnDodge);
            _ownerEntity.SetAction(Player.ButtonActions.Interaction,OnInteraction);

            _moveDirection = Vector2.zero;
        }

        public override void ClearState()
        {
            _ownerEntity.OnMove = null;
            _ownerEntity.ClearAction(Player.ButtonActions.Swing);
            //_ownerEntity.ClearAction(Player.ButtonActions.Throw);
            _ownerEntity.ClearAction(Player.ButtonActions.Dodge);
            _ownerEntity.ClearAction(Player.ButtonActions.Interaction);

            _ownerEntity.Rigidbody.velocity = Vector2.zero;
        }

        public override void FixedUpdateState()
        {
            if (_ownerEntity.CheckFall())
            {
                _ownerEntity.Rigidbody.velocity = Vector2.zero;
                _ownerEntity.ChangeState(Player.States.Fall);
                return;
            }
            else
            {
                _ownerEntity.LastSafePosition = _ownerEntity.transform.position;
            }
            
            var speed = _moveSpeed.Value;
            var currentNormalizedSpeed = _ownerEntity.Rigidbody.velocity.sqrMagnitude / (speed * speed);
            
            _ownerEntity.Animator.SetFloat(AnimationMoveSpeedHash, currentNormalizedSpeed);
            _ownerEntity.Animator.SetBool(AnimationIdleHash, currentNormalizedSpeed <= 0.1f);
            
            _ownerEntity.Rigidbody.velocity = _moveDirection * speed;
        }

        protected virtual void OnMove(Vector2 direction)
        {
            _moveDirection = direction;
        }

        protected virtual void OnSwing(bool isOn)
        {
            if (isOn && _ownerEntity.StatComponent.GetStatus(PlayerStats.SwingCooldown).IsFull())
            {
                _ownerEntity.ChangeState(Player.States.Swing);
            }
        }
        
        protected virtual void OnThrow(bool isOn)
        {
            if (isOn&& _ownerEntity.StatComponent.GetStatus(PlayerStats.ThrowCooldown).IsFull())
            {
                _ownerEntity.ChangeState(Player.States.Throw);
            }
        }
        
        protected virtual void OnDodge(bool isOn)
        {
            if (isOn && _ownerEntity.StatComponent.GetStatus(PlayerStats.DodgeCooldown).IsFull() && _moveDirection != Vector2.zero)
            {
                (_ownerEntity.ChangeState(Player.States.Dodge) as PlayerDodgeState).InitializeState(_moveDirection);
            }
        }

        protected virtual void OnInteraction(bool isOn)
        {
            if (isOn)
            {
                SystemManager.Instance.PlayerManager.PlayerItemInteraction.Invoke();
            }
        }

    }
}