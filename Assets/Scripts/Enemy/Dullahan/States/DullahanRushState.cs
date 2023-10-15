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
        private enum RushState
        {
            Ready,
            Rushing,
            OnAir
        }
        
        private readonly int IsRushingAnimHash = Animator.StringToHash("IsRushing");
        private readonly int RushReadyAnimHash = Animator.StringToHash("RushReady");
        
        private const string RushEffectPath = "Effect/Prefabs/FX_Boss_Rush_Air_Resistance.prefab";
        private const string SparkEffectPath = "Effect/Prefabs/FX_Boss_Rush_Spark.prefab";
        private const string ShockEffectPath = "Effect/Prefabs/FX_Boss_Rush_Shock.prefab";
        
        private RushState _state=0;
        
        private Vector2 _dir;

        private float _time;
        
        private Transform _transform;
        private Transform _rushCenter;
        //
        private float _speed;
        private float _size;
        private float _damage;

        private int _rushSide = 0;

        private SoundManager _soundManager;
        
        private ParticleSystem _rushEffect;
        private ParticleSystem _sparkEffect;

        private DullahanData _data;
        
        public DullahanRushState(IFSMEntity owner) : base(owner)
        {
            _transform = _ownerEntity.transform;
            _rushCenter = _ownerEntity.CenterTransform;
            
            _data = _ownerEntity.DullahanData;
            
            SystemManager.Instance.ResourceManager.CacheAsset(RushEffectPath);
            SystemManager.Instance.ResourceManager.CacheAsset(SparkEffectPath);
        }

        public override void InitializeState()
        {
            _speed = _ownerEntity.DullahanData.RushSpeed;
            _size = _ownerEntity.RushColliderSize;
            _damage = _ownerEntity.DullahanData.RushHitDamage;

            _soundManager = SystemManager.Instance.SoundManager;

            _dir = (SystemManager.Instance.PlayerManager.Player.transform.position - _ownerEntity.transform.position);
            _rushSide = _ownerEntity.SetDir(_dir,2);

            _dir.Normalize();

            _state = RushState.Ready;
            _time = 0;

            _ownerEntity.Animator.SetTrigger(RushReadyAnimHash);
            _soundManager.PlayOneShot(_soundManager.SoundData.Boss_RushReady, _ownerEntity.transform.position);
            
            _ownerEntity.RushTrailObject.SetActive(true);
            _ownerEntity.Rigidbody.velocity = Vector2.zero;
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
            
            switch (_state)
            {
                case RushState.Ready:
                    Ready();
                    break;
                case RushState.Rushing:
                    Rushing();
                    break;
                case RushState.OnAir:
                    OnAir();
                    break;
            }
        }

        public override void FixedUpdateState()
        {
            if (_state != RushState.Rushing)
            {
                return;
            }
            
            if (CheckHit())
            {
                _state = RushState.OnAir;
                _time = 0;
                
                SystemManager.Instance.ResourceManager.ReleaseObject(RushEffectPath, _rushEffect);
                SystemManager.Instance.ResourceManager.ReleaseObject(SparkEffectPath, _sparkEffect);
                
                _ownerEntity.Animator.SetBool(IsRushingAnimHash, false);
                _ownerEntity.RushTrailObject.SetActive(false);
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

        private void Ready()
        {
            if (_time > _ownerEntity.DullahanData.RushReadyTime)
            {
                var dir = (SystemManager.Instance.PlayerManager.Player.transform.position - _ownerEntity.transform.position);
                if (dir.x > 0 == _dir.x > 0 && _rushSide == _ownerEntity.GetSide(dir, 2))
                {
                    _dir = dir.normalized;
                }
                
                _ownerEntity.RushTrailObject.SetActive(true);
                _ownerEntity.Animator.SetBool(IsRushingAnimHash, true);
                _soundManager.PlayOneShot(_soundManager.SoundData.Boss_Rush, _ownerEntity.transform.position);
                _ownerEntity.SetPhysics(false);
                _time = 0;
                    
                SetEffect();
                
                _state = RushState.Rushing;
            }
        }

        private void Rushing()
        {
            _transform.Translate(_dir * (_data.RushSpeed * Time.deltaTime));
            
            if (_time > _ownerEntity.DullahanData.RushLengthTime)
            {
                SystemManager.Instance.ResourceManager.ReleaseObject(RushEffectPath, _rushEffect);
                SystemManager.Instance.ResourceManager.ReleaseObject(SparkEffectPath, _sparkEffect);
                
                _ownerEntity.ChangeState(Dullahan.States.Normal);
                _time = 0;
            }
        }

        private void OnAir()
        {
            _transform.Translate(-_dir * (_data.RushAirSpeed * Time.deltaTime));
            
            if (_time > _ownerEntity.DullahanData.RushAirTime)
            {
                _ownerEntity.ChangeState(Dullahan.States.Stun);
                _time = 0;
            }
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