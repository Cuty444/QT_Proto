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
        private float _releaseDelay;
        private float _releaseTimer;
        
        private Transform _transform;

        private SoundManager _soundManager;

        private float _lastStuckTime = 0f;

        private bool _isPierce;
        
        private IHitAble _lastHitAble;
        

        public EnemyProjectileState(IFSMEntity owner) : base(owner)
        {
            _transform = _ownerEntity.transform;
            
            var data = SystemManager.Instance.DataManager.GetDataBase<ProjectileGameDataBase>()
                    .GetData(_ownerEntity.Data.ProjectileDataId);
            
            _size = data.ColliderRad * 0.5f;
            _damage = data.DirectDmg;
            
            _speedDecay = SystemManager.Instance.GetSystem<GlobalDataSystem>().GlobalData.SpdDecay;

            _soundManager = SystemManager.Instance.SoundManager;
        }

        public void InitializeState(Vector2 dir, float power, LayerMask bounceMask, bool isPierce, bool playSound)
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
            _isPierce = isPierce;

            _ownerEntity.SetPhysics(false);
            
            if (_ownerEntity.HP <= 0)
            { 
                if(playSound)
                    _soundManager.PlayOneShot(_soundManager.SoundData.MonsterStun);
            }

            _ownerEntity.Animator.SetBool(ProjectileAnimHash, true);
            
            if(playSound)
                _soundManager.PlayOneShot(_soundManager.SoundData.Monster_AwaySFX);

            _damage = _ownerEntity._damage;
        }
        
        public override void ClearState()
        {
            _ownerEntity.BounceMask = _ownerEntity.Shooter.BounceMask;
            _ownerEntity.Animator.SetBool(ProjectileAnimHash, false);
        }
        
        public override void UpdateState()
        {
            _releaseTimer += Time.deltaTime;
            if (_isReleased && _releaseTimer > _releaseDelay)
            {
                _ownerEntity.ChangeState(Enemy.States.Dead);
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

            if (_speed > 0.5f)
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
                    pierceCheck = _isPierce;
                    //SystemManager.Instance.ResourceManager.EmitParticle(HitEffectPath, hit.point); 
                }
                else
                {
                    if (Time.timeSinceLevelLoad - _lastStuckTime > 0.1f)
                    {
                        _lastStuckTime = Time.timeSinceLevelLoad;
                        _soundManager.PlayOneShot(_soundManager.SoundData.Monster_AwayWallHitSFX);
                    }

                    _lastHitAble = null;
                }
            }

            if (isTriggerCheck || pierceCheck)
                return;

            _direction += hit.normal * (-2 * Vector2.Dot(_direction, hit.normal));
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

            _transform.Translate(_direction * (_speed * Time.deltaTime));
            
            // easeInQuad
            var height = _speed / _maxSpeed;
            height *= height;

            _ownerEntity.BallObject.localPosition = Vector3.up * (_ownerEntity.BallHeightMin + height * _ownerEntity.BallHeight);
            _ownerEntity.Animator.SetFloat(ProjectileSpeedAnimHash, height);
        }
    }
}
