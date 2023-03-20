using System;
using System.Collections;
using System.Collections.Generic;
using QT.Core;
using QT.Core.Data;
using UnityEngine;
using UnityEngine.UI;

namespace QT.Enemy
{
    public class EnemyHP : MonoBehaviour
    {
        #region Inspector_Definition

        [SerializeField] private RectTransform _hpRectTransform;

        #endregion

        #region StartData_Declaration

        private float _stunDelay;
        private int _hpMax;

        #endregion

        #region Global_Declaration

        private EnemyController _enemyController;
        private Image _hpImage;
        private Coroutine _deadCoroutine;
        private int _currentHp;

        #endregion


        private void Awake()
        {
            _hpImage = _hpRectTransform.GetComponentsInChildren<Image>()[1];
            _deadCoroutine = null;
            _enemyController = GetComponent<EnemyController>();
        }

        private void Start()
        {
            GlobalDataSystem globalDataSystem = SystemManager.Instance.GetSystem<GlobalDataSystem>();
            _hpMax = globalDataSystem.EnemyTable.HPMax;
            _stunDelay = globalDataSystem.GlobalData.DeadAfterStunTime;
            _currentHp = _hpMax;
        }

        public void HitDamage(int damage)
        {
            if (_currentHp <= 0)
                return;
            _currentHp -= damage;
            _hpImage.fillAmount = QT.Util.Math.floatNormalization(_currentHp, _hpMax, 0);
            if (_currentHp <= 0)
            {
                _deadCoroutine = StartCoroutine(QT.Util.UnityUtil.WaitForFunc(EnemyDead, _stunDelay));
                _enemyController.EnemyStunSet();
            }
        }

        public void IsStun()
        {
            StopCoroutine(_deadCoroutine);
            Destroy(_hpRectTransform.gameObject);
            _deadCoroutine = null;
            _enemyController.EnemyToBall();
        }

        private void EnemyDead()
        {
            Destroy(gameObject);
        }
    }
}