using System;
using System.Collections;
using System.Collections.Generic;
using QT.Core;
using QT.Core.Player;
using QT.Data;
using UnityEditor.Playables;
using UnityEngine;

public class BatSwing : MonoBehaviour
{
    private PlayerSystem _playerSystem;
    private GlobalDataSystem _globalDataSystem;
    private float _shootSpd;
    private void Start()
    {
        _playerSystem = SystemManager.Instance.GetSystem<PlayerSystem>();
        _globalDataSystem = SystemManager.Instance.GetSystem<GlobalDataSystem>();
        _playerSystem.ChargeAtkShootEvent.AddListener(SetShootSpeed);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (enabled == false)
            return;
        if(collision.gameObject.layer == LayerMask.NameToLayer("Ball") || collision.gameObject.layer == LayerMask.NameToLayer("BallHit"))
        {
            Vector2 _attackDirection = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            float bulletAngleDegree = QT.Util.Math.GetDegree(transform.position, _attackDirection);
            BallMove Ball = collision.GetComponent<BallMove>();
            Ball.transform.rotation = Quaternion.Euler(0, 0, bulletAngleDegree);
            Ball.BulletSpeed = _shootSpd;
            Ball.ForceChange();
            Ball.IsShot = true;
            if(QT.Util.Flags.ChargeAtkPierce.None == _playerSystem.ChargeAtkPierce)
            {
                Ball.gameObject.layer = LayerMask.NameToLayer("Ball");
                Debug.Log("Ball");
            }
            else if(_globalDataSystem.BatTable.ChargeAtkPierce.HasFlag(_playerSystem.ChargeAtkPierce))
            {
                Ball.gameObject.layer = LayerMask.NameToLayer("BallHit");
                Debug.Log("BallHit");
            }
            else
            {
                Ball.gameObject.layer = LayerMask.NameToLayer("Ball");
                Debug.Log("Ball");
            }
            _playerSystem.BatSwingBallHitEvent.Invoke();
        }
        else if(collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {

        }
    }

    private void SetShootSpeed(float speed)
    {
        _shootSpd = speed;
    }
}
