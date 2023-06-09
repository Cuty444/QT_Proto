using System;
using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;

namespace QT
{
    public class InteractionObject : MonoBehaviour, IHitable
    {
        private bool isHit = false;
        private SkeletonMecanim _skeletonMecanim;
        private Animator _animator;
        private readonly int AnimationHitHash = Animator.StringToHash("Hit");

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
        }
    }
}
