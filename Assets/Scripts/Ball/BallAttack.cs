using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QT.Core;
using QT.Core.Data;

namespace QT.Ball
{

    public class BallAttack : MonoBehaviour
    {
        #region StartData_Declaration

        private float _batBounce; //배트 튕김 피해량 가중치
        private float _ballBounce; // 볼 튕김 피해량 가중치
        private int _bounceMinDmg; // 튕김 피해량 최소값

        #endregion

        #region Global_Declaration

        private Rigidbody2D _rigidbody2D;

        #endregion

        private void Awake()
        {
            GlobalDataSystem dataSystem = SystemManager.Instance.GetSystem<GlobalDataSystem>();
            _rigidbody2D = GetComponent<Rigidbody2D>();
            _batBounce = dataSystem.BatTable.BounceSpdDmgPer;
            _ballBounce = dataSystem.BallTable.BallBounceSpdDmgPer;
            _bounceMinDmg = dataSystem.GlobalData.BounceMinDmg;
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

            return damage < _bounceMinDmg ? 0 : damage;
        }
    }
}