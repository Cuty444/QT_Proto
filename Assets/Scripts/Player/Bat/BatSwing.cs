using System;
using System.Collections;
using System.Collections.Generic;
using QT.Core;
using QT.Core.Player;
using UnityEngine;

public class BatSwing : MonoBehaviour
{
    private PlayerSystem _playerSystem;
    private void Start()
    {
        _playerSystem = SystemManager.Instance.GetSystem<PlayerSystem>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (enabled == false)
            return;
        if(collision.gameObject.layer == LayerMask.NameToLayer("Ball") || collision.gameObject.layer == LayerMask.NameToLayer("BallHit"))
        {
            Vector2 _attackDirection = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            float bulletAngleRadian = Mathf.Atan2(_attackDirection.y - transform.position.y, _attackDirection.x - transform.position.x);
            float bulletAngleDegree = 180 / Mathf.PI * bulletAngleRadian;
            BallMove Ball = collision.GetComponent<BallMove>();
            Ball.transform.rotation = Quaternion.Euler(0, 0, bulletAngleDegree);
            Ball.ForceChange();
            Ball.IsShot = true;
            Ball.gameObject.layer = LayerMask.NameToLayer("BallHit");
            _playerSystem.BatSwingBallHitEvent.Invoke();
        }
    }
}
