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
    [FSMState((int) Jello.States.Restore)]
    public class JelloRestoreState : FSMState<Jello>
    {
        private enum RestoreState
        {
            Ready,
            End
        }
        
        private static readonly int RestoreAnimHash = Animator.StringToHash("Restore");
        
        private SoundManager _soundManager;
        private readonly JelloData _data;
        
        private RestoreState _state;
        private float _timer;
        
        private Bone _leftHandBone;
        private Bone _rightHandBone;
        

        public JelloRestoreState(IFSMEntity owner) : base(owner)
        {
            _data = _ownerEntity.JelloData;
            
            _leftHandBone = _ownerEntity.SkeletonRenderer.Skeleton.FindBone(_ownerEntity.LeftHandBoneName);
            _rightHandBone = _ownerEntity.SkeletonRenderer.Skeleton.FindBone(_ownerEntity.RightHandBoneName);
        }

        public override void InitializeState()
        {
            _ownerEntity.Animator.SetTrigger(RestoreAnimHash);
            
            _state = RestoreState.Ready;
            _timer = 0;
        }
        
        public override void UpdateState()
        {
            _timer += Time.deltaTime;
            
            switch (_state)
            {
                case RestoreState.Ready:
                    if (_timer > _data.RestoreDelay)
                    {
                        RestoreHands();
                        _state = RestoreState.End;
                        _timer = 0;
                    }
                    break;
                case RestoreState.End:
                    if (_timer > _data.RestoreAfterDelay)
                    {
                        _ownerEntity.ChangeState(Jello.States.Normal);
                    }
                    break;
            }
        }

        private void RestoreHands()
        {
            _leftHandBone.ScaleX = 1;
            _leftHandBone.ScaleY = 1;
            
            _ownerEntity.LeftHand.HP.SetStatus(100);
            
            _rightHandBone.ScaleX = 1;
            _rightHandBone.ScaleY = 1;
            
            _ownerEntity.RightHand.HP.SetStatus(100);
        }

    }
}