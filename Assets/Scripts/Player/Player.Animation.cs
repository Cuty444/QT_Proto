using System;
using System.Collections;
using UnityEngine;

namespace QT.Player
{
    public partial class Player
    {
        private readonly string AnimationIdleString = "PlayerIdle";
        private readonly int AnimationSwingHash = Animator.StringToHash("PlayerSwing");
        private readonly int AnimationThrowHash = Animator.StringToHash("PlayerThrow");
        private readonly int AnimationDodgeHash = Animator.StringToHash("PlayerDodge");
        private readonly int AnimationDodgeEndHash = Animator.StringToHash("PlayerDodgeEnd");
        private readonly int AnimationRigidHash = Animator.StringToHash("PlayerRigid");
        private readonly int AnimationDeadHash = Animator.StringToHash("PlayerDead");
        private readonly int AnimationDirectionXHash = Animator.StringToHash("DirectionX");
        private readonly int AnimationDirectionYHash = Animator.StringToHash("DirectionY");

        private Animator _animator;
        private const string _animatorValue = "MouseRotate";
        private bool _isRigid;
        private bool _isDodge = false;
        public void AngleAnimation()
        {
            float playerAngleDegree = QT.Util.Math.GetDegree(transform.position, MousePos);
            float yAngle = 0f;
            switch (playerAngleDegree)
            {
                case > 22.5f and < 67.5f:
                    yAngle = 180f;
                    _animator.SetFloat(_animatorValue,3f);
                    break;
                case > 67.5f and < 112.5f:
                    _animator.SetFloat(_animatorValue,4f);
                    break;
                case > 112.5f and < 157.5f:
                    _animator.SetFloat(_animatorValue,3f);
                    break;
                case > -157.5f and < -112.5f:
                    _animator.SetFloat(_animatorValue,1f);
                    break;
                case > -112.5f and < -67.5f:
                    _animator.SetFloat(_animatorValue,0f);
                    break;
                case > -67.5f and < -22.5f:
                    yAngle = 180f;
                    _animator.SetFloat(_animatorValue,1f);
                    break;
                case > 157.5f:
                case < -157.5f:
                    _animator.SetFloat(_animatorValue,2f);
                    break;
                default:
                    yAngle = 180f;
                    _animator.SetFloat(_animatorValue,2f);
                    break;
            }
            
            _animator.transform.rotation = Quaternion.Euler(0f, yAngle,0f);
        }

        public void SetMoveCheck(bool isCheck)
        {
            _animator.SetBool(AnimationIdleString,isCheck);
        }
        public void SetSwingAnimation()
        {
            _animator.SetTrigger(AnimationSwingHash);
            StartCoroutine(WaitForSecond(0.2f, () =>
            {
                _animator.ResetTrigger(AnimationThrowHash);
            }));
        }

        public void SetThrowAnimation()
        {
            _animator.SetTrigger(AnimationThrowHash);
            StartCoroutine(WaitForSecond(0.5f, () =>
            {
                _animator.ResetTrigger(AnimationThrowHash);
            }));
        }

        public void SetDodgeAnimation()
        {
            _animator.SetTrigger(AnimationDodgeHash);
            _animator.SetFloat(AnimationDirectionXHash,BefereDodgeDirecton.x);
            _animator.SetFloat(AnimationDirectionYHash,BefereDodgeDirecton.y);
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
                _animator.ResetTrigger(AnimationDodgeHash);
            }));
            _animator.transform.rotation = Quaternion.Euler(0f, yAngle,0f);
            DodgeEffectRotation(yAngle);
        }

        public void SetDodgeEndAnimation()
        {
            _animator.SetTrigger(AnimationDodgeEndHash);
            _isDodge = false;
            StartCoroutine(WaitForSecond(0.5f, () =>
            {
                _animator.ResetTrigger(AnimationDodgeEndHash);
            }));
        }

        public void SetRigidAnimation()
        {
            _animator.SetTrigger(AnimationRigidHash);
            ChangeState(Player.States.Move);
            _isRigid = true;
            StartCoroutine(WaitForSecond(0.33f, () =>
            {
                _animator.ResetTrigger(AnimationRigidHash);
                _isRigid = false;
            }));
        }
        
        public void SetDeadAnimation()
        {
            _animator.SetTrigger(AnimationDeadHash);
        }

        public bool GetRigidTrigger()
        {
            return _isRigid;
        }

        IEnumerator WaitForSecond(float time,Action func)
        {
            yield return new WaitForSeconds(time);
            func.Invoke();
        }
    }
}
