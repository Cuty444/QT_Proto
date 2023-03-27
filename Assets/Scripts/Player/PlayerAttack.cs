using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using QT.Core;
using QT.Core.Input;
using QT.Core.Player;
using QT.Core.Data;
using QT.Data;
using QT.UI;
using QT.Player.Bat;
using QT.Ball;
using Unity.Mathematics;

namespace QT.Player
{
    public class PlayerAttack : MonoBehaviour
    {
        #region Inspector_Definition

        [Header("Bullet")] [SerializeField] private GameObject _bulletObject;
        [SerializeField] private TrailRenderer _trailRenderer;
        [SerializeField] private Transform _batPos;
        [SerializeField] private BatSwing _batSwing;
        [SerializeField] private float _rotationTime = 0.1f; // 공속 부분과 동기화 필요 임시 수치
        [SerializeField] private float _ballRecoveryCoolTime = 5f;
        #endregion

        #region StartData_Declaration

        private PlayerSystem _playerSystem;
        private PlayerCanvas _playerCanvas;
        private RectTransform _chargingBarBackground;
        private Image _chargingBarImage;
        private Image _ballStackBarImage;
        private Text _ballStackText;

        private float _bulletSpeed;
        private float[] _atkShootSpd;
        private float _atkCoolTime;
        private float[] _chargingMaxTimes;
        private float _atkCentralAngle;
        private int[] _swingRigidDmg;
        private int _ballStackMax;

        #endregion

        #region Global_Declaration

        private SpriteRenderer _batSpriteRenderer;

        private float _currentAtkCoolTime;
        private float _currentChargingTime;
        private float _currentBallRecoveryTime;
        private float _upAtkCentralAngle;
        private float _downAtkCentralAngle;

        private int _currentBallStack;

        private bool _isUpDown = true;
        private bool _isMouseDownCheck = false;
        

        #endregion

        public Transform _batAngleTransform;
        void Start()
        {
            GlobalDataSystem globalDataSystem = SystemManager.Instance.GetSystem<GlobalDataSystem>();
            _bulletSpeed = globalDataSystem.BallTable.ThrowSpd;
            _atkCoolTime = globalDataSystem.BatTable.AtkCooldown;
            _chargingMaxTimes = globalDataSystem.BatTable.ChargingMaxTimes;
            _atkShootSpd = globalDataSystem.BatTable.AtkShootSpd;
            _swingRigidDmg = globalDataSystem.BatTable.SwingRigidDmg;
            _atkCentralAngle = globalDataSystem.BatTable.AtkCentralAngle;
            float halfAngle = _atkCentralAngle * 0.5f;
            _upAtkCentralAngle = -90.0f - halfAngle;
            _downAtkCentralAngle = -90.0f + halfAngle;
            _ballStackMax = globalDataSystem.CharacterTable.BallStackMax;
            InputSystem inputSystem = SystemManager.Instance.GetSystem<InputSystem>();
            inputSystem.OnKeyDownAttackEvent.AddListener(KeyDownAttack);
            inputSystem.OnKeyUpAttackEvent.AddListener(KeyUpAttack);
            inputSystem.OnKeyEThrowEvent.AddListener(KeyEThrow);
            _playerSystem = SystemManager.Instance.GetSystem<PlayerSystem>();
            _currentAtkCoolTime = _atkCoolTime;
            _currentChargingTime = 0f;
            _isMouseDownCheck = false;
            _playerCanvas = UIManager.Instance.GetUIPanel<PlayerCanvas>();
            _chargingBarBackground = _playerCanvas.ChargingBarBackground;
            _chargingBarBackground.gameObject.SetActive(false);
            _chargingBarImage = _chargingBarBackground.GetComponentsInChildren<Image>()[1];
            PlayerHPCanvas playerHpCanvas = UIManager.Instance.GetUIPanel<PlayerHPCanvas>();
            _ballStackBarImage = playerHpCanvas.PlayerBallStackImage;
            _ballStackText = playerHpCanvas.PlayerBallStackText;
            _batSpriteRenderer = _batSwing.GetComponent<SpriteRenderer>();
            _batSpriteRenderer.enabled = false;
            _currentBallStack = _ballStackMax;
            _ballStackBarImage.fillAmount = 0;
            _ballStackText.text = _currentBallStack.ToString();
        }

        private void Update()
        {
            AttackCoolTime();
            BallRecovery();
        }

        private void LateUpdate()
        {
            _playerCanvas.transform.position = transform.position;
        }

        private void FixedUpdate()
        {
            if (_currentAtkCoolTime > _atkCoolTime)
            {
                PlayerSwingAngle();
            }
        }

        #region AttackFunc

        private void AttackCoolTime()
        {
            _currentAtkCoolTime += Time.deltaTime;
            if (_isMouseDownCheck)
            {
                _currentChargingTime += Time.deltaTime;
                if (_currentChargingTime > _chargingMaxTimes[0])
                {
                    if (!_chargingBarBackground.gameObject.activeSelf)
                    {
                        _chargingBarBackground.gameObject.SetActive(true);
                    }

                    _chargingBarImage.fillAmount = QT.Util.Math.floatNormalization(_currentChargingTime,
                        _chargingMaxTimes[_chargingMaxTimes.Length - 1], _chargingMaxTimes[0]);
                }
            }
        }

        private void KeyEThrow()
        {
            if (_currentBallStack == 0)
                return;
            AttackBulletInstate();
        }

