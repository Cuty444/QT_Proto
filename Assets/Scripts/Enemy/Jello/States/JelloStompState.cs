using System;
using System.Collections;
using System.Collections.Generic;
using QT.Core;
using QT.Sound;
using QT.Util;
using UnityEngine;

namespace QT.InGame
{
    [FSMState((int) Jello.States.Stomp)]
    public class JelloStompState : FSMState<Jello>
    {
        private enum StompState
        {
            Ready,
            OnAir,
            End
        }
        
        private static readonly int JumpReadyAnimHash = Animator.StringToHash("JumpReady");
        private static readonly int IsJumpingAnimHash = Animator.StringToHash("IsJumping");
        
        private SoundManager _soundManager;
        private readonly EnemyGameData _enemyData;
        private readonly JelloData _data;
        
        private Vector2 _dir;
        private Transform _transform;
        private Transform _target;
        private Transform _jelloObject;
        
        private float _speed;
        
        private StompState _state;
        private float _timer;

        private Vector2 _startPos;
        
        private int _repeatCount;
        

        public JelloStompState(IFSMEntity owner) : base(owner)
        {
            _enemyData = _ownerEntity.Data;
            _data = _ownerEntity.JelloData;
            _transform = _ownerEntity.transform;
        }

        public override void InitializeState()
        {
            _target = SystemManager.Instance.PlayerManager.Player.transform;
            _soundManager = SystemManager.Instance.SoundManager;
            _jelloObject = _ownerEntity.JelloObject;
            _repeatCount = 1;

            _startPos = _transform.position;
            
            Setting(_target.position);
        }
        
        public override void ClearState()
        {
            _ownerEntity.Shooter.ShootPoint = _ownerEntity.ShootPointTransform;
            
            _ownerEntity.Animator.ResetTrigger(JumpReadyAnimHash);
            _ownerEntity.Animator.SetBool(IsJumpingAnimHash, false);
            _jelloObject.localPosition = Vector2.zero;
            _ownerEntity.SetPhysics(true);
        }

        public override void UpdateState()
        {
            _timer += Time.deltaTime;
            
            switch (_state)
            {
                case StompState.Ready:
                    Ready();
                    break;
                case StompState.OnAir:
                    OnAir();
                    break;
                case StompState.End:
                    if (_timer > _data.StompEndDelay)
                    {
                        _ownerEntity.RevertToPreviousState();
                    }
                    break;
            }

        }

        private void Setting(Vector2 target)
        {
            _dir = (target - (Vector2)_transform.position).normalized;
            _ownerEntity.SetDir(_dir, 4);

            _speed = _data.StompAirSpeed;
            
            var hit = Physics2D.Raycast(_transform.position, _dir, float.PositiveInfinity, _ownerEntity.Shooter.BounceMask);
            if (hit.collider != null)
            {
                var speedToHit = hit.distance / _data.RushAirTime;
                if (speedToHit < _speed)
                {
                    _speed = speedToHit;
                }
            }
            
            _ownerEntity.Animator.SetTrigger(JumpReadyAnimHash);
            _soundManager.PlayOneShot(_soundManager.SoundData.Boss_RushReady, _ownerEntity.transform.position);
            
            _ownerEntity.Rigidbody.velocity = Vector2.zero;
            
            _state = StompState.Ready;
            _timer = 0;
        }
        
        private void Ready()
        {
            if (_timer > _data.StompReadyTime)
            {
                _ownerEntity.Animator.SetBool(IsJumpingAnimHash, true);
                
                _soundManager.PlayOneShot(_soundManager.SoundData.Boss_Rush, _ownerEntity.transform.position);
                _ownerEntity.SetPhysics(false);
                    
                _state = StompState.OnAir;
                _timer = 0;
            }
        }

        private void OnAir()
        {
            var time = _timer / _data.StompLengthTime;
            _transform.Translate(_dir * (_speed * Time.deltaTime));
            _jelloObject.localPosition = new Vector2(0, Mathf.Sin(Mathf.PI * time));
            
            if (_timer > _data.StompLengthTime)
            {
                _soundManager.PlayOneShot(_soundManager.SoundData.Boss_Motorcycle_End, _ownerEntity.transform.position);
                _ownerEntity.Animator.SetBool(IsJumpingAnimHash, false);
                _ownerEntity.SetPhysics(true);
                
                _ownerEntity.Shooter.ShootPoint = _transform;
                _ownerEntity.Shooter.Shoot(_data.StompShootId, AimTypes.World, ProjectileOwner.Boss);

                if (_repeatCount < _data.StompRepeatCount)
                {
                    _repeatCount++;

                    if (_repeatCount == _data.StompRepeatCount)
                    {
                        Setting(_startPos);
                    }
                    else
                    {
                        Setting(_target.position);
                    }
                }
                else
                {
                    _state = StompState.End;
                    _timer = 0;
                }
            }
        }

    }
}