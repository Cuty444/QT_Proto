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
        
        [SerializeField] private LayerMask _bounceMask;

        private float _speed;
        private float _speedDecay;

        private float _size;

        private int _maxBounce;
        private int _bounceCount;

        private Vector2 _direction;

        private float _damage;

        
        private void Awake()
        {
            _speedDecay = SystemManager.Instance.GetSystem<GlobalDataSystem>().GlobalData.SpdDecay;
        }

        public void Init(ProjectileGameData data, Vector2 dir, float speed, int maxBounce)
        {
            _speed = speed;
            _size = data.ColliderRad;
            _damage = data.DirectDmg;

            _direction = dir;
            _bounceCount = _maxBounce = maxBounce;
        }
        
        public void Hit(Vector2 dir, float newSpeed)
        {
            _direction = dir;
            _speed = newSpeed;
            _bounceCount = _maxBounce;
        }

        private void Update()
        {
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
        }
    }

}