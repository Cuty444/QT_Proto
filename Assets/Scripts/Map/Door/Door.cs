using QT.Core;
using Spine.Unity;
using UnityEngine;

namespace QT
{
    public class Door : MonoBehaviour
    {
        private Animator _animator;
        private SkeletonMecanim _skeletonMecanim;
        private readonly int AnimationOpenHash = Animator.StringToHash("Open");

        private bool _isOpen = false;
        
        private Vector2Int _direction;
        
        private void Awake()
        {
            _animator = GetComponentInChildren<Animator>();
            _skeletonMecanim = GetComponentInChildren<SkeletonMecanim>();
        }

        public void Init(Vector2Int direction)
        {
            _isOpen = false;
            _direction = direction;
        }
        
        public void DoorOpen()
        {
            _isOpen = true;
            _animator.SetTrigger(AnimationOpenHash);
        }

        private void OnTriggerEnter2D(Collider2D col)
        {
            if (_isOpen)
            {
                SystemManager.Instance.PlayerManager.PlayerDoorEnter.Invoke(_direction);
            }
        }
    }
}
