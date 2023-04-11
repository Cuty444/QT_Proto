using System;
using System.Collections;
using System.Collections.Generic;
using QT.Core;
using QT.Core.Data;
using QT.Core.Input;
using QT.Core.Player;
using QT.UI;
using UnityEngine;
using UnityEngine.UI;

public class PlayerDodge : MonoBehaviour
{
    #region StartData_Declaration

    private PlayerSystem _playerSystem;
    private float _dodgeCoolDown;
    private float _decelerationSpeed;
    private float _stopDistance  = 3f; // 멈출 거리
    #endregion
    
    #region Global_Declaration
    
    private Rigidbody2D _rigidbody2D;
    private Image _invicibleBar;
    private Vector2 _moveDirection;
    private float _speed;
    private int _dodgeState;
    private float _minDrag;
    private float _currentDodgeCoolTime;
    #endregion
    
    [SerializeField] private float dragMultiplier;

    private void Start()
    {
        GlobalDataSystem globalDataSystem = SystemManager.Instance.GetSystem<GlobalDataSystem>();
        _dodgeCoolDown = globalDataSystem.CharacterTable.DodgeCooldown;
        _decelerationSpeed = globalDataSystem.CharacterTable.DodgeDecelerationSpeed;
        _stopDistance = globalDataSystem.CharacterTable.DodgeStopDistance;
        _speed = globalDataSystem.CharacterTable.MovementSpd;
        _currentDodgeCoolTime = _dodgeCoolDown;
        InputSystem inputSystem = SystemManager.Instance.GetSystem<InputSystem>();
        inputSystem.OnKeySpaceDodgeEvent.AddListener(OnDodge);
        inputSystem.OnKeyMoveEvent.AddListener(SetMoveDirection);
        _playerSystem = SystemManager.Instance.GetSystem<PlayerSystem>();
        _playerSystem.PlayerCollisionEnemyEvent.AddListener(EnemyCollisionDragStop);;
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _invicibleBar = UIManager.Instance.GetUIPanel<PlayerHPCanvas>().PlayerInvicibleImage;
        _invicibleBar.fillAmount = QT.Util.Math.Remap(_currentDodgeCoolTime, _dodgeCoolDown, 0);
        _dodgeState = 0;
    }

    private void Update()
    {
        _currentDodgeCoolTime += Time.deltaTime;
        _invicibleBar.fillAmount = QT.Util.Math.Remap(_currentDodgeCoolTime, _dodgeCoolDown, 0);
    }

    private void SetMoveDirection(Vector2 dir)
    {
        _moveDirection = dir.normalized;
    }
    private void FixedUpdate()
    {
        if (_dodgeState > 1)
        {
            float speed = _rigidbody2D.velocity.magnitude;
            float drag = speed / (_stopDistance * dragMultiplier);
            _rigidbody2D.drag = drag;
            if (_rigidbody2D.drag <= _minDrag)
            {
                _rigidbody2D.drag = 0f;
                _dodgeState = 0;
                _playerSystem.DodgeEvent.Invoke(false);
                Debug.Log("Dodge End");
                //_rigidbody2D.mass = 100;
            }
        }
    }
    private void OnDodge()
    {
        if (_dodgeState > 0 || _moveDirection == Vector2.zero || _currentDodgeCoolTime <= _dodgeCoolDown)
            return;
        _playerSystem.DodgeEvent.Invoke(true);
        _dodgeState++;
        _rigidbody2D.velocity = Vector2.zero;
        _currentDodgeCoolTime = 0f;
        //_rigidbody2D.mass = 1000;
        _rigidbody2D.AddForce(_moveDirection * (_speed * _decelerationSpeed),ForceMode2D.Impulse);
        _minDrag = Mathf.Lerp(0f, _rigidbody2D.velocity.magnitude / (_stopDistance * dragMultiplier), 0.05f);
            Debug.Log(_minDrag);
        float dodgeMaxTime = _stopDistance / _rigidbody2D.velocity.magnitude;
        Debug.Log("Time : " + dodgeMaxTime);
        StartCoroutine(QT.Util.UnityUtil.WaitForFunc(StopDodge, dodgeMaxTime));
    }

    private void StopDodge()
    {
        _dodgeState++;
        //_rigidbody2D.velocity = Vector2.zero;
        //_playerSystem.DodgeEvent.Invoke(false);
    }

    private void EnemyCollisionDragStop()
    {
        if (_dodgeState < 1)
            return;
        _minDrag = float.MaxValue;
    }
}
