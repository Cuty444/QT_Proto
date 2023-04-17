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
        public int ProjectileId => gameObject.GetInstanceID();
        public Vector2 Position => transform.position;

        [SerializeField] private float _ballHeight;
        [SerializeField] private Transform _ballObject;
        [SerializeField] private LayerMask _bounceMask;
        private TrailRenderer _trailRenderer;
        
        private float _maxSpeed;
        private float _speed;
        private float _speedDecay;

        private float _size;

        private int _maxBounce;
        private int _bounceCount;

        private Vector2 _direction;

        private float _damage;

        private bool _isDestroyed;
        private float _throwProjectileDelayedTime;
        private void Awake()
        {
            _speedDecay = 0f;//SystemManager.Instance.GetSystem<GlobalDataSystem>().GlobalData.SpdDecay;
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
            if (_isDestroyed)
            {
                _isDestroyed = false;
                SystemManager.Instance?.PlayerManager.PlayerThrowProjectileReleased.Invoke();
            }
        }

        public void Init(ProjectileGameData data, Vector2 dir, float speed, int maxBounce, LayerMask bounceMask)
        {
            _maxSpeed = _speed = speed;
            _size = data.ColliderRad * 0.5f;
            _damage = data.DirectDmg;

            _direction = dir;
            _bounceCount = _maxBounce = maxBounce;

            _bounceMask = bounceMask;
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
            _bounceCount = _maxBounce;
            
            _bounceMask = bounceMask;
        }
        
        public void ResetBounceCount(int maxBounce)
        {
            _bounceCount = _maxBounce = maxBounce;
        }

        public LayerMask GetLayerMask()
        {
            return _bounceMask;
        }

        public void ResetDelayedProjectile(float time)
        {
            _throwProjectileDelayedTime = time;
            _isDestroyed = false;
            _bounceMask = LayerMask.GetMask("Wall", "Enemy");
        }

        private void Update()
        {
            if (_throwProjectileDelayedTime > 0f)
            {
                PlayerProjectTileUpdate();
                if (_isDestroyed)
                {
                    _throwProjectileDelayedTime -= Time.deltaTime;
                }
                return;
            }
            var moveLength = _speed * Time.deltaTime;
            var hit = Physics2D.CircleCast(transform.position, _size, _direction, moveLength, _bounceMask);

            if (hit.collider != null)
            {
                _direction += hit.normal * (-2 * Vector2.Dot(_direction, hit.normal));
                if (--_bounceCount < 0)
                {
                    SystemManager.Instance.ResourceManager.ReleaseObject(this);
                }
            }

            transform.Translate(_direction * moveLength);

            _speed -= _speedDecay * Time.deltaTime;
            if (_speed <= 0)
            {
                SystemManager.Instance.ResourceManager.ReleaseObject(this);
            }

            // easeInQuad
            var height = _speed / _maxSpeed;
            height *= height;

            _ballObject.transform.localPosition = Vector3.up * (height * _ballHeight);
        }

        private void PlayerProjectTileUpdate()
        {
            if (_isDestroyed)
                return;
            var moveLength = _speed * Time.deltaTime;
            var hit = Physics2D.CircleCast(transform.position, _size, _direction, moveLength, _bounceMask);
            if (hit.collider != null)
            {
                _direction += hit.normal * (-2 * Vector2.Dot(_direction, hit.normal));
                if (--_bounceCount < 0)
                {
                    _speed = 0f;
                    _isDestroyed = true;
                    _bounceMask = LayerMask.GetMask("ProjectileThrow");
                    StartCoroutine(PlayerProjectileDelayed());
                    transform.position = hit.point + (hit.normal * _size);
                    return;
                    _trailRenderer.Clear();
                    SystemManager.Instance.ResourceManager.ReleaseObject(this);
                }
            }

            transform.Translate(_direction * moveLength);

            _speed -= _speedDecay * Time.deltaTime;
            return;
            if (_speed <= 0)
            {
                _trailRenderer.Clear();
                SystemManager.Instance.ResourceManager.ReleaseObject(this);
            }
        }

        private IEnumerator PlayerProjectileDelayed()
        {
            float delayTime = 0f;
            float distance = 1f * Time.deltaTime;
            while (delayTime < 0.5f) // TODO : 이 부분 추후 데이터 테이블 불러오기
            {
                delayTime += Time.deltaTime;
                transform.Translate(_direction * distance);
                yield return null;
            }

            _bounceMask = LayerMask.GetMask("Wall","ProjectileDelayed");
            _speed = 0f;
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