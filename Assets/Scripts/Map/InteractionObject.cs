using System;
using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;

namespace QT
{
    public class InteractionObject : MonoBehaviour, IHitAble
    {
        public int InstanceId => gameObject.GetInstanceID();
        public Vector2 Position => transform.position;
        [field: SerializeField] public float ColliderRad { get; private set; }
        public bool IsClearTarget => false;
      
        
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

        public void Hit(Vector2 dir, float power, AttackType attackType)
        {
            if (isHit)
                return;

            isHit = true;

            if (_circleCollider2D != null)
            {
                _circleCollider2D.enabled = false;
                _animator.SetTrigger(AnimationHitHash);
            }
        }
        
        public float GetHp()
        {
            return 0f;
        }
    }
}
