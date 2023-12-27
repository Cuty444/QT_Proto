using System;
using System.Collections;
using System.Collections.Generic;
using QT.Core;
using QT.Sound;
using QT.Util;
using UnityEngine;

namespace QT.InGame
{
    [FSMState((int) JelloRightHand.States.Rush)]
    public class JelloRightHandRushState : FSMState<JelloRightHand>
    {
        private enum RushState
        {
            Ready,
            Rushing,
            End
        }
        
        private static readonly int RushReadyAnimHash = Animator.StringToHash("RushReady");
        private static readonly int IsRushingAnimHash = Animator.StringToHash("IsRushing");

        private readonly LayerMask _bounceMask;
        
        private SoundManager _soundManager;
        private readonly JelloHandData _data;
        
        
        private float _speed;
        private float _size;
        private float _damage;
        
        private Vector2 _dir;
        private Transform _target;
        private Transform _transform;
        
        
        private RushState _state;
        private float _timer;

        private int _bounceCount;
        

        public JelloRightHandRushState(IFSMEntity owner) : base(owner)
        {
            _data = _ownerEntity.JelloData;
            _transform = _ownerEntity.transform;
            _bounceMask = _ownerEntity.Shooter.BounceMask;
        }

        public override void InitializeState()
        {
            _speed = _data.RushStartSpeed;
            _size = _ownerEntity.ColliderRad * 0.5f;
            _damage = _data.RushHitDamage;

            _soundManager = SystemManager.Instance.SoundManager;

            _target = SystemManager.Instance.PlayerManager.Player.transform;
            
            _dir = (_target.position - _transform.position).normalized;
            _ownerEntity.SetDir(_dir, 4);
            
            _ownerEntity.Animator.SetTrigger(RushReadyAnimHash);
            _soundManager.PlayOneShot(_soundManager.SoundData.Jello_RushReady, _ownerEntity.transform.position);
            
            _ownerEntity.Rigidbody.velocity = Vector2.zero;
            
            _state = RushState.Ready;
            _bounceCount = 0;
            _timer = 0;
        }
        
        public override void ClearState()
        {
            _ownerEntity.Animator.ResetTrigger(RushReadyAnimHash);
            _ownerEntity.Animator.SetBool(IsRushingAnimHash, false);
            _ownerEntity.SetPhysics(true);
        }

        public override void UpdateState()
        {
            _timer += Time.deltaTime;
            
            switch (_state)
            {
                case RushState.Ready:
                    Ready();
                    break;
                case RushState.Rushing:
                    Rushing();
                    break;
                case RushState.End:
                    if (_timer > _data.RushEndDelay)
                    {
                        _ownerEntity.ChangeState(Jello.States.Normal);
                    }
                    break;
            }

        }

        public override void FixedUpdateState()
        {
            if (_state != RushState.Rushing)
            {
                return;
            }
            
            if (_timer > _data.RushHitCheckDelay)
            {
                CheckHit();

                if (_bounceCount > _data.RushBounceCount)
                {
                    _ownerEntity.Animator.SetBool(IsRushingAnimHash, false);
                    _state = RushState.End;
                    _timer = 0;
                }
            }
        }

        private void CheckHit()
        {
            var hit = Physics2D.CircleCast(_transform.position, _size, _dir, _speed * Time.deltaTime, _bounceMask);

#if UNITY_EDITOR
            Debug.DrawRay(_transform.position, _dir * (_size + _speed * Time.deltaTime), Color.magenta, 1);
            Debug.DrawRay(_transform.position, new Vector3(-_dir.y, _dir.x) * (_size), Color.magenta, 1);
            Debug.DrawRay(_transform.position, new Vector3(_dir.y, -_dir.x) * (_size), Color.magenta, 1);
#endif

            if (hit.collider == null)
            {
                return;
            }
            
            if (hit.collider.TryGetComponent(out IHitAble hitable))
            {
                hitable.Hit(_dir, _damage);
            }

            
            Vector2 targetDir = (_target.transform.position - _transform.position).normalized;
            _dir = Vector2.Reflect(_dir, hit.normal);
            
             _dir = Vector3.RotateTowards(_dir, targetDir, _data.RushReflectCorrection * Mathf.Deg2Rad, 0);
             _ownerEntity.SetDir(_dir,4);
            
            _transform.Translate(hit.normal * _size);
            
            //_soundManager.PlayOneShot(_soundManager.SoundData.Boss_Rush_Crash, _ownerEntity.transform.position);
            _soundManager.PlayOneShot(_soundManager.SoundData.Jello_Hit, _ownerEntity.transform.position);
            
            _bounceCount++;
        }
        
        private void Ready()
        {
            if (_timer > _data.RushReadyTime)
            {
                _ownerEntity.Animator.SetBool(IsRushingAnimHash, true);
                
                _soundManager.PlayOneShot(_soundManager.SoundData.Jello_RushReady, _ownerEntity.transform.position);
                _ownerEntity.SetPhysics(false);
                _timer = 0;
                    
                _state = RushState.Rushing;
            }
        }

        private void Rushing()
        {
            var progress = _timer / _data.RushLengthTime;
            progress *= progress; // easeInQuad
            
            _speed = Mathf.Lerp(_data.RushStartSpeed, _data.RushEndSpeed, progress);
            
            _transform.Translate(_dir * (_speed * Time.deltaTime));
            
            if (_timer > _data.RushLengthTime)
            {
                _soundManager.PlayOneShot(_soundManager.SoundData.Jello_Land, _ownerEntity.transform.position);
                
                _ownerEntity.ChangeState(Jello.States.Normal);
                _timer = 0;
            }
        }
        
    }
}