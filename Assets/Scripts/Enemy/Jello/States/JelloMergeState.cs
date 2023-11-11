using System;
using System.Collections;
using System.Collections.Generic;
using QT.Core;
using QT.Sound;
using QT.Util;
using Spine;
using Spine.Unity;
using UnityEngine;

namespace QT.InGame
{
    [FSMState((int) Jello.States.Merge)]
    public class JelloMergeState : FSMState<Jello>
    {
        private enum SplitState
        {
            Merge,
            AfterDelay,
            Restore,
            End
        }
        
        private static readonly int MergeAnimHash = Animator.StringToHash("Restore");
        
        private SoundManager _soundManager;
        private readonly JelloData _data;
        
        private SplitState _state;
        private float _timer;
        
        private Bone _leftHandBone;
        private Bone _rightHandBone;

        public JelloMergeState(IFSMEntity owner) : base(owner)
        {
            _data = _ownerEntity.JelloData;
            
            _leftHandBone = _ownerEntity.SkeletonRenderer.Skeleton.FindBone(_ownerEntity.LeftHandBoneName);
            _rightHandBone = _ownerEntity.SkeletonRenderer.Skeleton.FindBone(_ownerEntity.RightHandBoneName);
        }

        public override void InitializeState()
        {
            if (_state == SplitState.Restore)
            {
                _ownerEntity.ChangeState(Jello.States.Restore);
                _state = SplitState.End;
            }
            else
            {
                _soundManager = SystemManager.Instance.SoundManager;
                _ownerEntity.Rigidbody.velocity = Vector2.zero;

                _ownerEntity.SetDir(Vector2.down, 4);
                
                CallHands();
            }
        }
        
        public override void ClearState()
        {
        }

        public override void UpdateState()
        {
            _timer += Time.deltaTime;

            switch (_state)
            {
                case SplitState.Merge:
                    if (_timer > _data.MergeDelay)
                    {
                        CollectHands();
                        _ownerEntity.Animator.SetTrigger(MergeAnimHash);
                        _state = SplitState.AfterDelay;
                        _timer = 0;
                    }
                    break;
                case SplitState.AfterDelay:
                    if (_timer > _data.MergeAfterDelay)
                    {
                        _ownerEntity.ChangeState(Jello.States.Shoot);
                        
                        _state = SplitState.Restore;
                        _timer = 0;
                    }
                    break;
            }
            
        }

        private void CallHands()
        {
            Vector2 targetPos = _ownerEntity.ShootPointPivot.position;
            
            var rightHand = _ownerEntity.RightHand;
            if (!rightHand.IsDead)
            {
                (rightHand.ChangeState(JelloRightHand.States.Return) as JelloRightHandReturnState).InitializeState(targetPos, _data.MergeDelay);
            }

            var leftHand = _ownerEntity.LeftHand;
            if (!leftHand.IsDead)
            {
                (leftHand.ChangeState(JelloLeftHand.States.Return) as JelloLeftHandReturnState).InitializeState(targetPos, _data.MergeDelay);
            }
            
            _state = SplitState.Merge;
            _timer = 0;
        }
        
        private void CollectHands()
        {
            float healAmount = _ownerEntity.HP.Value * _data.MergeHealAmount;
            float heal = 0;

            var rightHand = _ownerEntity.RightHand;

            if (!rightHand.IsDead)
            {
                _rightHandBone.ScaleX = 1;
                _rightHandBone.ScaleY = 1;
                heal += healAmount;
            }
            
            rightHand.gameObject.SetActive(false);
            rightHand.transform.parent = _ownerEntity.transform;

            
            var leftHand = _ownerEntity.LeftHand;
            
            if (!leftHand.IsDead)
            {
                _leftHandBone.ScaleX = 1;
                _leftHandBone.ScaleY = 1;
                heal += healAmount;
            }

            leftHand.gameObject.SetActive(false);
            leftHand.transform.parent = _ownerEntity.transform;
            
            _ownerEntity.Heal(heal);
        }

    }
}