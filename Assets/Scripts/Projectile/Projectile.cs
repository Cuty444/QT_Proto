using System;
using System.Collections;
using System.Collections.Generic;
using QT.Core;
using QT.Core.Data;
using UnityEngine;

namespace QT.InGame
{
    public class Projectile : MonoBehaviour, IProjectile
    {
        private const string HitEffectPath = "Effect/Prefabs/FX_M_Hit.prefab";
        private const string ColliderDustEffectPath = "Effect/Prefabs/FX_Collider_Dust.prefab";
        private const string BallBounceSoundPath = "Assets/Sound/QT/Assets/Ball_Bounce_";
        private const string BallThrowHitSoundPath = "Assets/Sound/QT/Assets/Player_Throw_Hit_";
        private const float ReleaseDecayAddition = 2;
        private const float MinSpeed = 0.1f;
        
        public int ProjectileId => gameObject.GetInstanceID();
        public Vector2 Position => transform.position;
        public float ColliderRad { get; private set; }

        private string _prefabPath;
        
        [SerializeField] private ProjectileOwner _owner;
        [SerializeField] private LayerMask _bounceMask;
        
        [Space]
        [SerializeField] private float _ballHeight;
        [SerializeField] private Transform _ballTransform;
        [SerializeField] private Transform _ballObject;
        
        [Space]
        [SerializeField] private float _maxSquashLength;
        [SerializeField] private float _maxStretchLength;
        [SerializeField] private float _dampSpeed;
        [SerializeField] private float _rotateSpeed;
        
        [Space]
        [SerializeField] private GameObject _player;
        [SerializeField] private GameObject _enemy;
        
        private TrailRenderer _trailRenderer;
        
        private float _maxSpeed;
        private float _speed;
        private float _speedDecay;
        private float _currentSpeedDecay;

        private int _maxBounce;
        private int _bounceCount;

        private Vector2 _direction;

        private float _damage;

        private bool _isReleased;
        private float _releaseStartTime;
        private float _releaseDelay;

        private float _reflectCorrection;
        private Transform _playerTransform;

        private LayerMask _enemyLayerMask;

        private float _currentStretch;

        private void Awake()
        {
            _speedDecay = SystemManager.Instance.GetSystem<GlobalDataSystem>().GlobalData.SpdDecay;
            _playerTransform = SystemManager.Instance.PlayerManager.Player.transform;
            _trailRenderer = GetComponentInChildren<TrailRenderer>();
            _enemyLayerMask = LayerMask.GetMask("Enemy");
        }

        private void OnEnable()
        {
            SystemManager.Instance.ProjectileManager.Register(this);
        }

        private void OnDisable()
        {
            SystemManager.Instance?.ProjectileManager.UnRegister(this);
            _trailRenderer?.Clear();
            
            // Todo : 더 확실한 플레이어 투사체 구분필요
            if (_isReleased && _releaseDelay > 0)
            {
                SystemManager.Instance?.PlayerManager.PlayerThrowProjectileReleased.Invoke();
            }
        }

        public void Init(ProjectileGameData data, Vector2 dir, float speed, int maxBounce, float reflectCorrection, LayerMask bounceMask, ProjectileOwner owner, float releaseDelay = 0, string path = "")
        {
            _ballTransform.up = _direction = dir;
            _maxSpeed = _speed = speed;
            _currentSpeedDecay = _speedDecay;
            
            _damage = data.DirectDmg;
            ColliderRad = data.ColliderRad * 0.5f;

            if (data.IsBounce)
            {
                _bounceCount = _maxBounce = maxBounce;
                _releaseDelay = releaseDelay;
            }
            else
            {
                _bounceCount = _maxBounce = 0;
                _releaseDelay = 0;
            }

            _reflectCorrection = reflectCorrection * Mathf.Deg2Rad;
            _bounceMask = bounceMask;
            _isReleased = false;

            _prefabPath = path;

            _owner = owner;
            SetOwnerColor();
        }
        
