using System;
using System.Collections;
using System.Collections.Generic;
using QT.Core;
using QT.Sound;
using QT.Util;
using UnityEngine;

namespace QT.InGame
{
    [FSMState((int) Jello.States.Rush)]
    public class JelloRushState : FSMState<Jello>
    {
        private enum RushState
        {
            Ready,
            Rushing,
            OnAir,
            End
        }
        
        private static readonly int RushReadyAnimHash = Animator.StringToHash("RushReady");
        private static readonly int IsRushingAnimHash = Animator.StringToHash("IsRushing");
        
        private SoundManager _soundManager;
        private readonly EnemyGameData _enemyData;
        private readonly JelloData _data;
        
        
        private float _speed;
        private float _size;
        private float _damage;
        
        private Vector2 _dir;
        private Transform _transform;
        private Transform _rushCenter;
        
        
        private RushState _state;
        private float _timer;
        

        public JelloRushState(IFSMEntity owner) : base(owner)
        {
            _enemyData = _ownerEntity.Data;
            _data = _ownerEntity.JelloData;
            _transform = _ownerEntity.transform;
            _rushCenter = _ownerEntity.ShootPointPivot;
        }

        public override void InitializeState()
        {
            _speed = _data.RushSpeed;
            _size = _ownerEntity.ColliderRad;
            _damage = _data.RushHitDamage;

            _soundManager = SystemManager.Instance.SoundManager;

            _dir = (SystemManager.Instance.PlayerManager.Player.transform.position - _transform.position).normalized;
            _ownerEntity.SetDir(_dir, 4);
            
            _ownerEntity.Animator.SetTrigger(RushReadyAnimHash);
            _soundManager.PlayOneShot(_soundManager.SoundData.Boss_RushReady, _ownerEntity.transform.position);
            
            _ownerEntity.Rigidbody.velocity = Vector2.zero;
            
            _state = RushState.Ready;
            _timer = 0;
        }
        
        public override void ClearState()
        {
            _ownerEntity.Shooter.StopAttack();
            
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
                case RushState.OnAir:
                    OnAir();
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
            
            if (_timer > _data.RushHitCheckDelay && CheckHit())
            {
                _ownerEntity.Shooter.StopAttack();
                _ownerEntity.Animator.SetBool(IsRushingAnimHash, false);
                _state = RushState.OnAir;
                _timer = 0;
            }
        }

        private bool CheckHit()
        {
            var hits = Physics2D.CircleCastAll(_rushCenter.position, _size, _dir, _speed * Time.deltaTime,
                _ownerEntity.HitMask);

#if UNITY_EDITOR
            Debug.DrawRay(_rushCenter.position, _dir * (_size + _speed * Time.deltaTime), Color.magenta, 1);
            Debug.DrawRay(_rushCenter.position, new Vector3(-_dir.y, _dir.x) * (_size), Color.magenta, 1);
            Debug.DrawRay(_rushCenter.position, new Vector3(_dir.y, -_dir.x) * (_size), Color.magenta, 1);
#endif
            
            foreach (var hit in hits)
            {
                if (hit.collider != null)
                {
                    if (hit.collider.TryGetComponent(out IHitAble hitable))
                    {
                        hitable.Hit(_dir, _damage);
                    }

                    // var normal = hit.normal;
                    // var angle = Mathf.Atan2(normal.y, normal.x) * Mathf.Rad2Deg - 90;
                    //
                    // SystemManager.Instance.ResourceManager.EmitParticle(ShockEffectPath, hit.point, angle);
                    // _ownerEntity.RushShockImpulseSource.GenerateImpulse(normal * 3);
                }
            }

            if (hits.Length > 0)
            {
                _soundManager.PlayOneShot(_soundManager.SoundData.Boss_Rush_Crash, _ownerEntity.transform.position);
                _soundManager.PlayOneShot(_soundManager.SoundData.Boss_Motorcycle_End, _ownerEntity.transform.position);
                return true;
            }

            return false;
        }
        
        private void Ready()
        {
            if (_timer > _data.RushReadyTime)
            {
                _ownerEntity.Shooter.PlayEnemyAtkSequence(_data.RushAtkId, ProjectileOwner.Boss);
                
                _ownerEntity.Animator.SetBool(IsRushingAnimHash, true);
                
                _soundManager.PlayOneShot(_soundManager.SoundData.Boss_Rush, _ownerEntity.transform.position);
                _ownerEntity.SetPhysics(false);
                _timer = 0;
                    
                _state = RushState.Rushing;
            }
        }

        private void Rushing()
        {
            _transform.Translate(_dir * (_data.RushSpeed * Time.deltaTime));
            
            if (_timer > _data.RushLengthTime)
            {
                _soundManager.PlayOneShot(_soundManager.SoundData.Boss_Motorcycle_End, _ownerEntity.transform.position);
                
                _ownerEntity.ChangeState(Jello.States.Normal);
                _timer = 0;
            }
        }
        
        
        private void OnAir()
        {
            _transform.Translate(-_dir * (_data.RushAirSpeed * Time.deltaTime));
            
            if (_timer > _data.RushAirTime)
            {
                _state = RushState.End;
                _timer = 0;
            }
        }
        
    }
}