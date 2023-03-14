using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using QT.Core;
using QT.Data;
public class BallCollsion : MonoBehaviour
{
    private Rigidbody2D _rigidbody2D;
    private float _batBounce; //��Ʈ ƨ�� ���ط� ����ġ
    private float _ballBounce; // �� ƨ�� ���ط� ����ġ
    private int _bounceMinDmg; // ƨ�� ���ط� �ּҰ�
    private void Awake()
    {
        GlobalDataSystem dataSystem = SystemManager.Instance.GetSystem<GlobalDataSystem>();
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _batBounce = dataSystem.BatTable.BounceSpdDmgPer;
        _ballBounce = dataSystem.BallTable.BallBounceSpdDmgPer;
        _bounceMinDmg = dataSystem.GlobalData.BounceMinDmg;
        Debug.Log(_rigidbody2D.gameObject.name);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            EnemyHP enemyHP = collision.GetComponent<EnemyHP>();
            Debug.Log(_rigidbody2D.velocity.magnitude);
            int damage;
            if (gameObject.layer == LayerMask.NameToLayer("Ball"))
            {
                damage = Mathf.RoundToInt(_rigidbody2D.velocity.magnitude);
            }
            else
            {
                damage = Mathf.RoundToInt(_rigidbody2D.velocity.magnitude * _ballBounce * _batBounce);
            }
            damage = damage < _bounceMinDmg ? _bounceMinDmg : damage;
            Debug.Log("Dmg : " + damage.ToString());
            enemyHP.HitDamage(damage);
            
            
            //ContactPoint2D[] contacts = new ContactPoint2D[1];
            //collision.GetContacts(contacts);
            //Vector2 contactPoint = contacts[0].point;
            //Vector3 contactPointInWorld = new Vector3(contactPoint.x, contactPoint.y, 0f);
        } // �ӵ��� ���� ������� �̱���

    }
}
