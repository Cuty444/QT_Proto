using System;
using System.Collections;
using System.Collections.Generic;
using QT.Core;
using QT.Util;
using Spine.Unity;
using UnityEngine;

namespace QT
{
    public class NpcInteraction : MonoBehaviour, IHitAble
    {
        private readonly int AnimationHitHash = Animator.StringToHash("Hit");
        private readonly int AnimationSoldHash = Animator.StringToHash("Sold");

        public int InstanceId => gameObject.GetInstanceID();
        public Vector2 Position => transform.position;
        [field: SerializeField] public float ColliderRad { get; private set; }
        public bool IsClearTarget => false;
        public bool IsDead => false;

        private Animator _animator;

        private void Awake()
        {
            _animator = GetComponentInChildren<Animator>();
            SystemManager.Instance.PlayerManager.AddItemEvent.AddListener(Sold);
        }

        private void OnEnable()
        {
            HitAbleManager.Instance.Register(this);
        }

        private void OnDisable()
        {
            HitAbleManager.Instance.UnRegister(this);
        }

        private void OnDestroy()
        {
            SystemManager.Instance?.PlayerManager.AddItemEvent.RemoveListener(Sold);
        }

        private void Hit()
        {
            _animator.SetTrigger(AnimationHitHash);
            StartCoroutine(UnityUtil.WaitForFunc(() =>
            {
                _animator.ResetTrigger(AnimationHitHash);
            }, 0.1f));
        }

        private void Sold()
        {
            if (!gameObject.activeInHierarchy)
            {
                return;
            }
            
            _animator.SetTrigger(AnimationSoldHash);
            StartCoroutine(UnityUtil.WaitForFunc(() =>
            {
                _animator.ResetTrigger(AnimationSoldHash);
            }, 0.1f));
        }
        
        public void Hit(Vector2 dir, float power,AttackType attackType)
        {
            Hit();
        }
    }
}
