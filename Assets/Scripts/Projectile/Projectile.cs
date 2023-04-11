using System;
using System.Collections;
using System.Collections.Generic;
using QT.Core;
using UnityEngine;

namespace QT
{
    public class Projectile : MonoBehaviour
    {
        [SerializeField] private LayerMask _bounceMask;

        private TrailRenderer _trailRenderer;

        private float _speed;
        private float _speedDecay;

        private float _size;

        private int _maxBounce;
        private int _bounceCount;

        private Vector2 _direction;
        private Vector2 _beforePos;

        private int _damage;

        private bool _isPlayer;
        private bool _isDestroyed;

        private void Awake()
        {
            _trailRenderer = GetComponent<TrailRenderer>();
            _beforePos = Vector2.zero;
        }

        public void Init(ProjectileGameData data, Vector2 dir, int maxBounce)
        {
            _speed = data.InitalSpd;
            _speedDecay = data.SpdDecay;
            _size = data.ColliderRad;

            _direction = dir;
            _bounceCount = _maxBounce = maxBounce;
        }

        public void Init(float speed, float speedDecay, float radius, Vector2 dir, int maxBounce,int damage, bool isPlayer)
        {
            _speed = speed;
            _speedDecay = speedDecay;
            _size = radius;

            _direction = dir;
            _bounceCount = _maxBounce = maxBounce;
            _isPlayer = isPlayer;
            _bounceMask = (1 << LayerMask.NameToLayer("Enemy")) | (1 << LayerMask.NameToLayer("Wall"));
            _isDestroyed = false;
        }
        
        public void Init(float speed, float speedDecay, float radius, Vector2 dir, int maxBounce,int damage, bool isPlayer,LayerMask _layerMask)
        {
            _speed = speed;
            _speedDecay = speedDecay;
            _size = radius;

            _direction = dir;
            _bounceCount = _maxBounce = maxBounce;
            _isPlayer = isPlayer;
            _bounceMask = _layerMask;
            _isDestroyed = false;

        }

        public void ChangerDirection(Vector2 direction, float newSpeed)
        {
            _direction = direction;
            _speed = newSpeed;
            _bounceCount = _maxBounce;
        }

        private void Update()
        {
            if (_isPlayer)
            {
                PlayerProjectTileUpdate();
                return;
            }
            var moveLength = _speed * Time.deltaTime;
            var hit = Physics2D.CircleCast(transform.position, _size, _direction, moveLength, _bounceMask);

            if (hit.collider != null)
            {
                _direction += hit.normal * (-2 * Vector2.Dot(_direction, hit.normal));
                if (--_bounceCount < 0)
                {
                    _trailRenderer.Clear();
                    SystemManager.Instance.ResourceManager.ReleaseObject(this);
                }
            }

            transform.Translate(_direction * moveLength);

            _speed -= _speedDecay * Time.deltaTime;
            if (_speed <= 0)
            {
                _trailRenderer.Clear();
                SystemManager.Instance.ResourceManager.ReleaseObject(this);
            }
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
    }

}