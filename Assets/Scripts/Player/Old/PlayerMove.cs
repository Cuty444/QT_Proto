using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QT.Core;
using QT.Core.Input;
using QT.Core.Data;

namespace QT.Player
{
    public class PlayerMove : MonoBehaviour
    {
        #region StartData_Declaration

        private float _speed;
        private float _reduceSpeed;

        #endregion

        #region Global_Declaration

        private Rigidbody2D _rigidbody2D;
        private Vector2 _moveDirection;
        private float _currentReduceSpeed;
        private bool _isDodge;
        #endregion
        
        private void Start()
        {
            InputSystem inputSystem = SystemManager.Instance.GetSystem<InputSystem>();
            inputSystem.OnKeyMoveEvent.AddListener(SetMoveDirection);
            inputSystem.OnKeyDownAttackEvent.AddListener(AttackOn);
            inputSystem.OnKeyUpAttackEvent.AddListener(AttackOff);
            GlobalDataSystem globalDataSystem = SystemManager.Instance.GetSystem<GlobalDataSystem>();
            _speed = globalDataSystem.CharacterTable.MovementSpd;
            _rigidbody2D = GetComponent<Rigidbody2D>();
            _reduceSpeed = globalDataSystem.CharacterTable.ChargeMovementDecreasePer;
            GetComponent<CircleCollider2D>().radius = globalDataSystem.CharacterTable.PCHitBoxRad;
            _currentReduceSpeed = 1f;
            SystemManager.Instance.PlayerManager.DodgeEvent.AddListener(Dodging);
            _isDodge = false;
        }

        private void FixedUpdate()
        {
            Move();
        }

        private void SetMoveDirection(Vector2 dir)
        {
            _moveDirection = dir.normalized;
        }

        private void AttackOn()
        {
            _currentReduceSpeed = _reduceSpeed;
        }

        private void AttackOff()
        {
            _currentReduceSpeed = 1f;
        }


        private void Move()
        {
            if(_isDodge)
                return;
            _rigidbody2D.velocity = _moveDirection * (_speed * Time.fixedDeltaTime * _currentReduceSpeed);
        }

        private void Dodging(bool isDodge)
        {
            _isDodge = isDodge;
        }
    }
}