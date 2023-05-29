using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QT
{
    public class DoorAnimator : MonoBehaviour
    {
        private Animator _animator;
        private readonly int AnimationOpenHash = Animator.StringToHash("Open");

        private void Awake()
        {
            _animator = GetComponentInChildren<Animator>();
        }

        public void DoorOpen()
        {
            _animator.SetTrigger(AnimationOpenHash);
        }
    }
}
