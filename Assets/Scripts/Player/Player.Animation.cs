using System;
using System.Collections;
using UnityEngine;

namespace QT.InGame
{
    public partial class Player
    {
        private readonly int AnimationSwingHash = Animator.StringToHash("PlayerSwing");
        private readonly int AnimationThrowHash = Animator.StringToHash("PlayerThrow");
        private readonly int AnimationDeadHash = Animator.StringToHash("PlayerDead");

        public Animator Animator { get; private set; }
        
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
