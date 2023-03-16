using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCollision : MonoBehaviour
{
    private EnemyHP _enemyHp;

    private void Start()
    {
        _enemyHp = GetComponent<EnemyHP>();
    }
    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.layer == LayerMask.NameToLayer("Ball"))
        {
            _enemyHp.HitDamage(col.gameObject.GetComponent<BallCollsion>().GetBallHitDamage());
        }
    }
}
