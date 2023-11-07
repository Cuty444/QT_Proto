using QT.Core;
using UnityEngine;

namespace QT
{
    public class Coin : MonoBehaviour
    {
        public static async void Spawn(int count, Vector2 position, Vector2 dir, float power)
        {
            for (int i = 0; i < count; i++)
            {
                var coin = await SystemManager.Instance.ResourceManager.GetFromPool<Coin>(Constant.CoinPrefabPath);
                coin.transform.position = position;
                coin.Drop(dir, power);
            }

            SystemManager.Instance.SoundManager.PlayOneShot(SystemManager.Instance.SoundManager.SoundData.Coin_GetSFX);
        }
        
        public static async void SpawnDelayStay(int count, Vector2 position, Vector2 dir, float power,float delayTime)
        {
            for (int i = 0; i < count; i++)
            {
                var coin = await SystemManager.Instance.ResourceManager.GetFromPool<Coin>(Constant.CoinPrefabPath);
                coin.transform.position = position;
                coin.Drop(dir, power);
                coin._isDelay = true;
                coin.Invoke(nameof(DelayOff), delayTime);
            }
            
            SystemManager.Instance.SoundManager.PlayOneShot(SystemManager.Instance.SoundManager.SoundData.Coin_GetSFX);
        }
        
        
        
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

        [Space] 
        [SerializeField] private float _gainStartSpeed = 4;

        private float _time;

        private float _maxSpeed;
        private float _speed;

        private Vector2 _direction;

        private float _currentStretch;

        private int _state = 0;

        private Transform _player;

        private bool _isDelay = false;

        public void Drop(Vector2 dir, float power)
        {
            var randomDir =  new Vector2(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f)).normalized;
            dir = (dir * 1.5f + randomDir).normalized;
            
            var randomSpeed = Random.Range(0.5f, 1.2f);
            _maxSpeed = _speed = power * SpeedMultiplier * randomSpeed;
            _direction = dir;
            
            gameObject.SetActive(true);
            _state = 0;

            _player = SystemManager.Instance.PlayerManager.Player.transform;
        }

        private void Update()
        {
            if (_player == null)
            {
                SystemManager.Instance.ResourceManager.ReleaseObject(Constant.CoinPrefabPath, this);
                return;
            }
            

            switch (_state)
            {
                case 0:
                    DropMove();
                    BounceEffect();
                    if (_isDelay)
                    {
                        return;
                    }
                    
                    if (_speed <= 0)
                    {
                        _state++;
                    }
                    else if (!DungeonManager.Instance.IsBattle)
                    {
                        _state = 2;
                        _speed = _gainStartSpeed;
                    }
                    break;
                
                case 1:
                    if ((_player.position - transform.position).sqrMagnitude < 0.5f || !DungeonManager.Instance.IsBattle)
                    {
                        _state++;
                        _speed = _gainStartSpeed;
                    }
                    break;
                
                case 2:
                    transform.position = Vector2.MoveTowards(transform.position, _player.position, _speed * Time.deltaTime);
                    _speed += _speed * Time.deltaTime;
                    
                    if ((_player.position - transform.position).sqrMagnitude < 0.1f)
                    {
                        SystemManager.Instance.PlayerManager.OnGoldValueChanged.Invoke(1);
                        SystemManager.Instance.ResourceManager.ReleaseObject(Constant.CoinPrefabPath, this);
                        
                        SystemManager.Instance.SoundManager.PlayOneShot(SystemManager.Instance.SoundManager.SoundData.Coin_GetSFX);
                        _state++;
                    }
                    break;
            }
        }

        private void DropMove()
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

        private void DelayOff()
        {
            _isDelay = false;
        }
        
    }
}