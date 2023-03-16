using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QT.Core;
using QT.Core.Input;
using QT.Data;
namespace QT.Player
{

    public class PlayerMove : MonoBehaviour
    {
        [SerializeField] private float _reduceSpeed;

        private Vector2 _moveDirection;
        private Rigidbody2D _rigidbody2D;
        private float _currentReduceSpeed;
        private float _speed;
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
            _rigidbody2D.velocity = _moveDirection * _speed * Time.fixedDeltaTime/** _currentReduceSpeed*/;
        }
    }

}