using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace QT.Core.Input
{
    public class InputSystem : SystemBase
    {
        private UnityEvent<Vector2> _onKeyMoveEvent = new UnityEvent<Vector2>();
        public UnityEvent<Vector2> OnKeyMoveEvent { get => _onKeyMoveEvent; }

        private UnityEvent<Vector2> _onKeyDownAttackEvent = new UnityEvent<Vector2>();
        public UnityEvent<Vector2> OnKeyDownAttackEvent { get => _onKeyDownAttackEvent; }

        private UnityEvent _onRightKeyDownGrapEvent = new UnityEvent();
        public UnityEvent OnRightKeyDownGrapEvent { get => _onRightKeyDownGrapEvent; }


        public override void OnInitialized()
        {
            base.OnInitialized();
        }

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
                OnKeyDownAttackEvent.Invoke(UnityEngine.Input.mousePosition);
            }
        }

        private  void RightMouseInputGrap()
        {
            if(UnityEngine.Input.GetMouseButtonDown(1))
            {
                Debug.Log("¿ìÅ¬¸¯");
                OnRightKeyDownGrapEvent.Invoke();
            }
        }
    }

}