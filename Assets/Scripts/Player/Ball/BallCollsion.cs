using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QT.Core;
using QT.Core.Player;
public class BallCollsion : MonoBehaviour
{
    private int _ballDmg;
    private void Awake()
    {
        _ballDmg = SystemManager.Instance.GetSystem<PlayerSystem>().BallTable.BallRigidDmg;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("sdagf");
        if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            EnemyHP enemyHP = collision.GetComponent<EnemyHP>();
            enemyHP.HitDamage(_ballDmg);
        }

    }

    //private void 
}
