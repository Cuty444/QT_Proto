using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QT.Core;
using QT.Data;
public class BallCollsion : MonoBehaviour
{
    private int _ballDmg;
    private void Awake()
    {
        GlobalDataSystem dataSystem = SystemManager.Instance.GetSystem<GlobalDataSystem>();
        _ballDmg = dataSystem.BallTable.BallRigidDmg;
        GetComponent<CircleCollider2D>().radius = dataSystem.BallTable.BallRad;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            EnemyHP enemyHP = collision.GetComponent<EnemyHP>();
            enemyHP.HitDamage(_ballDmg);
        }

    }

}
