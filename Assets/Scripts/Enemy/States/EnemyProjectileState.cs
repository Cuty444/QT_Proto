using System.Collections;
using System.Collections.Generic;
using QT.Core;
using QT.Core.Data;
using UnityEngine;

namespace QT.Enemy
{
    [FSMState((int)Enemy.States.Projectile)]
    public class EnemyProjectileState : FSMState<Enemy>
    {
        private static readonly int ProjectileAnimHash = Animator.StringToHash("Projectile");
        private static readonly int ProjectileSpeedAnimHash = Animator.StringToHash("ProjectileSpeed");
        
        private const string HitEffectPath = "Effect/Prefabs/FX_Yagubat_Hit.prefab";
        private const float ReleaseDecayAddition = 2;
        private const float MinSpeed = 0.1f;
        
        private LayerMask _bounceMask;
        
        private float _maxSpeed;
        private float _speed;
        private float _speedDecay;
        private float _currentSpeedDecay;
        
        private float _size;

        private int _maxBounce;
        private int _bounceCount;
        
        private Vector2 _direction;
        
        private float _damage;
        
        private bool _isReleased;
        private float _releaseStartTime;
        private float _releaseDelay;
        
        
        private Transform _transform;
        
        public EnemyProjectileState(IFSMEntity owner) : base(owner)
        {
            _transform = _ownerEntity.transform;
            
            var data = SystemManager.Instance.DataManager.GetDataBase<ProjectileGameDataBase>()
                    .GetData(_ownerEntity.Data.ProjectileDataId);
            
            _size = data.ColliderRad * 0.5f;
            _damage = data.DirectDmg;
        }

        public void InitializeState(Vector2 dir, float power, LayerMask bounceMask)
        {
            _direction = dir;
            _maxSpeed = _speed = power;
            _bounceMask = bounceMask;
            
            _bounceCount = _maxBounce = 2;
            _releaseDelay = 1;
            _isReleased = false;

            _ownerEntity.SetPhysics(false);
            
            _ownerEntity.Animator.SetTrigger(ProjectileAnimHash);
        }
        
        public override void ClearState()
        {
            SystemManager.Instance.ProjectileManager.UnRegister(_ownerEntity);
        }
        
        public override void UpdateState()
        {
            if (_isReleased && Time.time - _releaseStartTime >= _releaseDelay)
            {
                _ownerEntity.ChangeState(Enemy.States.Dead);
            }

            CheckHit();
            Move();
        }
        
        private void CheckHit()
        {
            var hit = Physics2D.CircleCast(_transform.position, _size, _direction, _speed * Time.deltaTime, _bounceMask);

            if (hit.collider != null)
            {
                if (hit.collider.TryGetComponent(out IHitable hitable))
                {
                    hitable.Hit(_direction, _damage);
                    SystemManager.Instance.ResourceManager.EmitParticle(HitEffectPath, hit.point); 
                }
                
                _direction += hit.normal * (-2 * Vector2.Dot(_direction, hit.normal));
                if (--_bounceCount == 0)
                {
                    _isReleased = true;
                    _releaseStartTime = Time.time;

                    if (_releaseDelay > 0)
                    {
                        _currentSpeedDecay = (_speed / _releaseDelay) + ReleaseDecayAddition;
                    }
                }
            }
        }

        private void Move()
        {
            _speed -= _currentSpeedDecay * Time.deltaTime;
            
            if (_isReleased)
            {
                _speed = Mathf.Max(_speed, MinSpeed);
            }
            else
            {
                _speed -= _speedDecay * Time.deltaTime;
                
                if (_speed <= 0)
                {
                    _isReleased = true;
                    _releaseStartTime = Time.time;
                
                    _speed = MinSpeed;
                }
            }

            _transform.Translate(_direction * (_speed * Time.deltaTime));
            
            // easeInQuad
            var height = _speed / _maxSpeed;
            height *= height;

            //_ballObject.transform.localPosition = Vector3.up * (height * _ballHeight);
            
            _ownerEntity.Animator.SetFloat(ProjectileSpeedAnimHash, height);
        }
    }
}
