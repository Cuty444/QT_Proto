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
        private readonly int RotatationAnimHash = Animator.StringToHash("Rotation");
        
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

        [HideInInspector] public bool IsMoveFlip;
        
        [HideInInspector] public float LastSwingTime;
        
        private InputAngleDamper _roationDamper = new (5);
        private float _lockAnimRotationTime = 0;
        private float _lockAnimRotationTimer = 0;
        
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

            _lockAnimRotationTimer += Time.deltaTime;


            var aimDir = ((Vector2) transform.position - AimPosition).normalized;
            
            SetEyeAngle(aimDir);
            
            if (CurrentStateIndex == (int) States.Swing)
            {
                SetRotation(aimDir);
            }
            else
            {
                SetRotation(-moveInput);
            }
            
            
            // if (CurrentStateIndex == (int) States.Swing)
            // {
            //     var aimDir = ((Vector2) transform.position - AimPosition).normalized;
            //     Aim(aimDir);
            // }
            // else
            // {
            //     Aim(-moveInput);
            // }
            
            OnAim?.Invoke(AimPosition);
        }

        public void LockAnimation(float lockTime)
        {
            _lockAnimRotationTime = lockTime;
            _lockAnimRotationTimer = 0;
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
            if (Time.timeScale == 0)
            {
                return null;
            }

            if (buttonEvents.TryGetValue(type, out var action))
            {
                return action;
            }
            return null;
        }

        private UnityAction<bool> GetAction(ValueActions type)
        {
            if (Time.timeScale == 0)
            {
                return null;
            }
            
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
        
        public void ResetSwingTime(bool isPressed)
        {
            if (isPressed)
            {
                LastSwingTime = Time.time;
            }
            else
            {
                LastSwingTime = -9999;
            }
        }
        
        public bool IsSwingAble()
        {
            return LastSwingTime + 0.2f > Time.time;
        }

        private void SetEyeAngle(Vector2 aimDir)
        {
            if (aimDir == Vector2.zero)
            {
                return;
            }
            
            if (IsReverseLookDir)
            {
                aimDir *= -1;
            }
            
            var angle = Mathf.Atan2(aimDir.y, aimDir.x) * Mathf.Rad2Deg - 90;
            //angle = _roationDamper.GetDampedValue(angle, Time.deltaTime);
            
            if (angle < 0)
            {
                angle += 360;
            }

            EyeTransform.rotation = Quaternion.Euler(0, 0, angle + 270);
        }
        
        private void SetRotation(Vector2 dir)
        {
            if (_lockAnimRotationTimer < _lockAnimRotationTime)
            {
                return;
            }
            
            if (dir == Vector2.zero)
            {
                return;
            }
            
            if (IsReverseLookDir)
            {
                dir *= -1;
            }
            
            float flip = 180;
            if (IsDodge)
            {
                flip = IsFlip ? 180f : 0f;
                Animator.transform.rotation = Quaternion.Euler(0f, flip, 0f);
                return;
            }
            
            var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90;
            angle = _roationDamper.GetDampedValue(angle, Time.deltaTime);
            
            if (angle < 0)
            {
                angle += 360;
            }

            if (angle > 180)
            {
                angle = 360 - angle;
                flip = 0;
            }
            
            Animator.SetFloat(RotatationAnimHash, angle / 180 * 5);
            Animator.transform.rotation = Quaternion.Euler(0f, flip, 0f);
        }

    }
}
