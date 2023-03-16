using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using QT.Core;
using QT.Data;

public class BallCollsion : MonoBehaviour
{
    public Rigidbody2D _rigidbody2D;
    private float _batBounce; //¹èÆ® Æ¨±è ÇÇÇØ·® °¡ÁßÄ¡
    private float _ballBounce; // º¼ Æ¨±è ÇÇÇØ·® °¡ÁßÄ¡
    private int _bounceMinDmg; // Æ¨±è ÇÇÇØ·® ÃÖ¼Ò°ª
    private bool _isBatNoHit; // ¹èÆ® ÁßÃ¸ Ãæµ¹ ¹æÁö
    public bool IsBatNoHit => _isBatNoHit;
    private void Awake()
    {
        GlobalDataSystem dataSystem = SystemManager.Instance.GetSystem<GlobalDataSystem>();
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _batBounce = dataSystem.BatTable.BounceSpdDmgPer;
        _ballBounce = dataSystem.BallTable.BallBounceSpdDmgPer;
        _bounceMinDmg = dataSystem.GlobalData.BounceMinDmg;
        _isBatNoHit = false;
    }

    public int GetBallHitDamage()
    {
        int damage;
        if (gameObject.layer == LayerMask.NameToLayer("Ball"))
        {
            damage = Mathf.RoundToInt(_rigidbody2D.velocity.magnitude);
        }
        else
        {
            damage = Mathf.RoundToInt(_rigidbody2D.velocity.magnitude * _ballBounce * _batBounce);
        }
        return damage < _bounceMinDmg ? _bounceMinDmg : damage;
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (_isBatNoHit)
            return;
        if (col.gameObject.layer == LayerMask.NameToLayer("Bat"))
        {
            _isBatNoHit = true;
            StartCoroutine(QT.Util.UnityUtil.WaitForFunc(() => _isBatNoHit = false, 0.1f));
        }
    }
}
