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
        private readonly int AnimationDirectionXHash = Animator.StringToHash("DirectionX");
        private readonly int AnimationDirectionYHash = Animator.StringToHash("DirectionY");

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

        public void SetDodgeAnimation()
        {
            Animator.SetTrigger(AnimationDodgeHash);
            Animator.SetFloat(AnimationDirectionXHash,BefereDodgeDirecton.x);
            Animator.SetFloat(AnimationDirectionYHash,BefereDodgeDirecton.y);
            float yAngle = 0f;
            if (BefereDodgeDirecton.x < -0.5f)
            {
                _dodgeDirection = 1;

            }
            else if (BefereDodgeDirecton.x > 0.5f)
            {
                _dodgeDirection = 1;
                yAngle = 180f;
            }
            else
            {
                if (BefereDodgeDirecton.y == -1f || BefereDodgeDirecton.y == 1f)
                {
                    _dodgeDirection = 0;
                        
                }
            }
            StartCoroutine(WaitForSecond(0.5f, () =>
            {
                Animator.ResetTrigger(AnimationDodgeHash);
            }));
            Animator.transform.rotation = Quaternion.Euler(0f, yAngle,0f);
            DodgeEffectRotation(yAngle);
        }

        public void SetDodgeEndAnimation()
        {
            Animator.SetTrigger(AnimationDodgeEndHash);
            _isDodge = false;
            StartCoroutine(WaitForSecond(0.5f, () =>
            {
                Animator.ResetTrigger(AnimationDodgeEndHash);
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
