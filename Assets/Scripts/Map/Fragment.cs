using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace QT
{
    public class Fragment : MonoBehaviour
    {
        private const float SpeedMultiplier = 0.2f;
        private static LayerMask BounceMask => LayerMask.GetMask("Wall", "HardCollider", "ProjectileCollider", "Enemy", "InteractionCollider", "Fall");

        [Space] 
        [SerializeField] private Transform _targetTransform;
        [SerializeField] private Transform _target;

        [Space] 
        [SerializeField] private float _decaySpeed;
        [SerializeField] private float _colliderRad;

        [Space] 
        [SerializeField] private float _maxSquashLength;
        [SerializeField] private float _maxStretchLength;
        [SerializeField] private float _dampSpeed;
        [SerializeField] private float _rotateSpeed;

        [Space] 
        [SerializeField] private float _frequency = 5;
        [SerializeField] private float _minHeight = 0.5f;
        [SerializeField] private float _height = 0.5f;

        private float _time;

        private float _maxSpeed;
        private float _speed;

        private Vector2 _direction;

        private float _currentStretch;

        private void Awake()
        {
            gameObject.SetActive(false);
        }

        
        public void Hit(Vector2 dir, float power)
        {
            var randomDir =  new Vector2(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f)).normalized;
            dir = (dir * 1.5f + randomDir).normalized;
            
            var randomSpeed = Random.Range(0.5f, 1.2f);
            _maxSpeed = _speed = power * SpeedMultiplier * randomSpeed;
            _direction = dir;
            
            gameObject.SetActive(true);
        }
        
        private void Update()
        {
            if (_speed > 0)
            {
                Move();
                BounceEffect();
            }
        }

        private void Move()
        {
            var hit = Physics2D.CircleCast(transform.position, _colliderRad, _direction, _speed * Time.deltaTime, BounceMask);

            if (hit.collider != null)
            {
                _direction = Vector2.Reflect(_direction, hit.normal);
                transform.Translate(hit.normal * _colliderRad);
            }
            
            _speed -= _decaySpeed * Time.deltaTime;
            _speed = Mathf.Max(_speed, 0);
            
            transform.Translate(_direction * (_speed * Time.deltaTime));
        }

        private void BounceEffect()
        {
            // easeInQuad
            var height = _speed / _maxSpeed;
            height *= height;

            var pos = Vector2.zero;

            _time += Time.deltaTime * _frequency;
            _time %= Mathf.PI;

            pos.y = Mathf.Sin(_time) * _height * height + _minHeight;

            _targetTransform.localPosition = pos;
            
            
            _target.Rotate(Vector3.forward, _speed * Time.deltaTime * _rotateSpeed);

            _currentStretch = Mathf.Lerp(_currentStretch, height, _dampSpeed * Time.deltaTime);
            _targetTransform.localScale = GetSquashSquashValue(_currentStretch);
            _targetTransform.up = Vector2.Lerp(_targetTransform.up, _direction, _dampSpeed * Time.deltaTime);
        }

        private Vector2 GetSquashSquashValue(float power)
        {
            if (power > 0)
            {
                power = Mathf.Lerp(1, _maxStretchLength, power);
            }
            else if (power < 0)
            {
                power = Mathf.Lerp(_maxSquashLength, 1, 1 + power);
            }

            return new Vector2(2 - power, power);
        }
        
    }
}