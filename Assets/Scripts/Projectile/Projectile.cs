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

        private float _maxSpeed;
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

        private void OnEnable()
        {
            SystemManager.Instance.ProjectileManager.Register(this);
        }

        private void OnDisable()
        {
            SystemManager.Instance.ProjectileManager.UnRegister(this);
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
                _speed = 0.1f;
                //SystemManager.Instance.ResourceManager.ReleaseObject(this);
            }

            // easeInQuad
            var height = _speed / _maxSpeed;
            height *= height;

            _ballObject.transform.localPosition = Vector3.up * (height * _ballHeight);
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