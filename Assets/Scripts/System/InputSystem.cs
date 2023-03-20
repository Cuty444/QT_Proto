using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace QT.Core.Input
{
    public class InputSystem : SystemBase
    {
        private UnityEvent<Vector2> _onKeyMoveEvent = new UnityEvent<Vector2>();
        public UnityEvent<Vector2> OnKeyMoveEvent => _onKeyMoveEvent;

        private UnityEvent _onKeyDownAttackEvent = new UnityEvent();
        public UnityEvent OnKeyDownAttackEvent => _onKeyDownAttackEvent;

        private UnityEvent _onKeyUpAttackEvent = new UnityEvent();
        public UnityEvent OnKeyUpAttackEvent => _onKeyUpAttackEvent;

        private UnityEvent _onRightKeyDownGrapEvent = new UnityEvent();
        public UnityEvent OnRightKeyDownGrapEvent => _onRightKeyDownGrapEvent;

        private void Update()
        {
            KeyInputMove();
            MouseInputAttack();
            RightMouseInputGrap();
        }

        private void KeyInputMove()
        {
            Vector2 dir = new Vector2(UnityEngine.Input.GetAxisRaw("Horizontal"), UnityEngine.Input.GetAxisRaw("Vertical"));
            OnKeyMoveEvent.Invoke(dir);
        }

        private void MouseInputAttack()
        {
            if(UnityEngine.Input.GetMouseButtonDown(0))
            {
                OnKeyDownAttackEvent.Invoke();
            }
            else if (UnityEngine.Input.GetMouseButtonUp(0))
            {
                OnKeyUpAttackEvent.Invoke();
            }
        }

        private  void RightMouseInputGrap() // 현재 미사용 시스템
        {
            if(UnityEngine.Input.GetMouseButtonDown(1))
            {
                Debug.Log("우클릭");
                OnRightKeyDownGrapEvent.Invoke();
            }
        }
    }

}