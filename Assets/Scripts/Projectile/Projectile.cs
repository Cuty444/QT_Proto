using System;
using System.Collections;
using System.Collections.Generic;
using QT.Core;
using QT.Core.Data;
using QT.Sound;
using UnityEngine;

namespace QT.InGame
{
    public class Projectile : MonoBehaviour, IProjectile
    {
        private const string HitEffectPath = "Effect/Prefabs/FX_M_Ball_Hit_Boom.prefab";
        private const string ColliderDustEffectPath = "Effect/Prefabs/FX_Collider_Dust.prefab";
        private const float ReleaseDecayAddition = 2;
        private const float MinSpeed = 0.1f;
        
        public int InstanceId => gameObject.GetInstanceID();
        public Vector2 Position => transform.position;
        public float ColliderRad { get; private set; }
        public LayerMask BounceMask => _bounceMask;

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
        [SerializeField] private GameObject _enemyElite;
        [SerializeField] private GameObject _boss;
        
        private TrailRenderer[] _trailRenderer;
        
        private float _maxSpeed;
        private float _speed;
        private float _speedDecay;
        private float _currentSpeedDecay;

        private int _maxBounce;
        private int _bounceCount;

        private Vector2 _direction;

        private float _damage;

        private bool _isReleased;
        private float _releaseDelay;
        private float _releaseTimer;

        private float _reflectCorrection;
        private Transform _playerTransform;


        private float _currentStretch;

        private SoundManager _soundManager;

        private bool _isPierce;

        private IHitAble _lastHitAble;
        
        private GlobalData _globalData;


        private void Awake()
        {
            _speedDecay = SystemManager.Instance.GetSystem<GlobalDataSystem>().GlobalData.SpdDecay;
            Player player = SystemManager.Instance.PlayerManager.Player;
            if (player == null)
            {
                Destroy(gameObject);
                return;
            }
            _playerTransform = player.transform;
            _trailRenderer = GetComponentsInChildren<TrailRenderer>();
            _soundManager = SystemManager.Instance.SoundManager;
            _boss.SetActive(false);
            
            _globalData = SystemManager.Instance.GetSystem<GlobalDataSystem>().GlobalData;
        }

        private void OnEnable()
        {
            ProjectileManager.Instance.Register(this);
        }

        private void OnDisable()
        {
            ProjectileManager.Instance.UnRegister(this);
            if (_trailRenderer != null)
            {
                for (int i = 0; i < _trailRenderer.Length; i++)
                {
                    _trailRenderer[i].Clear();
                }
            }
        }

        public void Init(ProjectileGameData data, Vector2 dir, float speed, int maxBounce, float reflectCorrection, LayerMask bounceMask, ProjectileOwner owner, bool isPierce = false, float releaseDelay = 0, string path = "")
        {
            _ballTransform.up = _direction = dir;
            _maxSpeed = _speed = speed;
            _currentSpeedDecay = _speedDecay;
            
            _damage = data.DirectDmg;
            ColliderRad = data.ColliderRad * 0.5f;
            
            _releaseTimer = 0;
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

            _isPierce = isPierce;

            _reflectCorrection = reflectCorrection * Mathf.Deg2Rad;
            _bounceMask = bounceMask;
            _isReleased = false;

            _prefabPath = path;

            _owner = owner;
            SetOwnerColor();
            TrailRendersSetEmitting(true);
        }

        public void Hit(Vector2 dir, float newSpeed, AttackType attackType)
        {
            ProjectileHit(dir, newSpeed, _bounceMask, _owner, _reflectCorrection);
        }
        
        
        public void ProjectileHit(Vector2 dir, float newSpeed, LayerMask bounceMask, ProjectileOwner owner, float reflectCorrection = 0,bool isPierce = false)
        {
            _ballTransform.up  = _direction = dir;
            _maxSpeed = Mathf.Max(_speed, newSpeed);
            _speed = newSpeed;
            _currentSpeedDecay = _speedDecay;
            
            _bounceCount = _maxBounce;

            switch (owner)
            {
                case ProjectileOwner.Player:
                case ProjectileOwner.PlayerTeleport:
                case ProjectileOwner.PlayerAbsorb:
                    _owner = ProjectileOwner.Player;
                    break;
                default:
                    _owner = owner;
                    break;
            }
            
            _bounceMask = bounceMask;
            _reflectCorrection = reflectCorrection * Mathf.Deg2Rad;
            _isReleased = false;
            _isPierce = isPierce;
            SetOwnerColor();
        }
        
