using System;
using System.Collections;
using System.Collections.Generic;
using QT.Core;
using QT.Core.Data;
using UnityEngine;

namespace QT.Enemy
{
    public class EnemyFSM : MonoBehaviour
    {
        #region StartData_Declaration

        private float _movementSpeed;

        #endregion

        #region Global_Declaration

        private Transform _playerTransform;
        private Rigidbody2D _rigidbody2D;

        #endregion

        private void Start()
        {
            _playerTransform = SystemManager.Instance.PlayerManager.Player.transform;
            _rigidbody2D = GetComponent<Rigidbody2D>();
            _movementSpeed = SystemManager.Instance.GetSystem<GlobalDataSystem>().EnemyTable.MovementSpd;
        }

        private void FixedUpdate()
        {
            EnemyMove();
        }


        private void EnemyMove()
        {
            float enemyAngleDegree = QT.Util.Math.GetDegree(transform.position, _playerTransform.position);
            float angle = enemyAngleDegree * Mathf.Deg2Rad;

            _rigidbody2D.velocity =
                new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * (_movementSpeed * Time.fixedDeltaTime);
        }
    }
}