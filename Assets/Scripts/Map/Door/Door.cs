using QT.Core;
using Spine.Unity;
using UnityEngine;
using UnityEngine.Events;

namespace QT
{
    public class Door : MonoBehaviour
    {
        private TweenAnimator _animator;
        private readonly int AnimationOpenHash = Animator.StringToHash("Open");

        private bool _isOpen = false;
        
        private Vector2Int _direction;
        private UnityAction<Vector2Int> _onDoorEnter;

        private void Awake()
        {
            _animator = GetComponentInChildren<TweenAnimator>();
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
            _animator?.ReStart();
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
