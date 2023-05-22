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
        
        public PlayerDodgeState(IFSMEntity owner) : base(owner)
        {
        }
        
        public void InitializeState(Vector2 dir)
        {
            _ownerEntity.GetStatus(PlayerStats.DodgeCooldown).SetStatus(0);
            
            _ownerEntity.Animator.SetTrigger(AnimationDodgeHash);
            _ownerEntity.Animator.SetFloat(AnimationDirectionXHash, dir.x);
            _ownerEntity.Animator.SetFloat(AnimationDirectionYHash, dir.y);
            
            _ownerEntity.DodgeEffectPlay(dir);

            var force = _ownerEntity.GetStat(PlayerStats.DodgeAddForce).Value;

            _ownerEntity.Rigidbody.velocity = dir * force;

            var duration = _ownerEntity.GetStat(PlayerStats.DodgeDurationTime).Value;

            _ownerEntity.StartCoroutine(WaitSecond(duration));
        }
        
        public override void ClearState()
        {
            _ownerEntity.Animator.SetTrigger(AnimationDodgeEndHash);
            
            _ownerEntity.Rigidbody.velocity = Vector2.zero;
        }
        
        IEnumerator WaitSecond(float time)
        {
            yield return new WaitForSeconds(time);
            
            _ownerEntity.RevertToPreviousState();
        }
    }
}
