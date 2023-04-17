using System;
using System.Collections;
using System.Collections.Generic;
using QT.Core;
using QT.Core.Data;
using UnityEngine;

namespace QT
{
    public class Projectile : MonoBehaviour, IProjectile
    {
        private const float ReleaseDecayAddition = 2;
        private const float MinSpeed = 0.1f;
        
        public int ProjectileId => gameObject.GetInstanceID();
        public Vector2 Position => transform.position;

        [SerializeField] private float _ballHeight;
        [SerializeField] private Transform _ballObject;
        [SerializeField] private LayerMask _bounceMask;
        private TrailRenderer _trailRenderer;
        
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

        
        private void Awake()
        {
            _speedDecay = SystemManager.Instance.GetSystem<GlobalDataSystem>().GlobalData.SpdDecay;
            _trailRenderer = GetComponentInChildren<TrailRenderer>();
        }

        private void OnEnable()
        {
            SystemManager.Instance.ProjectileManager.Register(this);
        }

        private void OnDisable()
        {
            SystemManager.Instance?.ProjectileManager.UnRegister(this);
            _trailRenderer.Clear();
            
            // Todo : 더 확실한 플레이어 투사체 구분필요
            if (_isReleased && _releaseDelay > 0)
            {
                SystemManager.Instance?.PlayerManager.PlayerThrowProjectileReleased.Invoke();
            }
        }

        public void Init(ProjectileGameData data, Vector2 dir, float speed, int maxBounce, LayerMask bounceMask, float releaseDelay = 0)
        {
            _maxSpeed = _speed = speed;
            _currentSpeedDecay = _speedDecay;
            _size = data.ColliderRad * 0.5f;
            _damage = data.DirectDmg;

            _direction = dir;
            _bounceCount = _maxBounce = maxBounce;

            _bounceMask = bounceMask;
            _releaseDelay = releaseDelay;
            _isReleased = false;
        }
        
        public void Hit(Vector2 dir, float newSpeed)
        {
            Hit(dir, newSpeed, _bounceMask);
        }
        
        public void Hit(Vector2 dir, float newSpeed, LayerMask bounceMask)
        {
            _direction = dir;
            _maxSpeed = Mathf.Max(_speed, newSpeed);
            _speed = newSpeed;
            _currentSpeedDecay = _speedDecay;
            _bounceCount = _maxBounce;
            
            _bounceMask = bounceMask;
            _isReleased = false;
        }
        
        public void ResetBounceCount(int maxBounce)
        {
            _bounceCount = _maxBounce = maxBounce;
        }

        public LayerMask GetLayerMask()
        {
            return _bounceMask;
        }

        
        private void Update()
        {
            if (_isReleased && Time.time - _releaseStartTime >= _releaseDelay)
            {
                SystemManager.Instance.ResourceManager.ReleaseObject(this);
            }

            CheckHit();
            Move();
        }

        
        private void CheckHit()
        {
            var hit = Physics2D.CircleCast(transform.position, _size, _direction, _speed * Time.deltaTime, _bounceMask);

            if (hit.collider != null)
            {
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

            transform.Translate(_direction * (_speed * Time.deltaTime));
            
            // easeInQuad
            var height = _speed / _maxSpeed;
            height *= height;

            _ballObject.transform.localPosition = Vector3.up * (height * _ballHeight);
        }

        
#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            UnityEditor.Handles.color = Color.red;
            UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.back, _size);

            UnityEditor.Handles.Label(transform.position, (_size * 2).ToString());
            Gizmos.DrawRay(transform.position, Vector2.right * _size);
        }
#endif
        
    }

}