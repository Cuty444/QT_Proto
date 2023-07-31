using System.Collections;
using System.Collections.Generic;
using QT.Util;
using Spine.Unity;
using UnityEngine;

namespace QT
{
    public class HitLoopObject : InteractionObject
    {
        private void Awake()
        {
            _skeletonMecanim = GetComponentInChildren<SkeletonMecanim>();
            _animator = GetComponentInChildren<Animator>();
        }
        
        public void Hit(Vector2 dir, float power,AttackType attackType)
        {
            if (isHit)
                return;
            
            _animator.SetTrigger(AnimationHitHash);
            isHit = true;
            StartCoroutine(UnityUtil.WaitForFunc(() =>
            {
                _animator.ResetTrigger(AnimationHitHash);
            }, 0.2f));
            StartCoroutine(UnityUtil.WaitForFunc(() =>
            {
                isHit = false;
            }, 1.0f));
        }
    }
}
