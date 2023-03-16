using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using QT.Core;
using QT.Data;
using QT.UI;

public class PlayerCollision : MonoBehaviour
{
    private int _hpMax;
    private float _mercyInvincibleTime;
    private int _currentHp;
    private bool _isMercy;

    private EnemyTable _enemyTable;
    private Coroutine _coroutine;
    private Image _hpImage;
    private void Start()
    {
        GlobalDataSystem globalDataSystem = SystemManager.Instance.GetSystem<GlobalDataSystem>();
        _enemyTable = globalDataSystem.EnemyTable;
        CharacterTable characterTable = globalDataSystem.CharacterTable;
        _hpMax = characterTable.HPMax;
        _currentHp = _hpMax;
        _mercyInvincibleTime = characterTable.MercyInvincibleTime;
        _hpImage = UIManager.Instance.GetUIPanel<PlayerHPCanvas>().PlayerHPImage;
        _hpImage.fillAmount = QT.Util.Math.floatNormalization(_currentHp, _hpMax, 0);
        _isMercy = false;
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (_isMercy)
            return;
        if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            HitDamage(_enemyTable.ATKDmg);
            _isMercy = true;
            StartCoroutine(QT.Util.UnityUtil.WaitForFunc(() => { _isMercy = false; },_mercyInvincibleTime));
        }
    }
    
    public void HitDamage(int damage)
    {
        if (_currentHp <= 0)
            return;
        _currentHp -= damage;
        Debug.Log("Player HP : " + _currentHp);
        _hpImage.fillAmount = QT.Util.Math.floatNormalization(_currentHp, _hpMax, 0);
        if(_currentHp <= 0)
        {
            SceneManager.LoadScene(0);
        }
    }
}
