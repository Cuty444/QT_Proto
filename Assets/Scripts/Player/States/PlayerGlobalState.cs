using UnityEngine;
using System.Collections;
using QT.Core;
using QT.Core.Input;

namespace QT.Player
{
    [FSMState((int)Player.States.Global, false)]
    public class PlayerGlobalState : FSMState<Player>
    {
        private InputSystem _inputSystem;
        
        private float _currentSwingCoolTime;
        private float _currentChargingTime;

        private int _currentBallStack;

        
        private bool _isMouseDownCheck;
        
        public PlayerGlobalState(IFSMEntity owner) : base(owner)
        {
            _inputSystem = SystemManager.Instance.GetSystem<InputSystem>();
            _inputSystem.OnKeyDownAttackEvent.AddListener(KeyDownAttack);
            _inputSystem.OnKeyUpAttackEvent.AddListener(KeyUpAttack);
            _inputSystem.OnKeyEThrowEvent.AddListener(KeyEThrow);

            _currentBallStack = (int)_ownerEntity.BallStackMax.Value + 100;
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
        }

        public override void FixedUpdateState()
        {
            _ownerEntity.AngleAnimation();
            //if (_currentSwingCoolTime > _ownerEntity.SwingCooldown)
            //{
            //    _isUpDown = _ownerEntity.PlayerSwingAngle();
            //}

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
                return;
            _ownerEntity.AttackBallInstate();
            _currentBallStack--;
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

        

        #endregion
        
        #region BallCollisionMathFunc
        
        
        


        #endregion
        
    }

}