using System.Collections;
using System.Collections.Generic;
using QT.Core;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace QT.InGame
{
    public partial class Player
    {
        public enum ButtonActions // bool 값으로 입력받는 액션
        {
            Swing,
            Dodge,
            Throw,
        }

        public enum ValueActions //float 값으로 입력받는 액션
        {
        }


        // 벡터넘겨주는 액션은 따로 처리
        public UnityAction<Vector2> OnMove { get; set; }
        private InputVector2Damper moveInputDamper = new ();

        public UnityEvent<Vector2> OnLook { get; private set; } = new();
        public Vector2 LookDir { get; private set; } = Vector2.zero;

        
        private Dictionary<ButtonActions, InputAction> buttonActions;
        private Dictionary<ButtonActions, UnityAction<bool>> buttonEvents;

        private Dictionary<ValueActions, InputAction> valueActions;
        private Dictionary<ValueActions, UnityAction<bool>> valueEvents;


        private PlayerInputActions inputActions;
        private InputAction moveInputAction;
        private InputAction lookInputAction;
        
        private Camera _camera;

        private void InitInputs()
        {
            buttonActions = new Dictionary<ButtonActions, InputAction>();
            buttonEvents = new Dictionary<ButtonActions, UnityAction<bool>>();

            valueActions = new Dictionary<ValueActions, InputAction>();
            valueEvents = new Dictionary<ValueActions, UnityAction<bool>>();

            inputActions = new PlayerInputActions();

            moveInputAction = inputActions.Player.Move;
            lookInputAction = inputActions.Player.Look;

            SetButtonAction(inputActions.Player.Swing, ButtonActions.Swing);
            SetButtonAction(inputActions.Player.Throw, ButtonActions.Throw);
            SetButtonAction(inputActions.Player.Dodge, ButtonActions.Dodge);

            _camera = Camera.main;
        }

        private void SetButtonAction(InputAction action, ButtonActions type)
        {
            buttonActions.Add(type, action);
            action.started += (x) => GetAction(type)?.Invoke(true);
            action.canceled += (x) => GetAction(type)?.Invoke(false);
        }

        private void UpdateInputs()
        {
            var moveInput = moveInputDamper.GetDampedValue(moveInputAction.ReadValue<Vector2>(), Time.deltaTime);
            var lookInput = lookInputAction.ReadValue<Vector2>();
            
            Vector2 mousePos = _camera.ScreenToWorldPoint(lookInput);
            Vector2 lookDir = ((Vector2)transform.position - mousePos).normalized;

            if (lookDir != Vector2.zero)
            {
                LookDir = lookDir;
            }
            
            OnMove?.Invoke(moveInput);
            OnLook?.Invoke(LookDir);
        }

        private void OnEnable()
        {
            inputActions.Enable();
        }

        private void OnDisable()
        {
            inputActions.Disable();
        }


        private UnityAction<bool> GetAction(ButtonActions type)
        {
            if (buttonEvents.TryGetValue(type, out var action))
            {
                return action;
            }
            return null;
        }

        private UnityAction<bool> GetAction(ValueActions type)
        {
            if (valueEvents.TryGetValue(type, out var action))
            {
                return action;
            }
            return null;
        }

        public bool GetActionValue(ButtonActions type)
        {
            if (buttonActions.TryGetValue(type, out var input))
            {
                return input.IsPressed();
            }
            return false;
        }

        public float GetActionValue(ValueActions type)
        {
            if (valueActions.TryGetValue(type, out var input))
            {
                return input.ReadValue<float>();
            }

            return 0;
        }

        public void SetAction(ButtonActions type, UnityAction<bool> action, bool update = false)
        {
            if (!buttonEvents.ContainsKey(type))
            {
                buttonEvents.Add(type, action);
            }
            else
            {
                buttonEvents[type] = action;
            }

            if (update && buttonActions.TryGetValue(type, out var input))
            {
                action?.Invoke(input.IsPressed());
            }
        }

        public void ClearAction(ButtonActions type)
        {
            if (buttonEvents.ContainsKey(type))
            {
                buttonEvents.Remove(type);
            }
        }
    }
}