        public void Hit(Vector2 dir, float newSpeed)
        {
            ProjectileHit(dir, newSpeed, _bounceMask, _owner, _reflectCorrection);
        }
        
        public void ProjectileHit(Vector2 dir, float newSpeed, LayerMask bounceMask, ProjectileOwner owner, float reflectCorrection = 0)
        {
            _ballTransform.up  = _direction = dir;
            _maxSpeed = Mathf.Max(_speed, newSpeed);
            _speed = newSpeed;
            _currentSpeedDecay = _speedDecay;
            
            _bounceCount = _maxBounce;
            _owner = owner;
            _bounceMask = bounceMask;
            _reflectCorrection = reflectCorrection * Mathf.Deg2Rad;
            _isReleased = false;
            
            SetOwnerColor();
        }
        
        public void ResetBounceCount(int maxBounce)
        {
            _bounceCount = _maxBounce = maxBounce;
        }

        public LayerMask GetLayerMask()
        {
            return _bounceMask;
        }
        
        private void SetOwnerColor()
        {
            _player?.SetActive(_owner == ProjectileOwner.Player);
            _enemy?.SetActive(_owner == ProjectileOwner.Enemy);
        }

        
        private void Update()
        {
            if (_isReleased && Time.time - _releaseStartTime >= _releaseDelay)
            {
                SystemManager.Instance.ResourceManager.ReleaseObject(_prefabPath, this);
            }

            CheckHit();
            Move();
        }

        
        private void CheckHit()
        {
            var hit = Physics2D.CircleCast(transform.position, ColliderRad, _direction, _speed * Time.deltaTime, _bounceMask);

            if (hit.collider != null)
            {
                if (hit.collider.TryGetComponent(out IHitable hitable))
                {
                    hitable.Hit(_direction, _damage);
                    if((_bounceMask &_enemyLayerMask) != 0)
                    {
                        SystemManager.Instance.ResourceManager.EmitParticle(HitEffectPath, hit.point);
                        SystemManager.Instance.SoundManager.RandomSoundOneShot(BallThrowHitSoundPath,3);
                    }
                }
                else
                {
                    SystemManager.Instance.SoundManager.RandomSoundOneShot(BallBounceSoundPath,5);
                }
                
                _ballTransform.up = hit.normal;
                _currentStretch = -1;
                _ballTransform.localScale = GetSquashSquashValue(_currentStretch);
                
                _direction = Vector2.Reflect(_direction, hit.normal);
                
                
                SystemManager.Instance.ResourceManager.EmitParticle(ColliderDustEffectPath, hit.point);
                
                if (_reflectCorrection != 0)
                {
                    var targetDir = ((Vector2) _playerTransform.transform.position - hit.point).normalized;
                    _direction = Vector3.RotateTowards(_direction, targetDir, _reflectCorrection, 0);
                }
                
                if (!_isReleased && --_bounceCount <= 0)
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

            _ballTransform.transform.localPosition = Vector3.up * (height * _ballHeight);
            _ballObject.Rotate(Vector3.forward, _speed * Time.deltaTime * _rotateSpeed);

            _currentStretch = Mathf.Lerp(_currentStretch, height, _dampSpeed * Time.deltaTime);
            _ballTransform.localScale = GetSquashSquashValue(_currentStretch);
            
            _ballTransform.up = Vector2.Lerp(_ballTransform.up, _direction, _dampSpeed * Time.deltaTime);
        }

        private Vector2 GetSquashSquashValue(float power)
        {
            if (power > 0)
            {
                power = Mathf.Lerp(1, _maxStretchLength, power);
            }
            else if (power < 0)
            {
                power = Mathf.Lerp(_maxSquashLength, 1, 1 + power);
            }

            return new Vector2(2 - power, power);
        }
        
        
#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            UnityEditor.Handles.color = Color.red;
            UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.back, ColliderRad);

            UnityEditor.Handles.Label(transform.position, (ColliderRad * 2).ToString());
            Gizmos.DrawRay(transform.position, Vector2.right * ColliderRad);
        }
#endif
        
    }

}