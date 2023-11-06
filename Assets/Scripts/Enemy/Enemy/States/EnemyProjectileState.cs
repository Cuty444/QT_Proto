using System.Collections;
using System.Collections.Generic;
using QT.Core;
using QT.Core.Data;
using QT.Sound;
using QT.Util;
using UnityEngine;

namespace QT.InGame
{
    [FSMState((int)Enemy.States.Projectile)]
    public class EnemyProjectileState : FSMState<Enemy>
    {
        private static readonly int ProjectileAnimHash = Animator.StringToHash("IsProjectile");
        private static readonly int ProjectileSpeedAnimHash = Animator.StringToHash("ProjectileSpeed");
        private static readonly int RigidAnimHash = Animator.StringToHash("IsRigid");
        
        private const string HitEffectPath = "Effect/Prefabs/FX_M_Wall_Hit.prefab";
        private const string HitEnemyEffectPath = "Effect/Prefabs/FX_M_Ball_Hit_Boom.prefab";
        private const string FlyingStartEffectPath = "Effect/Prefabs/FX_M_Flying_AirResistance.prefab";
        private const string FlyingEffectPath = "Effect/Prefabs/FX_M_Flying_Dust.prefab";

        
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
        private float _releaseDelay;
        private float _releaseTimer;
        
        private Transform _transform;
        private Transform _targetTransform;

        private SoundManager _soundManager;

        private float _lastStuckTime = 0f;

        private ProjectileProperties _properties;
        
        private IHitAble _lastHitAble;
        
        private GlobalData _globalData;

        private ParticleSystem _flyingEffect;
        
        public EnemyProjectileState(IFSMEntity owner) : base(owner)
        {
            _transform = _ownerEntity.transform;
            
            _globalData = SystemManager.Instance.GetSystem<GlobalDataSystem>().GlobalData;
            _speedDecay = _globalData.SpdDecay;

            _soundManager = SystemManager.Instance.SoundManager;
        }

        public async void InitializeState(Vector2 dir, float power, LayerMask bounceMask, ProjectileProperties properties, Transform target)
        {
            _direction = dir;
            _maxSpeed = _speed = power;
            _bounceMask = bounceMask;
            _ownerEntity.BounceMask = bounceMask;

            _currentSpeedDecay = _speedDecay;
            _bounceCount = _maxBounce = 2;
            _releaseDelay = 1;
            _releaseTimer = 0;
            _isReleased = false;
            _properties = properties;

            _targetTransform = target;

            _ownerEntity.SetPhysics(false);
            
            if (_ownerEntity.HP <= 0)
            {
                _soundManager.PlayOneShot(_soundManager.SoundData.MonsterStun);
            }
            _soundManager.PlayOneShot(_soundManager.SoundData.Monster_AwaySFX);

            _ownerEntity.Animator.SetBool(ProjectileAnimHash, true);
            
            _size = _ownerEntity.ColliderRad;
            _damage = _ownerEntity.ProjectileDamage;
            
            _flyingEffect = await SystemManager.Instance.ResourceManager.GetFromPool<ParticleSystem>(FlyingEffectPath, _ownerEntity.BallObject);
            _flyingEffect.transform.ResetLocalTransform();
            
            _flyingEffect.Play();

            var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            SystemManager.Instance.ResourceManager.EmitParticle(FlyingStartEffectPath, Vector2.zero,  Quaternion.Euler(0, 0, angle), _ownerEntity.BallObject);
        }
        
        public override void ClearState()
        {
            _ownerEntity.BounceMask = _ownerEntity.Shooter.BounceMask;
            _ownerEntity.Animator.SetBool(ProjectileAnimHash, false);
            
            _flyingEffect.Stop();
            SystemManager.Instance.ResourceManager.ReleaseObjectWithDelay(FlyingEffectPath, _flyingEffect, 1.0f);
        }
        
        public override void UpdateState()
        {
            _releaseTimer += Time.deltaTime;
            if (_isReleased && _releaseTimer > _releaseDelay)
            {
                _ownerEntity.ChangeState(Enemy.States.Dead);
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
            var hit = Physics2D.CircleCast(_transform.position, _size, _direction, _speed * Time.deltaTime,
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
                    else
                    {
                        _soundManager.PlayOneShot(_soundManager.SoundData.Monster_AwayMonsterHitSFX);
                    }

                    _lastHitAble = hitAble;
                    pierceCheck = _properties.HasFlag(ProjectileProperties.Pierce);
                    SystemManager.Instance.ResourceManager.EmitParticle(HitEnemyEffectPath, hit.point); 
                }
                else
                {
                    if (Time.timeSinceLevelLoad - _lastStuckTime > 0.1f)
                    {
                        _lastStuckTime = Time.timeSinceLevelLoad;
                        _soundManager.PlayOneShot(_soundManager.SoundData.Monster_AwayWallHitSFX);
                        SystemManager.Instance.ResourceManager.EmitParticle(HitEffectPath, hit.point); 
                    }

                    _lastHitAble = null;
                }
            }

            if (isTriggerCheck || pierceCheck)
                return;

            
            if (_speed > _globalData.BallMinSpdToHit && _ownerEntity.HP > 0)
            {
                _ownerEntity.OnDamageEvent.Invoke(-_direction, _speed * _globalData.BallBounceDamage, AttackType.Ball);
            }

            _direction = Vector2.Reflect(_direction, hit.normal);
            _transform.Translate(hit.normal * _size);
            
            _properties &= ~ProjectileProperties.Guided;
            
            if (_properties.HasFlag(ProjectileProperties.Explosion))
            {
                Explosion.MakeExplosion(_transform.position, SystemManager.Instance.PlayerManager.Player);
                _properties &= ~ProjectileProperties.Explosion;
            }
            
            if (--_bounceCount == 0)
            {
                if (_ownerEntity.HP <= 0)
                {
                    _isReleased = true;
                }

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
                    if (_ownerEntity.Steering.IsStuck())
                    {
                        _ownerEntity.HP.SetStatus(0);
                        _isReleased = true;
                        _releaseTimer = 0;
                        _ownerEntity.ChangeState(Enemy.States.Dead);
                        return;
                    }
                    if (_ownerEntity.HP <= 0)
                    {
                        _isReleased = true;
                        _releaseTimer = 0;

                        _speed = MinSpeed;
                    }
                    else
                    {
                        _ownerEntity.Animator.SetBool(RigidAnimHash, false);
                        _ownerEntity.Animator.SetFloat(ProjectileSpeedAnimHash, 1);
                        _ownerEntity.SetPhysics(true);
                        _ownerEntity.ChangeState(Enemy.States.Normal);
                        return;
                    }
                }
            }
            
            if (_properties.HasFlag(ProjectileProperties.Guided))
            {
                Vector2 targetDir = (_targetTransform.transform.position - _transform.position).normalized;
                _direction = Vector3.RotateTowards(_direction, targetDir, _globalData.BallGuidedAngle * Mathf.Deg2Rad * Time.deltaTime, 0);
            }
            
            _transform.Translate(_direction * (_speed * Time.deltaTime));
            
            // easeInQuad
            var height = _speed / _maxSpeed;
            height *= height;

            _ownerEntity.BallObject.localPosition = Vector3.up * (_ownerEntity.BallHeightMin + height * _ownerEntity.BallHeight);
            _ownerEntity.Animator.SetFloat(ProjectileSpeedAnimHash, height);
        }
    }
}
