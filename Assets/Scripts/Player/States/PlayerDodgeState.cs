using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QT.Core;

namespace QT.InGame
{
    [FSMState((int)Player.States.Dodge)]
    public class PlayerDodgeState : FSMState<Player>
    {
        private readonly int AnimationDodgeHash = Animator.StringToHash("PlayerDodge");
        private readonly int AnimationDodgeEndHash = Animator.StringToHash("PlayerDodgeEnd");
        private readonly int AnimationDirectionXHash = Animator.StringToHash("DirectionX");
        private readonly int AnimationDirectionYHash = Animator.StringToHash("DirectionY");
        private const string DodgeSoundPath = "Assets/Sound/QT/Assets/Player_Dash.wav";

        public PlayerDodgeState(IFSMEntity owner) : base(owner)
        {
        }
        
        public void InitializeState(Vector2 dir)
        {
            _ownerEntity.GetStatus(PlayerStats.DodgeCooldown).SetStatus(0);
            _ownerEntity.GetStatus(PlayerStats.DodgeInvincibleTime).SetStatus(0);
            _ownerEntity.Animator.SetTrigger(AnimationDodgeHash);
            SystemManager.Instance.SoundManager.PlayOneShot(DodgeSoundPath);
            float tempX = dir.x;
            if (dir.y is <= 0.2f and >= -0.2f)
            {
                if (dir.x > 0f)
                {
                    dir.x = 1f;
                }
                else if(dir.x < 0f)
                {
                    dir.x = -1f;
                }

                dir.y = 0f;
            }
            else
            {
                if (dir.x > 0f)
                {
                    dir.x = 0.7071f;
                }
                else if(dir.x < 0f)
                {
                    dir.x = -0.7071f;
                }
            }
            if (tempX is <= 0.2f and >= -0.2f)
            {
                if (dir.y > 0f)
                {
                    dir.y = 1f;
                }
                else if (dir.y < 0f)
                {
                    dir.y = -1f;
                }

                dir.x = 0f;
            }
            else
            {
                if (dir.y > 0f)
                {
                    dir.y = 0.7071f;
                }
                else if (dir.y < 0f)
                {
                    dir.y = -0.7071f;
                }
            }

            if (dir == Vector2.zero)
            {
                _ownerEntity.RevertToPreviousState();
                return;
            }
            
            _ownerEntity.Animator.SetFloat(AnimationDirectionXHash, dir.x);

            _ownerEntity.Animator.SetFloat(AnimationDirectionYHash, dir.y);

            _ownerEntity.IsDodge = true;
            _ownerEntity.IsFlip = dir.x > 0;
            _ownerEntity.DodgeEffectPlay(dir);

            var force = _ownerEntity.GetStat(PlayerStats.DodgeAddForce).Value;

            _ownerEntity.Rigidbody.velocity = dir * force;

            var duration = _ownerEntity.GetStat(PlayerStats.DodgeDurationTime).Value;

            _ownerEntity.StartCoroutine( Util.UnityUtil.WaitForFunc(() =>
            {
                _ownerEntity.RevertToPreviousState();
            },duration));
        }
        
        public override void ClearState()
        {
            _ownerEntity.Animator.SetTrigger(AnimationDodgeEndHash);
            
            _ownerEntity.Rigidbody.velocity = Vector2.zero;
            _ownerEntity.IsDodge = false;
        }
    }
}
