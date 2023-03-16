using System;
using System.Collections;
using System.Collections.Generic;
using QT.Core;
using QT.Core.Enemy;
using QT.Data;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class EnemyFSM : MonoBehaviour
{
    private Transform _playerTransform;
    private Rigidbody2D _rigidbody2D;
    private float _movementSpeed;

    private bool _isMove;
    public bool IsMove => _isMove;

    private void Start()
    {
        _playerTransform = SystemManager.Instance.GetSystem<EnemySystem>().PlayerTransform;
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _movementSpeed = SystemManager.Instance.GetSystem<GlobalDataSystem>().EnemyTable.MovementSpd;
        _isMove = false;
    }

    private void FixedUpdate()
    {
        if (_isMove)
            return;
        EnemyMove();
    }


    private void EnemyMove()
    {
        float enemyAngleDegree = QT.Util.Math.GetDegree(transform.position,_playerTransform.position);
        float angle = enemyAngleDegree * Mathf.Deg2Rad;

        _rigidbody2D.velocity = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * _movementSpeed * Time.fixedDeltaTime;
    }
    
}
