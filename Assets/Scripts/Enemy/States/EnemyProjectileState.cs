using System.Collections;
using System.Collections.Generic;
using QT.Core;
using QT.Core.Data;
using QT.Sound;
using UnityEngine;

namespace QT.InGame
{
    [FSMState((int)Enemy.States.Projectile)]
    public class EnemyProjectileState : FSMState<Enemy>
    {
        private static readonly int ProjectileAnimHash = Animator.StringToHash("Projectile");
        private static readonly int ProjectileSpeedAnimHash = Animator.StringToHash("ProjectileSpeed");
        private static readonly int NormalAnimHash = Animator.StringToHash("Normal");
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
        private float _releaseStartTime;
        private float _releaseDelay;
        
        
        private Transform _transform;
        
        private bool isNormal;

        private SoundManager _soundManager;
        public EnemyProjectileState(IFSMEntity owner) : base(owner)
        {
            _transform = _ownerEntity.transform;
            
            var data = SystemManager.Instance.DataManager.GetDataBase<ProjectileGameDataBase>()
                    .GetData(_ownerEntity.Data.ProjectileDataId);
            
            _size = data.ColliderRad * 0.5f;
            _damage = data.DirectDmg;

            _soundManager = SystemManager.Instance.SoundManager;
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
            
            if (_ownerEntity.HP <= 0)
            { 
                _ownerEntity.HpCanvas.gameObject.SetActive(false);
            }
            _ownerEntity.Animator.SetTrigger(ProjectileAnimHash);
            
            _soundManager.PlayOneShot(_soundManager.SoundData.MonsterFly);
            isNormal = false;
        }
        
        public override void ClearState()
        {
            _ownerEntity.Animator.ResetTrigger(ProjectileAnimHash);
            if (isNormal)
            {
                _ownerEntity.Animator.SetTrigger(NormalAnimHash);
                _ownerEntity.StartCoroutine(
                    Util.UnityUtil.WaitForFunc(() => { _ownerEntity.Animator.ResetTrigger(NormalAnimHash); }, 0.2f));
            }
            SystemManager.Instance.ProjectileManager.UnRegister(_ownerEntity);
            _ownerEntity.IsTeleportProjectile = false;
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
                    if (_ownerEntity.IsTeleportProjectile)
                    {
                        hitable.Hit(_direction,_damage,AttackType.Teleport);
                    }
                    else
                    {
                        hitable.Hit(_direction, _damage);
                    }
                    //SystemManager.Instance.ResourceManager.EmitParticle(HitEffectPath, hit.point); 
                }
                
                _direction += hit.normal * (-2 * Vector2.Dot(_direction, hit.normal));
                if (--_bounceCount == 0)
                {
                    if (_ownerEntity.HP <= 0)
                    {
                        _isReleased = true;
                    }
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
                    if (_ownerEntity.IsFall)
                    {
                        _ownerEntity.HP.SetStatus(0);
                        _isReleased = true;
                        _releaseStartTime = Time.time;
                        _ownerEntity.ChangeState(Enemy.States.Dead);
                        return;
                    }
                    if (_ownerEntity.HP <= 0)
                    {
                        _isReleased = true;
                        _releaseStartTime = Time.time;

                        _speed = MinSpeed;
                    }
                    else
                    {
                        isNormal = true;
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
