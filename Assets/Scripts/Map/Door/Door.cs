using QT.Util;
using UnityEngine;
using UnityEngine.Events;

namespace QT
{
    public class Door : MonoBehaviour
    {
        private const float DoorOpenDelay = 0.5f;
        
        private TweenAnimator _animator;
        private Collider2D _collider2D;

        private bool _isOpen = false;
        
        private Vector2Int _direction;
        private UnityAction<Vector2Int> _onDoorEnter;
        

        private void Awake()
        {
            _animator = GetComponentInChildren<TweenAnimator>();
            _collider2D = GetComponentInChildren<Collider2D>();
            _animator?.Reset();
            
            if (_animator != null)
            {
                _collider2D.enabled = false;
            }
        }

        public void Init(Vector2Int direction, UnityAction<Vector2Int> onDoorEnter)
        {
            _isOpen = false;
            _direction = direction;
            _onDoorEnter = onDoorEnter;
        }

        public void DoorOpen()
        {
            _isOpen = true;
            
            if (_animator != null && gameObject.activeInHierarchy)
            {
                _animator?.ReStart();
                StartCoroutine(UnityUtil.WaitForFunc(() => _collider2D.enabled = true, DoorOpenDelay));
            }
            else
            {
                _collider2D.enabled = true;
            }
        }

        private void OnTriggerEnter2D(Collider2D col)
        {
            if (_isOpen)
            {
                _onDoorEnter?.Invoke(_direction);
            }
        }
    }
}
