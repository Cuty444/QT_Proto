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
        private readonly int AnimationRigidHash = Animator.StringToHash("PlayerRigid");
        
        
        private GlobalDataSystem _globalDataSystem;
        private PlayerHPCanvas _playerHpCanvas;

        private float _currentThrowCoolTime;
        private float _currentSwingCoolTime;
        private float _currentChargingTime;
        private float _currentDodgeCoolTime;

        private float _startDodgeTime;
        private float _startInvincibleTime;
        
        private int _currentBallStack;


        public PlayerGlobalState(IFSMEntity owner) : base(owner)
        {
            _playerHpCanvas = SystemManager.Instance.UIManager.GetUIPanel<PlayerHPCanvas>();
            _playerHpCanvas.gameObject.SetActive(true);
            _playerHpCanvas.SetHp(_ownerEntity.GetStat(PlayerStats.HP) as Status);

            _globalDataSystem = SystemManager.Instance.GetSystem<GlobalDataSystem>();
            
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
            
            _ownerEntity.EyeTransform.rotation = Quaternion.Euler(0, 0, angle + 270);

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
            _currentDodgeCoolTime += Time.deltaTime;
            
            float dodgeCoolTime = _ownerEntity.GetStat(PlayerStats.DodgeCooldown);
            _playerHpCanvas.SetDodgeCoolTime(Util.Math.Remap(_currentDodgeCoolTime,dodgeCoolTime,0f));
        }

        private void ThrowCoolTime()
        {
            _currentThrowCoolTime += Time.deltaTime;
        }
        
        #endregion

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

            _ownerEntity.Animator.SetTrigger(AnimationRigidHash);
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