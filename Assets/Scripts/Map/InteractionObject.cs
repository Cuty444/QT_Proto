using System;
using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;

namespace QT
{
    public class InteractionObject : MonoBehaviour, IHitable
    {
        protected bool isHit = false;
        protected SkeletonMecanim _skeletonMecanim;
        protected Animator _animator;
        protected readonly int AnimationHitHash = Animator.StringToHash("Hit");

        private CircleCollider2D _circleCollider2D;

        private void Awake()
        {
            _skeletonMecanim = GetComponentInChildren<SkeletonMecanim>();
            _animator = GetComponentInChildren<Animator>();
            _circleCollider2D = GetComponent<CircleCollider2D>();
        }

        public void Hit(Vector2 dir, float power,AttackType attackType)
        {
            if (isHit)
                return;
            
            _animator.SetTrigger(AnimationHitHash);
            isHit = true;
            _circleCollider2D.enabled = false;
        }
    }
}
