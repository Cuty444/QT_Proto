using System;
using System.Collections;
using System.Collections.Generic;
using QT.Core;
using QT.Core.Data;
using QT.Sound;
using UnityEngine;
using UnityEngine.Serialization;

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
        
        public Vector2 Direction { get; private set; }
        public float Speed { get; private set; }
        
        public IHitAble LastHitAble { get; private set; }
        
        
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
        public float _speedDecay;
        private float _currentSpeedDecay;

        private int _maxBounce;
        private int _bounceCount;
        
        private float _damage;

        private bool _isReleased;
        private float _releaseDelay;
        private float _releaseTimer;
        
        private float _currentStretch;
        
        private ProjectileProperties _properties;
        
        private Transform _targetTransform;
        private SoundManager _soundManager;
        private GlobalData _globalData;
        
        private string _prefabPath;


        private void Awake()
        {
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

        public void Init(ProjectileGameData data, Vector2 dir, float speed, int maxBounce, LayerMask bounceMask, ProjectileOwner owner, ProjectileProperties properties, Transform target = null, float releaseDelay = 0, string path = "")
        {
            _ballTransform.up = Direction = dir;
            _maxSpeed = Speed = speed;
            _currentSpeedDecay = _speedDecay = _globalData.SpdDecay;
            
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

            _bounceMask = bounceMask;
            _isReleased = false;

            _prefabPath = path;

            _owner = owner;
            SetOwnerColor();
            TrailRendersSetEmitting(true);
            
            _properties = properties;
            _targetTransform = target;
        }

        public void ProjectileHit(Vector2 dir, float newSpeed, LayerMask bounceMask, ProjectileOwner owner, ProjectileProperties properties, Transform target = null)
        {
            _ballTransform.up  = Direction = dir;
            _maxSpeed = Mathf.Max(Speed, newSpeed);
            Speed = newSpeed;
            _currentSpeedDecay = _speedDecay = _globalData.SpdDecay;
            
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
            _isReleased = false;
            _properties = properties;
            SetOwnerColor();

            _targetTransform = target;
        }
        
        public void ResetBounceCount(int maxBounce)
        {
            _bounceCount = _maxBounce = maxBounce;
        }

        public void ResetProjectileDamage(int damage)
        {
            _damage = damage;
        }

        public void ResetSpeedDecay(float decay)
        {
            _speedDecay = decay;
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
            
            if (_properties.HasFlag(ProjectileProperties.Guided) && (_targetTransform == null || !_targetTransform.gameObject.activeInHierarchy))
            {
                _properties &= ~ProjectileProperties.Guided;
            }
            
            CheckHit();
            Move();
        }


        private void CheckHit()
        {
            var hit = Physics2D.CircleCast(transform.position, ColliderRad, Direction, Speed * Time.deltaTime,
                _bounceMask);
            
            if (hit.collider == null)
                return;

            var pierceCheck = false;
            var isTriggerCheck = false;

            if (Speed >_globalData.BallMinSpdToHit)
            {
                if (hit.collider.TryGetComponent(out IHitAble hitAble))
                {
                    if (LastHitAble == hitAble)
                        return;

                    hitAble.Hit(Direction, _damage);
                    if (!hitAble.IsClearTarget)
                    {
                        isTriggerCheck = hit.collider.isTrigger;
                    }
                    else if (_owner == ProjectileOwner.Player)
                    {
                        SystemManager.Instance.ResourceManager.EmitParticle(HitEffectPath, hit.point);
                        _soundManager.PlayOneShot(_soundManager.SoundData.PlayerThrowHitSFX);
                    }

                    LastHitAble = hitAble;
                    pierceCheck = _properties.HasFlag(ProjectileProperties.Pierce);
                }
                else
                {
                    LastHitAble = null;
                    _soundManager.PlayOneShot(_soundManager.SoundData.BallBounceSFX, hit.transform.position);
                }
            }

            if (isTriggerCheck || pierceCheck)
                return;

            _ballTransform.up = hit.normal;
            _currentStretch = -1;
            _ballTransform.localScale = GetSquashSquashValue(_currentStretch);

            Direction = Vector2.Reflect(Direction, hit.normal);
            transform.Translate(hit.normal * ColliderRad);

            SystemManager.Instance.ResourceManager.EmitParticle(ColliderDustEffectPath, hit.point);
            
            _properties &= ~ProjectileProperties.Guided;

            if (_properties.HasFlag(ProjectileProperties.Explosion))
            {
                if (_owner == ProjectileOwner.Player)
                {
                    Explosion.MakeExplosion(Position, SystemManager.Instance.PlayerManager.Player);
                }
                else
                {
                    Explosion.MakeExplosion(Position);
                }
            _properties &= ~ProjectileProperties.Explosion;
            }
            
            if (!_isReleased && --_bounceCount <= 0)
            {
                _isReleased = true;
                _releaseTimer = 0;

                if (_releaseDelay > 0)
                {
                    _currentSpeedDecay = (Speed / _releaseDelay) + ReleaseDecayAddition;
                }
            }
        }

        private void Move()
        {
            Speed -= _currentSpeedDecay * Time.deltaTime;
            
            if (_isReleased)
            {
                Speed = Mathf.Max(Speed, MinSpeed);
            }
            else
            {
                Speed -= _speedDecay * Time.deltaTime;
                
                if (Speed <= 0)
                {
                    _isReleased = true;
                    _releaseTimer = 0;
                
                    Speed = MinSpeed;
                }
            }

            
            if (_properties.HasFlag(ProjectileProperties.Guided))
            {
                Vector2 targetDir = ((Vector2) _targetTransform.transform.position - Position).normalized;
                Direction = Vector3.RotateTowards(Direction, targetDir, _globalData.BallGuidedAngle * Mathf.Deg2Rad * Time.deltaTime, 0);
            }
            
            transform.Translate(Direction * (Speed * Time.deltaTime));
            
            // easeInQuad
            var height = Speed / _maxSpeed;
            height *= height;

            _ballTransform.transform.localPosition = Vector3.up * (height * _ballHeight);
            _ballObject.Rotate(Vector3.forward, Speed * Time.deltaTime * _rotateSpeed);

            _currentStretch = Mathf.Lerp(_currentStretch, height, _dampSpeed * Time.deltaTime);
            _ballTransform.localScale = GetSquashSquashValue(_currentStretch);
            
            _ballTransform.up = Vector2.Lerp(_ballTransform.up, Direction, _dampSpeed * Time.deltaTime);
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