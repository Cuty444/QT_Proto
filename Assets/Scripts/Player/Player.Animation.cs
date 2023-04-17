using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace QT.Player
{
    public partial class Player
    {
        private readonly string AnimationSwingString = "PlayerSwing";
        private readonly string AnimationIdleString = "PlayerIdle";

        private Animator _animator;
        private const string _animatorValue = "MouseRotate";
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
        public void SetSwingAnimation(bool isCheck)
        {
            _animator.SetBool(AnimationSwingString,isCheck);
        }
    }
}
