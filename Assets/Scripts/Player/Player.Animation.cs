using System;
using System.Collections;
using UnityEngine;

namespace QT.InGame
{
    public partial class Player
    {
        private readonly int AnimationSwingHash = Animator.StringToHash("PlayerSwing");
        private readonly int AnimationThrowHash = Animator.StringToHash("PlayerThrow");
        private readonly int AnimationDodgeHash = Animator.StringToHash("PlayerDodge");
        private readonly int AnimationDodgeEndHash = Animator.StringToHash("PlayerDodgeEnd");
        private readonly int AnimationRigidHash = Animator.StringToHash("PlayerRigid");
        private readonly int AnimationDeadHash = Animator.StringToHash("PlayerDead");

        public Animator Animator { get; private set; }
        
        private bool _isRigid;
        private bool _isDodge = false;
        
        
        public void SetSwingAnimation()
        {
            Animator.SetTrigger(AnimationSwingHash);
            StartCoroutine(WaitForSecond(0.2f, () =>
            {
                Animator.ResetTrigger(AnimationThrowHash);
            }));
        }

        public void SetThrowAnimation()
        {
            Animator.SetTrigger(AnimationThrowHash);
            StartCoroutine(WaitForSecond(0.5f, () =>
            {
                Animator.ResetTrigger(AnimationThrowHash);
            }));
        }


        public void SetRigidAnimation()
        {
            Animator.SetTrigger(AnimationRigidHash);
            ChangeState(Player.States.Move);
            _isRigid = true;
            StartCoroutine(WaitForSecond(0.33f, () =>
            {
                Animator.ResetTrigger(AnimationRigidHash);
                _isRigid = false;
            }));
        }
        
        public void SetDeadAnimation()
        {
            Animator.SetTrigger(AnimationDeadHash);
        }

        IEnumerator WaitForSecond(float time,Action func)
        {
            yield return new WaitForSeconds(time);
            func.Invoke();
        }
    }
}
