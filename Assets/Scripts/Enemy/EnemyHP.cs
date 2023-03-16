using System;
using System.Collections;
using System.Collections.Generic;
using QT.Core;
using QT.Data;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
public class EnemyHP : MonoBehaviour
{
    [SerializeField] private RectTransform _hpRectTransform;
    private Image _hpImage;
    private int _hpMax;
    private int _currentHp;
    [HideInInspector] public Rigidbody2D _rigidbody2D;
    private float _stunDelay;
    private Coroutine _deadCoroutine;
    private EnemyController _enemyController;
    private bool _hpZeroDelay;
    private void Awake()
    {
        _hpImage = _hpRectTransform.GetComponentsInChildren<Image>()[1];
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _deadCoroutine = null;
        _enemyController = GetComponent<EnemyController>();
        _hpZeroDelay = false;
    }

    private void Start()
    {
        GlobalDataSystem globalDataSystem = SystemManager.Instance.GetSystem<GlobalDataSystem>();
        _hpMax = globalDataSystem.EnemyTable.HPMax;
        _stunDelay = globalDataSystem.GlobalData.DeadAfterStunTime;
        _currentHp = _hpMax;
    }


    private void FixedUpdate()
    {
        _rigidbody2D.velocity = Vector3.zero;
    }

    public void HitDamage(int damage)
    {
        if (_currentHp <= 0)
            return;
        _currentHp -= damage;
        _hpImage.fillAmount = QT.Util.Math.floatNormalization(_currentHp, _hpMax, 0);
        if (_currentHp <= 0)
        {
            StartCoroutine(QT.Util.UnityUtil.WaitForFunc(() => { _hpZeroDelay = true; }, 0.5f));
            _deadCoroutine = StartCoroutine(QT.Util.UnityUtil.WaitForFunc(EnemyDead, _stunDelay));
            _enemyController.EnemyStunSet();
        }
    }

    public bool IsStun()
    {
        if (!_hpZeroDelay)
            return false;
        StopCoroutine(_deadCoroutine);
        Destroy(_hpRectTransform.gameObject);
        _deadCoroutine = null;
        _enemyController.EnemyToBall();
        return true;
    }

    private void EnemyDead()
    {
        Destroy(gameObject);
    }
}
