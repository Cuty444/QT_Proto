using UnityEngine;
using System.Collections;
using QT.Core;
using QT.Core.Data;
using QT.UI;
using Unity.VisualScripting;
using UnityEngine.UI;

namespace QT.InGame
{
    [FSMState((int)Player.States.Global, false)]
    public class PlayerGlobalState : FSMState<Player>
    {
        private readonly int AnimationMouseRotateHash = Animator.StringToHash("MouseRotate");
        
        private GlobalDataSystem _globalDataSystem;
        private PlayerHPCanvas _playerHpCanvas;

        private float _currentThrowCoolTime;
        private float _currentSwingCoolTime;
        private float _currentChargingTime;
        private float _currentDodgeCoolTime;

        private float _startDodgeTime;
        private float _startInvincibleTime;
        
        private int _currentBallStack;

        private Image _dodgeCoolBackgroundImage;
        private Image _dodgeCoolBarImage;
        
        private bool _isMouseDownCheck;
        
        public PlayerGlobalState(IFSMEntity owner) : base(owner)
        {
            _playerHpCanvas = SystemManager.Instance.UIManager.GetUIPanel<PlayerHPCanvas>();
            _playerHpCanvas.gameObject.SetActive(true);
            _dodgeCoolBackgroundImage = _playerHpCanvas.PlayerDodgeCoolBackgroundImage;
            _dodgeCoolBarImage = _playerHpCanvas.PlayerDodgeCoolBarImage;
            _playerHpCanvas.SetHp(_ownerEntity.GetStat(PlayerStats.HP) as Status);

            _globalDataSystem = SystemManager.Instance.GetSystem<GlobalDataSystem>();
            SystemManager.Instance.PlayerManager.PlayerThrowProjectileReleased.AddListener(() =>
            {
                _playerHpCanvas.ThrowProjectileGauge(true);
                _currentBallStack++;
            });
            _currentBallStack = (int)_ownerEntity.GetStat(PlayerStats.BallStackMax);
            _currentSwingCoolTime = _ownerEntity.GetStat(PlayerStats.SwingCooldown);
            _currentDodgeCoolTime = _ownerEntity.GetStat(PlayerStats.DodgeCooldown);
            _currentThrowCoolTime = _ownerEntity.GetStat(PlayerStats.ThrowCooldown);
            _ownerEntity.SetBatActive(false);
            _ownerEntity.OnDamageEvent.AddListener(OnDamage);
            
            _ownerEntity.OnLook.AddListener(OnLook);
        }


        public override void UpdateState()
        {
            if (_ownerEntity.CurrentStateIndex == (int) Player.States.Dead)
                return;
            ThrowCoolTime();
            AttackCoolTime();
            DodgeCoolTime();
        }
        
        private void OnLook(Vector2 lookDir)
        {
            var angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg - 90;
            float flip = 180;
            
            if (angle < 0)
            {
                angle += 360;
            }
            
            _ownerEntity.EyeTransform.rotation = Quaternion.Euler(0, 0, angle);

            if (angle > 180)
            {
                angle = 360 - angle;
                flip = 0;
            }
            
            _ownerEntity.Animator.SetFloat(AnimationMouseRotateHash, angle / 180 * 4);
            _ownerEntity.Animator.transform.rotation = Quaternion.Euler(0f, flip, 0f);
        }
        
        
        #region CoolDown

        private void AttackCoolTime()
        {
            _currentSwingCoolTime += Time.deltaTime;
        }

        private void DodgeCoolTime()
        {
            float dodgeCoolTime = _ownerEntity.GetStat(PlayerStats.DodgeCooldown);
            
            _currentDodgeCoolTime += Time.deltaTime;
            _dodgeCoolBarImage.fillAmount = Util.Math.Remap(_currentDodgeCoolTime,dodgeCoolTime,0f);
            _dodgeCoolBackgroundImage.gameObject.SetActive(_currentDodgeCoolTime < dodgeCoolTime);
        }

        private void ThrowCoolTime()
        {
            _currentThrowCoolTime += Time.deltaTime;
        }
        
        #endregion

        private void KeyEThrow()
        {
            if (_globalDataSystem.GlobalData.IsPlayerParrying)
            {
                return;
            }
            if (_ownerEntity.CurrentStateIndex == (int) Player.States.Dodge)
            {
                return;
            }
            if (_currentBallStack == 0)
            {
                return;
            }

            if (_currentThrowCoolTime < _ownerEntity.GetStat(PlayerStats.ThrowCooldown))
            {
                return;
            }

            _ownerEntity.AttackBallInstate();
            _currentBallStack--;
            _currentThrowCoolTime = 0f;
            _ownerEntity.SetThrowAnimation();
            if(_currentBallStack == 0)
                _playerHpCanvas.ThrowProjectileGauge(false);
        }

        

        private void OnDamage(Vector2 dir, float damage)
        {
            if (Time.time - _startInvincibleTime < _ownerEntity.GetStat(PlayerStats.MercyInvincibleTime))
            { 
                return;
            }

            if (Time.time - _startDodgeTime < _ownerEntity.GetStat(PlayerStats.DodgeInvincibleTime))
            {
                return;
            }
            _startInvincibleTime = Time.time;
            _ownerEntity.ChangeState(Player.States.Rigid);
            _ownerEntity.PlayerHitEffectPlay();

            var hp = _ownerEntity.GetStat(PlayerStats.HP) as Status;
            
            hp.AddStatus(-damage);
            _playerHpCanvas.CurrentHpImageChange(hp);
            
            if (hp <= 0)
            {
                PlayerDead();
            }
        }

        private void PlayerDead()
        {
            SystemManager.Instance.PlayerManager.PlayerThrowProjectileReleased.RemoveAllListeners();
            _ownerEntity.OnDamageEvent.RemoveAllListeners();
            _ownerEntity.ChangeState(Player.States.Dead);
        }
        
    }

}