        public void ResetBounceCount(int maxBounce)
        {
            _bounceCount = _maxBounce = maxBounce;
        }

        public void ResetProjectileDamage(int damage)
        {
            _damage = damage;
        }

        private void SetOwnerColor()
        {
            _player?.SetActive(_owner == ProjectileOwner.Player);
            _enemy?.SetActive(_owner == ProjectileOwner.Enemy);
            _enemyElite?.SetActive(_owner == ProjectileOwner.EnemyElite);
            _boss?.SetActive(_owner == ProjectileOwner.Boss);
        }

        
        private void Update()
        {
            _releaseTimer += Time.deltaTime;
            if (_isReleased && _releaseTimer > _releaseDelay)
            {
                SystemManager.Instance.ResourceManager.ReleaseObject(_prefabPath, this);
                TrailRendersSetEmitting(false);
            }

            CheckHit();
            Move();
        }


        private void CheckHit()
        {
            var hit = Physics2D.CircleCast(transform.position, ColliderRad, _direction, _speed * Time.deltaTime,
                _bounceMask);
            
            if (hit.collider == null)
                return;

            var pierceCheck = false;
            var isTriggerCheck = false;

            if (_speed >_globalData.BallMinSpdToHit)
            {
                if (hit.collider.TryGetComponent(out IHitAble hitAble))
                {
                    if (_lastHitAble == hitAble)
                        return;

                    hitAble.Hit(_direction, _damage);
                    if (!hitAble.IsClearTarget)
                    {
                        isTriggerCheck = hit.collider.isTrigger;
                    }
                    else if (_owner == ProjectileOwner.Player)
                    {
                        SystemManager.Instance.ResourceManager.EmitParticle(HitEffectPath, hit.point);
                        _soundManager.PlayOneShot(_soundManager.SoundData.PlayerThrowHitSFX);
                    }

                    _lastHitAble = hitAble;
                    pierceCheck = _isPierce;
                }
                else
                {
                    _lastHitAble = null;
                    _soundManager.PlayOneShot(_soundManager.SoundData.BallBounceSFX, hit.transform.position);
                }
            }

            if (isTriggerCheck || pierceCheck)
                return;

            _ballTransform.up = hit.normal;
            _currentStretch = -1;
            _ballTransform.localScale = GetSquashSquashValue(_currentStretch);

            _direction = Vector2.Reflect(_direction, hit.normal);
            transform.Translate(hit.normal * ColliderRad);

            SystemManager.Instance.ResourceManager.EmitParticle(ColliderDustEffectPath, hit.point);

            if (_reflectCorrection != 0)
            {
                var targetDir = ((Vector2) _playerTransform.transform.position - hit.point).normalized;
                _direction = Vector3.RotateTowards(_direction, targetDir, _reflectCorrection, 0);
            }

            if (!_isReleased && --_bounceCount <= 0)
            {
                _isReleased = true;
                _releaseTimer = 0;

                if (_releaseDelay > 0)
                {
                    _currentSpeedDecay = (_speed / _releaseDelay) + ReleaseDecayAddition;
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
                    _releaseTimer = 0;
                
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

        private void TrailRendersSetEmitting(bool isActive)
        {
            if (_trailRenderer == null)
            {
                return;
            }
            
            for (int i = 0; i < _trailRenderer.Length; i++)
            {
                _trailRenderer[i].emitting = isActive;
            }
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