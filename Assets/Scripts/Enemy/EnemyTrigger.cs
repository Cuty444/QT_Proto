using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTrigger : MonoBehaviour
{
    private EnemyHP _enemyHp;

    private void Start()
    {
        _enemyHp = GetComponentInParent<EnemyHP>();
    }

    private void OnTriggerEnter2D(Collider2D col)
    { 
        if (col.gameObject.layer == LayerMask.NameToLayer("BallHit"))
        {
            _enemyHp.HitDamage(col.GetComponent<BallCollsion>().GetBallHitDamage());
        }
        
    }
}
