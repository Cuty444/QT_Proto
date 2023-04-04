using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QT.Core;
using QT.Core.Player;
using QT.Core.Data;
using QT.Data;
using QT.Enemy;
using QT.Ball;


namespace QT.Player.Bat
{
    public class BatSwing : MonoBehaviour
    {
        #region StartData_Declaration

        private PlayerSystem _playerSystem;
        private GlobalDataSystem _globalDataSystem;

        #endregion

        #region Global_Declaration

        private List<GameObject> _hitObjectList = new List<GameObject>();
        private float _shootSpd;
        private int _swingRigidDmg;

        #endregion


        private void Start()
        {
            _playerSystem = SystemManager.Instance.GetSystem<PlayerSystem>();
            _globalDataSystem = SystemManager.Instance.GetSystem<GlobalDataSystem>();
            _playerSystem.ChargeAtkShootEvent.AddListener(SetShootSpeed);
            _playerSystem.BatSwingRigidHitEvent.AddListener(SetSwingRigidDamage);
            _playerSystem.BatSwingEndEvent.AddListener(SwingEndObjectClear);
            this.enabled = false;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (enabled == false)
                return;
            for (int i = 0; i < _hitObjectList.Count; i++)
            {
                if (collision.gameObject == _hitObjectList[i])
                    return;
            }

            if (collision.gameObject.layer == LayerMask.NameToLayer("Ball") ||
                collision.gameObject.layer == LayerMask.NameToLayer("BallHit"))
            {
                _hitObjectList.Add(collision.gameObject);
                EnemyHP enemyHP = collision.GetComponent<EnemyHP>();
                if (enemyHP != null)
                {
                    enemyHP.IsStun();
                }

                Vector2 _attackDirection = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                float bulletAngleDegree = QT.Util.Math.GetDegree(transform.position, _attackDirection);
                BallMove Ball = collision.GetComponent<BallMove>();
                Ball.transform.rotation = Quaternion.Euler(0, 0, bulletAngleDegree);
                Ball.BulletSpeed = _shootSpd;
                Ball.ForceChange();
                Ball.SwingBallHit();
                if (ChargeAtkPierce.None == _playerSystem.ChargeAtkPierce)
                {
                    Ball.gameObject.layer = LayerMask.NameToLayer("Ball");
                }
                else if (_globalDataSystem.BatTable.ChargeAtkPierce.HasFlag(_playerSystem.ChargeAtkPierce))
                {
                    Ball.gameObject.layer = LayerMask.NameToLayer("BallHit");
                }
                else
                {
                    Ball.gameObject.layer = LayerMask.NameToLayer("Ball");
                }
            }
            else if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
            {
                _hitObjectList.Add(collision.gameObject);
                EnemyHP enemyHP = collision.GetComponent<EnemyHP>();
                enemyHP.HitDamage(_swingRigidDmg);
            }
        }

        private void SetShootSpeed(float speed)
        {
            _shootSpd = speed;
        }

        private void SetSwingRigidDamage(int damage)
        {
            _swingRigidDmg = damage;
        }

        private void SwingEndObjectClear()
        {
            _hitObjectList.Clear();
        }
    }
}