        private void KeyDownAttack()
        {
            if (_currentAtkCoolTime < _atkCoolTime)
            {
                return;
            }
            else
            {
                _isMouseDownCheck = true;
                _currentChargingTime = 0f;
            }
        }

        private void KeyUpAttack()
        {
            AttackCheck();
            _isMouseDownCheck = false;
        }

        private void AttackCheck()
        {
            if (!_isMouseDownCheck)
                return;
            _batSpriteRenderer.enabled = true;
            if (_currentChargingTime <= _chargingMaxTimes[0])
            {
                // 일반스윙
                AttackBatSwing();
            }
            else
            {
                // 차징스윙
                ChargingBatSwing();
            }
        }

        private void AttackBulletInstate()
        {
            float bulletAngleDegree = Util.Math.GetDegree(transform.position, Camera.main.ScreenToWorldPoint(Input.mousePosition));
            BallMove Ball = Instantiate(_bulletObject).GetComponent<BallMove>();
            Ball.transform.position = transform.position;
            Ball.transform.rotation = Quaternion.Euler(0, 0, bulletAngleDegree);
            Ball.BulletSpeed = _bulletSpeed;
            _currentBallStack--;
        }

        private void AttackBatSwing()
        {
            float rotationSpeed;
            _playerSystem.ChargeAtkPierce = ChargeAtkPierce.None;
            if (_isUpDown == true)
            {
                _batPos.transform.localRotation = Quaternion.Euler(0f, 0f, _upAtkCentralAngle);
                rotationSpeed = Mathf.DeltaAngle(_batPos.localEulerAngles.z,_downAtkCentralAngle) / _rotationTime;
                StartCoroutine(BatSwing(_batPos, rotationSpeed, _downAtkCentralAngle));
            }
            else
            {
                _batPos.transform.localRotation = Quaternion.Euler(0f, 0f, _downAtkCentralAngle);
                rotationSpeed = (Mathf.DeltaAngle(_batPos.localEulerAngles.z, _upAtkCentralAngle) / _rotationTime) * -1f;
                StartCoroutine(BatSwing(_batPos, rotationSpeed, _upAtkCentralAngle));
            }

            //_isUpDown = !_isUpDown;
            _currentAtkCoolTime = 0f;
            if (!_chargingBarBackground.gameObject.activeSelf)
            {
                _playerSystem.ChargeAtkShootEvent.Invoke(_atkShootSpd[0]);
                _playerSystem.BatSwingRigidHitEvent.Invoke(_swingRigidDmg[0]);
            }
        }

        private void ChargingBatSwing()
        {
            AttackBatSwing();
            for (int i = _chargingMaxTimes.Length - 1; i >= 0; i--)
            {
                if (_chargingMaxTimes[i] < _currentChargingTime)
                {
                    _playerSystem.ChargeAtkPierce = (ChargeAtkPierce) (1 << i);
                    _playerSystem.ChargeAtkShootEvent.Invoke(_atkShootSpd[i]);
                    _playerSystem.BatSwingRigidHitEvent.Invoke(_swingRigidDmg[i]);
                    break;
                }
            }

            _chargingBarBackground.gameObject.SetActive(false);
        }


        private IEnumerator BatSwing(Transform targetTransform, float rotateSpeed, float targetZ)
        {
            _trailRenderer.enabled = true;
            _batSwing.enabled = true;
            Quaternion targetRotation = Quaternion.Euler(0f, 0f, targetZ);
            float currentRotationTime = 0.0f;
            while (_rotationTime > currentRotationTime)
            {
                targetTransform.localRotation = Quaternion.RotateTowards(targetTransform.localRotation, targetRotation,
                    rotateSpeed * Time.deltaTime);
                yield return null;
                currentRotationTime += Time.deltaTime;
            }

            targetTransform.localRotation = Quaternion.RotateTowards(targetTransform.localRotation, targetRotation,
                rotateSpeed * Time.deltaTime);
            yield return new WaitForSeconds(0.1f);
            _trailRenderer.enabled = false;
            _batSwing.enabled = false;
            _batSpriteRenderer.enabled = false;
            _playerSystem.BatSwingEndEvent.Invoke();
        }

        #endregion

        private void PlayerSwingAngle()
        {
            float playerRotation = _batAngleTransform.rotation.z;
            switch (playerRotation)
            {
                case > 0.35f and < 0.7f:
                    _isUpDown = false;
                    break;
                case > 0.7f and < 0.95f:
                    _isUpDown = true;
                    break;
                case > 0.95f:
                case < -0.95f:
                    _isUpDown = true;
                    break;
                case > -0.95f and < -0.7f:
                    _isUpDown = false;
                    break;
                case > -0.7f and < -0.35f:
                    _isUpDown = true;
                    break;
                default:
                    _isUpDown = false;
                    break;
            }
        }

        private void BallRecovery()
        {
            if (_ballStackMax == _currentBallStack)
                return;
            _currentBallRecoveryTime += Time.deltaTime;
            if (_currentBallRecoveryTime >= _ballRecoveryCoolTime)
            {
                _currentBallRecoveryTime = 0f;
                _currentBallStack++;
            }
            _ballStackBarImage.fillAmount = QT.Util.Math.floatNormalization(_currentBallRecoveryTime,_ballRecoveryCoolTime, 0);
            _ballStackText.text = _currentBallStack.ToString();
        }
    }
}