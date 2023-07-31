using System;
using System.Collections;
using System.Collections.Generic;
using QT.Core;
using QT.Util;
using Spine.Unity;
using UnityEngine;

namespace QT
{
    public class NpcInteraction : MonoBehaviour, IHitable
    {
        private readonly int AnimationHitHash = Animator.StringToHash("Hit");
        private readonly int AnimationSoldHash = Animator.StringToHash("Sold");

        public int InstanceId => gameObject.GetInstanceID();
        public Vector2 Position => transform.position;
        [field: SerializeField] public float ColliderRad { get; private set; }

        private Animator _animator;

        private void Awake()
        {
            _animator = GetComponentInChildren<Animator>();
            SystemManager.Instance.PlayerManager.AddItemEvent.AddListener(Sold);
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
        
        public float GetHp()
        {
            return 0f;
        }
    }
}
