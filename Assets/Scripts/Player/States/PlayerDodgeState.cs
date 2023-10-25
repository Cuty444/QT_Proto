using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QT.Core;
using QT.Sound;

namespace QT.InGame
{
    [FSMState((int)Player.States.Dodge)]
    public class PlayerDodgeState : FSMState<Player>
    {
        private readonly int IsDodgeAnimHash = Animator.StringToHash("IsDodge");
        private readonly int DirectionXAnimHash = Animator.StringToHash("DirectionX");
        private readonly int DirectionYAnimHash = Animator.StringToHash("DirectionY");

        private LayerMask _dodgeLayer;
        private SoundManager _soundManager;
        private LayerMask _playerLayer;

        public PlayerDodgeState(IFSMEntity owner) : base(owner)
        {
            _soundManager = SystemManager.Instance.SoundManager;
            _dodgeLayer = LayerMask.NameToLayer("PlayerDodge");
            _playerLayer = LayerMask.NameToLayer("Player");
        }
        
        public void InitializeState(Vector2 dir)
        {
            if (dir == Vector2.zero)
            {
                _ownerEntity.RevertToPreviousState();
                return;
            }
            
            _ownerEntity.gameObject.layer = _dodgeLayer;
            
            _ownerEntity.StatComponent.GetStatus(PlayerStats.DodgeCooldown).SetStatus(0);
            _ownerEntity.StatComponent.GetStatus(PlayerStats.DodgeInvincibleTime).SetStatus(0);
            
            _ownerEntity.Animator.SetBool(IsDodgeAnimHash, true);
            _soundManager.PlayOneShot(_soundManager.SoundData.PlayerDashSFX);
            
            
            dir.Normalize();
            
            _ownerEntity.Animator.SetFloat(DirectionXAnimHash, dir.x);
            _ownerEntity.Animator.SetFloat(DirectionYAnimHash, dir.y);

            _ownerEntity.IsDodge = true;
            _ownerEntity.IsFlip = dir.x > 0;
            _ownerEntity.DodgeEffectPlay(dir);

            var force = _ownerEntity.StatComponent.GetStat(PlayerStats.DodgeAddForce).Value;
            _ownerEntity.Rigidbody.velocity = dir * force;

            
            var duration = _ownerEntity.StatComponent.GetStat(PlayerStats.DodgeDurationTime).Value;

            _ownerEntity.StartCoroutine( Util.UnityUtil.WaitForFunc(() =>
            {
                if (!_ownerEntity.CheckFall())
                {
                    _ownerEntity.RevertToPreviousState();
                }
                else
                {
                    _ownerEntity.ChangeState(Player.States.Fall);
                }
                
            },duration));
            
            SystemManager.Instance.EventManager.InvokeEvent(TriggerTypes.OnDodge, null);
        }
        
        public override void ClearState()
        {
            SystemManager.Instance.EventManager.InvokeEvent(TriggerTypes.OnDodgeEnd, null);
            
            _ownerEntity.Animator.SetBool(IsDodgeAnimHash, false);
            
            _ownerEntity.gameObject.layer = _playerLayer;
            _ownerEntity.Rigidbody.velocity = Vector2.zero;
            _ownerEntity.IsDodge = false;
        }
    }
}
