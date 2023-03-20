using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QT.Ball;

namespace QT.Enemy
{
    public class EnemyTrigger : MonoBehaviour
    {
        #region Inspector_Definition

        [SerializeField] private EnemyHP _enemyHp;

        #endregion

        private void OnTriggerEnter2D(Collider2D col)
        {
            if (col.gameObject.layer == LayerMask.NameToLayer("BallHit"))
            {
                _enemyHp.HitDamage(col.GetComponent<BallAttack>().GetBallHitDamage());
            }
        }
    }
}