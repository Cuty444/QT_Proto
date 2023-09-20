using System.Collections;
using System.Timers;
using QT.Core;
using QT.Sound;
using QT.Util;
using UnityEngine;

namespace QT.InGame
{
    [FSMState((int) Dullahan.States.Rush)]
    public class DullahanRushState : FSMState<Dullahan>
    {
        private readonly int IsRushingAnimHash = Animator.StringToHash("IsRushing");
        private readonly int RushReadyAnimHash = Animator.StringToHash("RushReady");
        
        private const string RushEffectPath = "Effect/Prefabs/FX_Boss_Rush_Air_Resistance.prefab";
        private const string SparkEffectPath = "Effect/Prefabs/FX_Boss_Rush_Spark.prefab";
        private const string ShockEffectPath = "Effect/Prefabs/FX_Boss_Rush_Shock.prefab";
        
        private bool _isReady;
        private Vector2 _dir;

        private float _time;
        
        private Transform _transform;
        private Transform _rushCenter;
        
        private float _speed;
        private float _size;
        private float _damage;

        private SoundManager _soundManager;
        
        private ParticleSystem _rushEffect;
        private ParticleSystem _sparkEffect;
        
        public DullahanRushState(IFSMEntity owner) : base(owner)
        {
            _transform = _ownerEntity.transform;
            _rushCenter = _ownerEntity.CenterTransform;
            
            _speed = _ownerEntity.DullahanData.RushSpeed;
            _size = _ownerEntity.RushColliderSize;
            _damage = _ownerEntity.DullahanData.RushHitDamage;
            
            SystemManager.Instance.ResourceManager.CacheAsset(RushEffectPath);
            SystemManager.Instance.ResourceManager.CacheAsset(SparkEffectPath);
        }

        public override void InitializeState()
        {
            _soundManager = SystemManager.Instance.SoundManager;
            
            _dir = (SystemManager.Instance.PlayerManager.Player.transform.position - _ownerEntity.transform.position);
            _ownerEntity.SetDir(_dir,2);

            _dir.Normalize();

            _isReady = false;
            _time = 0;

            _ownerEntity.Animator.SetTrigger(RushReadyAnimHash);
            _soundManager.PlayOneShot(_soundManager.SoundData.Boss_RushReady, _ownerEntity.transform.position);
            
            _ownerEntity.RushTrailObject.SetActive(true);
            _ownerEntity.Rigidbody.velocity = Vector2.zero;

            SetEffect();
        }
        
        public override void ClearState()
        {
            _ownerEntity.Animator.ResetTrigger(RushReadyAnimHash);
            _ownerEntity.Animator.SetBool(IsRushingAnimHash, false);
            _ownerEntity.RushTrailObject.SetActive(false);
            _ownerEntity.SetPhysics(true);
        }
        
        public override void UpdateState()
        {
            _time += Time.deltaTime;
            
            if (!_isReady)
            {
                if (_time > _ownerEntity.DullahanData.RushReadyTime)
                {
                    _isReady = true;
                    _ownerEntity.RushTrailObject.SetActive(true);
                    _ownerEntity.Animator.SetBool(IsRushingAnimHash, true);
                    _soundManager.PlayOneShot(_soundManager.SoundData.Boss_Rush, _ownerEntity.transform.position);
                    _ownerEntity.SetPhysics(false);
                    _time = 0;
                }
            }
            else
            {
                _transform.Translate(_dir * (_speed * Time.deltaTime));
            }
        }

        public override void FixedUpdateState()
        {
            if (!_isReady)
            {
                return;
            }
            
            if (CheckHit() || _time > _ownerEntity.DullahanData.RushLengthTime)
            {
                _ownerEntity.ChangeState(Dullahan.States.Normal);
            }
        }

        private bool CheckHit()
        {
            var hits = Physics2D.CircleCastAll(_rushCenter.position, _size, _dir, _speed * Time.deltaTime,
                _ownerEntity.HitMask);

            Debug.DrawRay(_rushCenter.position, _dir * (_size + _speed * Time.deltaTime), Color.magenta, 1);
            Debug.DrawRay(_rushCenter.position, new Vector3(-_dir.y, _dir.x) * (_size), Color.magenta, 1);
            Debug.DrawRay(_rushCenter.position, new Vector3(_dir.y, -_dir.x) * (_size), Color.magenta, 1);

            foreach (var hit in hits)
            {
                if (hit.collider != null)
                {
                    if (hit.collider.TryGetComponent(out IHitAble hitable))
                    {
                        hitable.Hit(_dir, _damage);
                    }

                    var normal = hit.normal;
                    var angle = Mathf.Atan2(normal.y, normal.x) * Mathf.Rad2Deg - 90;

                    
                    SystemManager.Instance.ResourceManager.ReleaseObject(RushEffectPath, _rushEffect);
                    SystemManager.Instance.ResourceManager.ReleaseObject(SparkEffectPath, _sparkEffect);
                    
                    SystemManager.Instance.ResourceManager.EmitParticle(ShockEffectPath, hit.point, angle);
                    _ownerEntity.RushShockImpulseSource.GenerateImpulse(normal * 3);
                }
            }

            if (hits.Length > 0)
            {
                _soundManager.PlayOneShot(_soundManager.SoundData.Boss_Rush_Crash, _ownerEntity.transform.position);
                return true;
            }

            return false;
        }
        
        
        private async void SetEffect()
        {
            _rushEffect = await SystemManager.Instance.ResourceManager.GetFromPool<ParticleSystem>(RushEffectPath, _ownerEntity.CenterTransform);
            _rushEffect.transform.ResetLocalTransform();

            var angle = Mathf.Atan2(_dir.y, _dir.x) * Mathf.Rad2Deg;
            _rushEffect.transform.rotation = Quaternion.Euler(0, 0, angle);
            
            _rushEffect.Play();
            
            
            _sparkEffect = await SystemManager.Instance.ResourceManager.GetFromPool<ParticleSystem>(SparkEffectPath, _ownerEntity.WheelTransform);
            _sparkEffect.transform.ResetLocalTransform();
        }

    }
}