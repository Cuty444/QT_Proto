using UnityEngine;
using System.Collections;
using QT.Core;
using QT.Core.Input;
using QT.UI;
using UnityEngine.UI;

namespace QT.Player
{
    [FSMState((int)Player.States.Global, false)]
    public class PlayerGlobalState : FSMState<Player>
    {
        private InputSystem _inputSystem;
        
        private float _currentSwingCoolTime;
        private float _currentChargingTime;

        private int _currentBallStack;


        private Image _throwProjectileUI;
        
        private bool _isMouseDownCheck;
        
        public PlayerGlobalState(IFSMEntity owner) : base(owner)
        {
            _inputSystem = SystemManager.Instance.GetSystem<InputSystem>();
            _inputSystem.OnKeyDownAttackEvent.AddListener(KeyDownAttack);
            _inputSystem.OnKeyUpAttackEvent.AddListener(KeyUpAttack);
            _inputSystem.OnKeyEThrowEvent.AddListener(KeyEThrow);
            _inputSystem.OnKeyMoveEvent.AddListener(MoveDirection);
            SystemManager.Instance.UIManager.GetUIPanel<PlayerHPCanvas>().gameObject.SetActive(true);
            _throwProjectileUI = SystemManager.Instance.UIManager.GetUIPanel<PlayerHPCanvas>().PlayerBallStackImage;
            _inputSystem.OnKeySpaceDodgeEvent.AddListener(KeySpaceDodge);
            SystemManager.Instance.PlayerManager.PlayerThrowProjectileReleased.AddListener(() =>
            {
                _throwProjectileUI.enabled = true;
                _currentBallStack++;
            });
            _currentBallStack = (int)_ownerEntity.BallStackMax.Value;
            _currentSwingCoolTime = _ownerEntity.SwingCooldown.Value;
            _ownerEntity.SetBatActive(false);
        }

        public override void InitializeState()
        {
        }

        public override void UpdateState()
        {
            SetDirection();
            AttackCoolTime();
            _ownerEntity.AngleAnimation();
        }

        public override void FixedUpdateState()
        {
            //if (_currentSwingCoolTime > _ownerEntity.SwingCooldown)
            //{
            //    _isUpDown = _ownerEntity.PlayerSwingAngle();
            //}

        }
        
        protected virtual void MoveDirection(Vector2 direction)
        {
            _ownerEntity.SetMoveDirection(direction.normalized);
            //if (_ownerEntity.MoveDirection == Vector2.zero)
            //{
            //    _ownerEntity.ChangeState(Player.States.Idle);
            //}

            _ownerEntity.SetMoveCheck(_ownerEntity.MoveDirection == Vector2.zero);
        }
        
        #region AttackFunc

        private void AttackCoolTime()
        {
            _currentSwingCoolTime += Time.deltaTime;
            if (_isMouseDownCheck)
            {
                _currentChargingTime += Time.deltaTime;
                //_ownerEntity.PlayerCurrentChargingTimeEvent.Invoke(_currentChargingTime);
                if (_currentChargingTime > _ownerEntity.ChargeTimes[0].Value)
                {
                    //if (!_chargingBarBackground.gameObject.activeSelf)
                    //{
                    //    _chargingBarBackground.gameObject.SetActive(true);
                    //}
                    //_chargingBarImage.fillAmount = QT.Util.Math.Remap(_currentChargingTime,
                    //    _chargingMaxTimes[_chargingMaxTimes.Length - 1], _chargingMaxTimes[0]);
                }
            }
        }
        
        private void SetDirection()
        {
            _ownerEntity.MouseValueSet();
            var angle = Util.Math.GetDegree(_ownerEntity.transform.position, _ownerEntity.MousePos);
            _ownerEntity.EyeTransform.rotation = Quaternion.Euler(0, 0, angle);
        }

        private void KeyEThrow()
        {
            if (_currentBallStack == 0)
            {
                return;
            }
            _ownerEntity.AttackBallInstate();
            _currentBallStack--;
            _ownerEntity.SetThrowAnimation();
            if(_currentBallStack == 0)
                _throwProjectileUI.enabled = false;
        }

        private void KeyDownAttack()
        {
            if (_currentSwingCoolTime < _ownerEntity.SwingCooldown.Value)
            {
                return;
            }
            else
            {
                _isMouseDownCheck = true;
                _ownerEntity.SwingAreaMeshRenderer.enabled = true;
            }
        }

        private void KeyUpAttack()
        {
            _isMouseDownCheck = false;
            _ownerEntity.SwingAreaMeshRenderer.enabled = false;
        }


        private void KeySpaceDodge()
        {
            _ownerEntity.ChangeState(Player.States.Dodge);
        }
        
        
        #endregion
        
        #region BallCollisionMathFunc
        
        
        


        #endregion
        
    }

}