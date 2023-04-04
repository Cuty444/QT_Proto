using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using QT.Core;
using QT.Core.Data;
using QT.Core.Player;
using QT.Enemy;
using QT.UI;

namespace QT.Player
{
    public class PlayerCollision : MonoBehaviour
    {
        #region StartData_Declaration

        private PlayerSystem _playerSystem;
        private Image _hpImage;
        private int _hpMax;
        private float _mercyInvincibleTime;
        private float _dodgeInvincibleTime;

        #endregion

        #region Global_Declaration

        private Coroutine _InvincibleCoroutine;
        private int _currentHp;
        private bool _isMercy;
        
        #endregion

        private void Start()
        {
            GlobalDataSystem globalDataSystem = SystemManager.Instance.GetSystem<GlobalDataSystem>();
            CharacterTable characterTable = globalDataSystem.CharacterTable;
            _playerSystem = SystemManager.Instance.GetSystem<PlayerSystem>();
            _playerSystem.DodgeEvent.AddListener(DodgeInvincible);
            _hpMax = characterTable.HPMax;
            _currentHp = _hpMax;
            _mercyInvincibleTime = characterTable.MercyInvincibleTime;
            _dodgeInvincibleTime = characterTable.DodgeInvincibleTime;
            _hpImage = UIManager.Instance.GetUIPanel<PlayerHPCanvas>().PlayerHPImage;
            _hpImage.fillAmount = QT.Util.Math.floatNormalization(_currentHp, _hpMax, 0);
            _isMercy = false;
            _InvincibleCoroutine = null;
        }

        private void OnCollisionStay2D(Collision2D collision)
        {
            if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
            {
                _playerSystem.PlayerCollisionEnemyEvent.Invoke();
                if (_isMercy)
                    return;
                HitDamage(collision.gameObject.GetComponent<EnemyAttack>().GetDamage());
                _isMercy = true;
                StartCoroutine(QT.Util.UnityUtil.WaitForFunc(() => { _isMercy = false; }, _mercyInvincibleTime));
            }
        }

        public void HitDamage(int damage)
        {
            if (_currentHp <= 0)
                return;
            _currentHp -= damage;
            Debug.Log("Player HP : " + _currentHp);
            _hpImage.fillAmount = QT.Util.Math.floatNormalization(_currentHp, _hpMax, 0);
            if (_currentHp <= 0)
            {
                SceneManager.LoadScene(0);
            }
        }

        private void DodgeInvincible(bool isInvicible)
        {
            if (!isInvicible)
                return;

            if (_InvincibleCoroutine != null)
            {
                StopCoroutine(_InvincibleCoroutine);
            }
            _isMercy = true;
            _InvincibleCoroutine = StartCoroutine(QT.Util.UnityUtil.WaitForFunc(() => 
            { 
                _isMercy = false;
                _InvincibleCoroutine = null;
            }, _dodgeInvincibleTime));

        }
    }
}