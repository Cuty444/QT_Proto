using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Experimental.GlobalIllumination;

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

        private UnityEvent _onKeySpaceDodgeEvent = new UnityEvent();
        public UnityEvent OnKeySpaceDodgeEvent => _onKeySpaceDodgeEvent;

        private UnityEvent _onKeyEThrowEvent = new UnityEvent();
        public UnityEvent OnKeyEThrowEvent => _onKeyEThrowEvent;

        private void Update()
        {
            SpaceKeyInputDodge();
            KeyInputMove();
            EKeyInputThrow();
            MouseInputAttack();
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

        private void SpaceKeyInputDodge()
        {
            if (UnityEngine.Input.GetKeyDown(KeyCode.Space))
            {
                OnKeySpaceDodgeEvent.Invoke();
            }
        }

        private void EKeyInputThrow()
        {
            if (UnityEngine.Input.GetKeyDown(KeyCode.E))
            {
                OnKeyEThrowEvent.Invoke();
            }
        }
    }

}