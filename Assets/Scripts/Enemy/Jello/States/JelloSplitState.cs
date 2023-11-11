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
    [FSMState((int) Jello.States.Split)]
    public class JelloSplitState : FSMState<Jello>
    {
        private enum SplitState
        {
            Split,
            End
        }
        
        private static readonly int SplitAnimHash = Animator.StringToHash("Split");
        
        private SoundManager _soundManager;
        private readonly JelloData _data;
        
        private SplitState _state;
        private float _timer;
        
        private Bone _leftHandBone;
        private Bone _rightHandBone;
        
        public Transform _target;
        private Transform _transform;
        

        public JelloSplitState(IFSMEntity owner) : base(owner)
        {
            _data = _ownerEntity.JelloData;
            
            _leftHandBone = _ownerEntity.SkeletonRenderer.Skeleton.FindBone(_ownerEntity.LeftHandBoneName);
            _rightHandBone = _ownerEntity.SkeletonRenderer.Skeleton.FindBone(_ownerEntity.RightHandBoneName);
            _transform = _ownerEntity.transform;
        }

        public override void InitializeState()
        {
            _target = SystemManager.Instance.PlayerManager.Player.transform;
            _soundManager = SystemManager.Instance.SoundManager;

            _ownerEntity.Animator.SetTrigger(SplitAnimHash);
            
            _ownerEntity.Rigidbody.velocity = Vector2.zero;
            _state = SplitState.Split;
            _timer = 0;
        }
        
        public override void ClearState()
        {
        }

        public override void UpdateState()
        {
            _timer += Time.deltaTime;

            switch (_state)
            {
                case SplitState.Split:
                    if (_timer > _data.SplitDelay)
                    {
                        SpawnHands();
                        _state = SplitState.End;
                        _timer = 0;
                    }
                    break;
                case SplitState.End:

                    if (_timer > _data.SplitAfterDelay)
                    {
                        _ownerEntity.ChangeState(Jello.States.SplitMove);
                    }

                    break;
            }
            
           
        }

        private void SpawnHands()
        {
            Vector2 dir = (_target.position - _transform.position).normalized * 0.5f;
            
            _ownerEntity.LeftHand.gameObject.SetActive(true);
            _ownerEntity.LeftHand.Initialization(_data.LeftHandEnemyId);
            InitHand(_ownerEntity.LeftHand, _ownerEntity.LeftHand.transform, _leftHandBone, dir);
            
            _ownerEntity.RightHand.gameObject.SetActive(true);
            _ownerEntity.RightHand.Initialization(_data.RightHandEnemyId);
            InitHand(_ownerEntity.RightHand, _ownerEntity.RightHand.transform, _rightHandBone, dir);
        }

        private void InitHand(IProjectile projectile, Transform transform, Bone bone, Vector2 dir)
        {
            bone.ScaleX = 0;
            bone.ScaleY = 0;
            
            Vector2 targetPos = bone.GetWorldPosition(_transform);
            Vector2 spawnPos = _ownerEntity.ShootPointPivot.position;
            
            transform.parent = _transform.parent;
            transform.position = spawnPos;
            //
            projectile.ResetBounceCount(0);
            projectile.ResetProjectileDamage(25);

            dir += (targetPos - spawnPos).normalized;
            dir.Normalize();
            
            projectile.ProjectileHit(new ProjectileHitData(dir, _data.SplitShootSpeed, _ownerEntity.Shooter.BounceMask,
                ProjectileOwner.Boss, ProjectileProperties.None));
        }

    }
}