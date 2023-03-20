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

        #endregion

        #region Global_Declaration

        private Rigidbody2D _rigidbody2D;
        private Vector2 _moveDirection;
        private float _currentReduceSpeed;

        #endregion

        [SerializeField] private float _reduceSpeed; // 이 값은 추후 테이블 참조

        private void Start()
        {
            InputSystem inputSystem = SystemManager.Instance.GetSystem<InputSystem>();
            inputSystem.OnKeyMoveEvent.AddListener(SetMoveDirection);
            inputSystem.OnKeyDownAttackEvent.AddListener(AttackOn);
            GlobalDataSystem globalDataSystem = SystemManager.Instance.GetSystem<GlobalDataSystem>();
            _speed = globalDataSystem.CharacterTable.MovementSpd;
            _rigidbody2D = GetComponent<Rigidbody2D>();
            GetComponent<CircleCollider2D>().radius = globalDataSystem.CharacterTable.PCHitBoxRad;
            _currentReduceSpeed = 1f;
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


        void Move()
        {
            _rigidbody2D.velocity = _moveDirection * _speed * Time.fixedDeltaTime /** _currentReduceSpeed*/;
        }
    }
}