using UnityEngine;
using System.Collections;
using QT.Core;
using QT.Core.Data;
using QT.Core.Input;
using QT.UI;
using Unity.VisualScripting;
using UnityEngine.UI;

namespace QT.Player
{
    [FSMState((int)Player.States.Global, false)]
    public class PlayerGlobalState : FSMState<Player>
    {
        private GlobalDataSystem _globalDataSystem;
        private InputSystem _inputSystem;
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
            _inputSystem = SystemManager.Instance.GetSystem<InputSystem>();
            _inputSystem.OnKeyDownAttackEvent.AddListener(KeyDownAttack);
            _inputSystem.OnKeyUpAttackEvent.AddListener(KeyUpAttack);
            _inputSystem.OnKeyEThrowEvent.AddListener(KeyEThrow);
            _inputSystem.OnKeyMoveEvent.AddListener(MoveDirection);
            _inputSystem.OnKeySpaceDodgeEvent.AddListener(KeySpaceDodge);
            _playerHpCanvas = SystemManager.Instance.UIManager.GetUIPanel<PlayerHPCanvas>();
            _playerHpCanvas.gameObject.SetActive(true);
            _dodgeCoolBackgroundImage = _playerHpCanvas.PlayerDodgeCoolBackgroundImage;
            _dodgeCoolBarImage = _playerHpCanvas.PlayerDodgeCoolBarImage;
            _playerHpCanvas.SetHp(_ownerEntity.GetStatus(PlayerStatus.HP));

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
        }

        public override void InitializeState()
        {
        }

        public override void UpdateState()
        {
            if (_ownerEntity.CurrentStateIndex == (int) Player.States.Dead)
                return;
            ThrowCoolTime();
            AttackCoolTime();
            DodgeCoolTime();
            if (_ownerEntity.CurrentStateIndex == (int)Player.States.Dodge)
                return;
            SetDirection();
            _ownerEntity.AngleAnimation();
        }
        
        
        protected virtual void MoveDirection(Vector2 direction)
        {
            if (_ownerEntity.CurrentStateIndex == (int)Player.States.Dodge)
                return;
            _ownerEntity.SetMoveDirection(direction.normalized);
            if (_ownerEntity.MoveDirection == Vector2.zero && _isMouseDownCheck == false)
            {
                _ownerEntity.ChangeState(Player.States.Idle);
            }

            _ownerEntity.SetMoveCheck(_ownerEntity.MoveDirection == Vector2.zero);
        }
        
        #region AttackFunc

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
        
        private void SetDirection()
        {
            _ownerEntity.MouseValueSet();
            var angle = Util.Math.GetDegree(_ownerEntity.transform.position, _ownerEntity.MousePos);
            _ownerEntity.EyeTransform.rotation = Quaternion.Euler(0, 0, angle);
        }

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

        private void KeyDownAttack()
        {
            if (_ownerEntity.CurrentStateIndex == (int) Player.States.Dodge)
            {
                return;
            }
            if (_currentSwingCoolTime >_ownerEntity.GetStat(PlayerStats.SwingCooldown))
            {
                _isMouseDownCheck = true;
                _ownerEntity.SwingAreaMeshRenderer.enabled = true;
                _ownerEntity.ChangeState(Player.States.Swing);
            }
        }

        private void KeyUpAttack()
        {
            if (_ownerEntity.CurrentStateIndex == (int)Player.States.Dodge)
                return;
            if (!_isMouseDownCheck)
            {
                return;
            }
            _isMouseDownCheck = false;
            _ownerEntity.SwingAreaMeshRenderer.enabled = false;
            _currentSwingCoolTime = 0f;
        }


        private void KeySpaceDodge()
        {
            if (_ownerEntity.CurrentStateIndex == (int)Player.States.Dodge)
                return;
            if (_currentDodgeCoolTime < _ownerEntity.GetStat(PlayerStats.DodgeCooldown))
                return;
            if (_ownerEntity.GetRigidTrigger())
                return;
            if (_ownerEntity.MoveDirection == Vector2.zero)
                return;
            _ownerEntity.SetBefereDodgeDirecton();
            _ownerEntity.ChangeState(Player.States.Dodge);
            _currentDodgeCoolTime = 0f;
            _startDodgeTime = Time.time;
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
            _ownerEntity.ChangeState(Player.States.Rigid);
            _ownerEntity.PlayerHitEffectPlay();

            var hp = _ownerEntity.GetStatus(PlayerStatus.HP);
            
            hp.AddStatus(-damage);
            _playerHpCanvas.CurrentHpImageChange(hp);
            
            if (hp <= 0)
            {
                PlayerDead();
            }
        }

        private void PlayerDead()
        {
            _inputSystem.OnKeyDownAttackEvent.RemoveAllListeners();
            _inputSystem.OnKeyUpAttackEvent.RemoveAllListeners();
            _inputSystem.OnKeyEThrowEvent.RemoveAllListeners();
            _inputSystem.OnKeyMoveEvent.RemoveAllListeners();
            _inputSystem.OnKeySpaceDodgeEvent.RemoveAllListeners();
            SystemManager.Instance.PlayerManager.PlayerThrowProjectileReleased.RemoveAllListeners();
            _ownerEntity.OnDamageEvent.RemoveAllListeners();
            _ownerEntity.ChangeState(Player.States.Dead);
        }
        
    }

}