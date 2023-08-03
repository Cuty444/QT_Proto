using System.Collections;
using System.Collections.Generic;
using QT.Core;
using QT.Sound;
using QT.Util;
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
        private float _releaseDelay;
        private float _releaseTimer;
        
        
        private Transform _transform;
        
        private bool _isNormal;

        private SoundManager _soundManager;

        private bool isStuckSound = false;
        
        private List<IHitAble> _hitAbles = new List<IHitAble>();
        

        public EnemyProjectileState(IFSMEntity owner) : base(owner)
        {
            _transform = _ownerEntity.transform;
            
            var data = SystemManager.Instance.DataManager.GetDataBase<ProjectileGameDataBase>()
                    .GetData(_ownerEntity.Data.ProjectileDataId);
            
            _size = data.ColliderRad * 0.5f;
            _damage = data.DirectDmg; // TODO : 의미 없음

            _soundManager = SystemManager.Instance.SoundManager;
        }

        public void InitializeState(Vector2 dir, float power, LayerMask bounceMask)
        {
            _direction = dir;
            _maxSpeed = _speed = power;
            _bounceMask = bounceMask;
            _ownerEntity.BounceMask = bounceMask;
            
            _bounceCount = _maxBounce = 2;
            _releaseDelay = 1;
            _isReleased = false;

            _ownerEntity.SetPhysics(false);
            
            if (_ownerEntity.HP <= 0)
            { 
                _ownerEntity.HpCanvas.gameObject.SetActive(false);
                _soundManager.PlayOneShot(_soundManager.SoundData.MonsterStun);
            }
            _ownerEntity.Animator.SetTrigger(ProjectileAnimHash);
            
            _soundManager.PlayOneShot(_soundManager.SoundData.Monster_AwaySFX);
            _isNormal = false;

            _damage = _ownerEntity._damage;
        }
        
        public override void ClearState()
        {
            _ownerEntity.Animator.ResetTrigger(ProjectileAnimHash);
            if (_isNormal)
            {
                _ownerEntity.Animator.SetTrigger(NormalAnimHash);
                _ownerEntity.StartCoroutine(
                    Util.UnityUtil.WaitForFunc(() => { _ownerEntity.Animator.ResetTrigger(NormalAnimHash); }, 0.2f));
            }
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
            var hit = Physics2D.CircleCast(_transform.position, _size, _direction, _speed * Time.deltaTime, _bounceMask);

            if (hit.collider != null)
            {
                bool isTriggerCheck = false;
                if (hit.collider.TryGetComponent(out IHitAble hitable))
                {
                    if (_hitAbles.Contains(hitable))
                        return;
                    else
                    {
                        _hitAbles.Clear();
                    }
                    if (hit.collider.TryGetComponent(out InteractionObject interactionObject))
                    {
                        isTriggerCheck = hit.collider.isTrigger;
                    }
                    else
                    {
                        hitable.Hit(_direction, _damage);
                        _soundManager.PlayOneShot(_soundManager.SoundData.Monster_AwayMonsterHitSFX);
                    }
                    _hitAbles.Add(hitable);
                    //SystemManager.Instance.ResourceManager.EmitParticle(HitEffectPath, hit.point); 
                }
                else
                {
                    if (!isStuckSound)
                    {
                        _soundManager.PlayOneShot(_soundManager.SoundData.Monster_AwayWallHitSFX);
                        isStuckSound = true;
                        _ownerEntity.StartCoroutine(UnityUtil.WaitForFunc(() =>
                        {
                            isStuckSound = false;
                        }, 0.1f));
                    }
                    _hitAbles.Clear();
                }
                
                if (isTriggerCheck)
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
                        _isNormal = true;
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
