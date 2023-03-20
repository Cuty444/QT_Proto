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

        private float _batBounce; //��Ʈ ƨ�� ���ط� ����ġ
        private float _ballBounce; // �� ƨ�� ���ط� ����ġ
        private int _bounceMinDmg; // ƨ�� ���ط� �ּҰ�

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

            return damage < _bounceMinDmg ? _bounceMinDmg : damage;
        }
    }
}