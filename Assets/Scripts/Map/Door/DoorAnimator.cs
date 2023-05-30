using System;
using System.Collections;
using System.Collections.Generic;
using QT.Map;
using Spine.Unity;
using UnityEngine;

namespace QT
{
    public class DoorAnimator : MonoBehaviour
    {
        private Animator _animator;
        private SkeletonMecanim _skeletonMecanim;
        private readonly int AnimationOpenHash = Animator.StringToHash("Open");

        private void Awake()
        {
            _animator = GetComponentInChildren<Animator>();
            _skeletonMecanim = GetComponentInChildren<SkeletonMecanim>();
        }

        public void DoorUpDown(MapDirection direction)
        {
            switch (direction)
            {
                case MapDirection.Up:
                    _skeletonMecanim.skeleton.SetSkin("Skin1");
                    _skeletonMecanim.skeleton.SetSlotsToSetupPose();
                    break;
                case MapDirection.Down:
                    _skeletonMecanim.skeleton.SetSkin("Skin2");
                    _skeletonMecanim.skeleton.SetSlotsToSetupPose();
                    break;
            }
        }

        public void DoorOpen()
        {
            _animator.SetTrigger(AnimationOpenHash);
        }
    }
}
