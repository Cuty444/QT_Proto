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
            Active,
            Interaction,
        }

        public enum ValueActions //float 값으로 입력받는 액션
        {
        }


        // 벡터넘겨주는 액션은 따로 처리
        public UnityAction<Vector2> OnMove { get; set; }

        public UnityEvent<Vector2> OnAim { get; private set; } = new();
        public UnityEvent<bool> OnActive { get; } = new();
        public Vector2 AimPosition { get; private set; } = Vector2.zero;

        
        private Dictionary<ButtonActions, InputAction> buttonActions;
        private Dictionary<ButtonActions, UnityAction<bool>> buttonEvents;

        private Dictionary<ValueActions, InputAction> valueActions;
        private Dictionary<ValueActions, UnityAction<bool>> valueEvents;


        private PlayerInputActions inputActions;
        private InputAction moveInputAction;
        private InputAction lookInputAction;
        
        private Camera _camera;

        [HideInInspector] public bool IsDodge;
        [HideInInspector] public bool IsFlip;
        [HideInInspector] public bool IsReverseLookDir = false;
        
        private void InitInputs()
        {
            buttonActions = new Dictionary<ButtonActions, InputAction>();
            buttonEvents = new Dictionary<ButtonActions, UnityAction<bool>>();

            valueActions = new Dictionary<ValueActions, InputAction>();
            valueEvents = new Dictionary<ValueActions, UnityAction<bool>>();

            inputActions = new PlayerInputActions();

            moveInputAction = inputActions.Player.Move;
            lookInputAction = inputActions.Player.Look;

            inputActions.Player.Active.started += (x) => OnActive.Invoke(true);
            inputActions.Player.Active.canceled += (x) => OnActive.Invoke(false);

            SetButtonAction(inputActions.Player.Swing, ButtonActions.Swing);
            SetButtonAction(inputActions.Player.Dodge, ButtonActions.Dodge);
            SetButtonAction(inputActions.Player.Active, ButtonActions.Active);
            SetButtonAction(inputActions.Player.Interaction, ButtonActions.Interaction);
            
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
            var moveInput = moveInputAction.ReadValue<Vector2>();
            var lookInput = lookInputAction.ReadValue<Vector2>();
            
            AimPosition = _camera.ScreenToWorldPoint(lookInput);

            OnMove?.Invoke(moveInput);
            OnAim?.Invoke(AimPosition);
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
