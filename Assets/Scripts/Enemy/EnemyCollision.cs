using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QT.Ball;

namespace QT.Enemy
{

    public class EnemyCollision : MonoBehaviour
    {
        #region Inspector_Definition

        [SerializeField] private EnemyHP _enemyHp;
        
        #endregion
        private void OnCollisionEnter2D(Collision2D col)
        {
            if (col.gameObject.layer == LayerMask.NameToLayer("Ball"))
            {
                _enemyHp.HitDamage(col.gameObject.GetComponent<BallAttack>().GetBallHitDamage());
            }
        }
    }
}