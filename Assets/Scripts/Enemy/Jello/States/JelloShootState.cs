using System;
using System.Collections;
using System.Collections.Generic;
using QT.Core;
using QT.Sound;
using QT.Util;
using UnityEngine;

namespace QT.InGame
{
    [FSMState((int) Jello.States.Shoot)]
    public class JelloShootState : FSMState<Jello>
    {
        private enum ShootState
        {
            Ready,
            Right,
            Left,
            Final,
            After
        }
        
        
        private static readonly int ShootReadyAnimHash = Animator.StringToHash("ShootReady");
        private static readonly int ShootAnimHash = Animator.StringToHash("Shoot");
        
        private readonly EnemyGameData _enemyData;
        private readonly JelloData _data;
        
        private ShootState _state;
        private float _timer;

        public JelloShootState(IFSMEntity owner) : base(owner)
        {
            _enemyData = _ownerEntity.Data;
            _data = _ownerEntity.JelloData;
        }

        public override void InitializeState()
        {
            _ownerEntity.Rigidbody.constraints = RigidbodyConstraints2D.FreezeAll;

            _ownerEntity.Animator.SetTrigger(ShootReadyAnimHash);
            _state = ShootState.Ready;
            _timer = 0;
        }

        public override void UpdateState()
        {
            _timer += Time.deltaTime;

            switch (_state)
            {
                case ShootState.Ready:
                    if (_timer > _data.ShootReadyDelay)
                    {
                        _ownerEntity.Animator.SetTrigger(ShootAnimHash);
                        _state = ShootState.Right;
                        _timer = 0;
                    }
                    break;
                case ShootState.Right:
                    if (_timer > _data.ShootRightHandDelay)
                    {
                        if (!_ownerEntity.RightHand.IsDead)
                        {
                            _ownerEntity.Shooter.ShootPoint = _ownerEntity.RightHandTransform;
                            _ownerEntity.Shooter.Shoot(_data.ShootRightHandShootId, AimTypes.World,
                                ProjectileOwner.Boss);
                        }

                        _state = ShootState.Left;
                        _timer = 0;
                    }
                    break;
                case ShootState.Left:
                    if (_timer > _data.ShootLeftHandDelay)
                    {
                        if (!_ownerEntity.LeftHand.IsDead)
                        {
                            _ownerEntity.Shooter.ShootPoint = _ownerEntity.LeftHandTransform;
                            _ownerEntity.Shooter.Shoot(_data.ShootLeftHandShootId, AimTypes.World,
                                ProjectileOwner.Boss);
                        }

                        _state = ShootState.Final;
                        _timer = 0;
                    }
                    break;
                case ShootState.Final:
                    if (_timer > _data.ShootFinalDelay)
                    {
                        _ownerEntity.Shooter.ShootPoint = _ownerEntity.ShootPointPivot;
                        _ownerEntity.Shooter.Shoot(_data.ShootFinalShootId, AimTypes.Target, ProjectileOwner.Boss);

                        if (!_ownerEntity.LeftHand.IsDead && !_ownerEntity.RightHand.IsDead)
                        {
                            _ownerEntity.Shooter.Shoot(_data.ShootFinalShootId2, AimTypes.Target, ProjectileOwner.Boss);
                        }

                        _state = ShootState.After;
                        _timer = 0;
                    }
                    break;
                case ShootState.After:
                    if (_timer > _data.ShootAfterDelay)
                    {
                        _ownerEntity.RevertToPreviousState();
                    }
                    break;
            }

        }

        public override void FixedUpdateState()
        {
            _ownerEntity.Rigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
        }

        public override void ClearState()
        {
            _ownerEntity.Shooter.ShootPoint = _ownerEntity.ShootPointPivot;
        }
        
    }
}