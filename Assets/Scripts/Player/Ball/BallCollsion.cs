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
        GetComponent<CircleCollider2D>().radius = dataSystem.BallTable.BallHitBoxRad;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            EnemyHP enemyHP = collision.GetComponent<EnemyHP>();
            enemyHP.HitDamage(_ballDmg);
            //ContactPoint2D[] contacts = new ContactPoint2D[1];
            //collision.GetContacts(contacts);
            //Vector2 contactPoint = contacts[0].point;
            //Vector3 contactPointInWorld = new Vector3(contactPoint.x, contactPoint.y, 0f);
        }

    }

}
