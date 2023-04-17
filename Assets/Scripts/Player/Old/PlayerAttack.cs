using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using QT.Core;
using QT.Core.Input;
using QT.Core.Data;
using QT.Data;
using QT.UI;
using QT.Player.Bat;
using QT.Ball;

namespace QT.Player
{
    public class PlayerAttack : MonoBehaviour
    {
        #region Inspector_Definition

        [Header("Bullet")][SerializeField] private GameObject _bulletObject;
        [SerializeField] private TrailRenderer _trailRenderer;
        [SerializeField] private Transform _eyeTransform;
        [SerializeField] private Transform _swingAreaPos;
        [SerializeField] private Transform _batPos;
        [SerializeField] private BatSwing _batSwing;
        [SerializeField] private float _rotationTime = 0.1f; // 공속 부분과 동기화 필요 임시 수치
        [SerializeField] private float _ballRecoveryCoolTime = 5f;
        #endregion

        #region StartData_Declaration

        private GlobalDataSystem _globalDataSystem;
        private PlayerManager _playerManager;
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
        private float _atkRadius;
        private int[] _swingRigidDmg;
        private int _ballStackMax;
        private int[] _chargeBounceValues;

        #endregion

        #region Global_Declaration

        private SpriteRenderer _batSpriteRenderer;


        private Color _swingAreaColor;

        private float _currentAtkCoolTime;
        private float _currentChargingTime;
        private float _currentBallRecoveryTime;
        private float _upAtkCentralAngle;
        private float _downAtkCentralAngle;
        private float _beforeChargingTime;

        private int _currentBallStack;

        private bool _isUpDown = true;
        private bool _isMouseDownCheck = false;

        private bool _isSwingBallHit;

        private Mesh _mesh;
        private MeshFilter _meshFilter;
        private MeshRenderer _meshRenderer;

        public Transform EyeTransform => _eyeTransform;


        #endregion

        void Start()
        {
            _globalDataSystem = SystemManager.Instance.GetSystem<GlobalDataSystem>();
            _bulletSpeed = _globalDataSystem.BallTable.ThrowSpd;
            _atkCoolTime = _globalDataSystem.BatTable.AtkCooldown;
            _chargingMaxTimes = _globalDataSystem.BatTable.ChargingMaxTimes;
            _atkShootSpd = _globalDataSystem.BatTable.AtkShootSpd;
            _swingRigidDmg = _globalDataSystem.BatTable.SwingRigidDmg;
            _atkCentralAngle = _globalDataSystem.BatTable.AtkCentralAngle;
            _chargeBounceValues = _globalDataSystem.BatTable.ChargeBounceValue;
            float halfAngle = _atkCentralAngle * 0.5f;
            _upAtkCentralAngle = -90.0f - halfAngle;
            _downAtkCentralAngle = -90.0f + halfAngle;
            _atkRadius = _globalDataSystem.BatTable.ATKRad;
            _ballStackMax = _globalDataSystem.CharacterTable.BallStackMax;
            InputSystem inputSystem = SystemManager.Instance.GetSystem<InputSystem>();
            inputSystem.OnKeyDownAttackEvent.AddListener(KeyDownAttack);
            inputSystem.OnKeyUpAttackEvent.AddListener(KeyUpAttack);
            inputSystem.OnKeyEThrowEvent.AddListener(KeyEThrow);
            _playerManager = SystemManager.Instance.PlayerManager;
            _playerManager.BatSwingBallHitEvent.AddListener(SwingBallHitCheck);
            _currentAtkCoolTime = _atkCoolTime;
            _currentChargingTime = 0f;
            _isMouseDownCheck = false;
            _playerCanvas = SystemManager.Instance.UIManager.GetUIPanel<PlayerCanvas>();
            _chargingBarBackground = _playerCanvas.ChargingBarBackground;
            _chargingBarBackground.gameObject.SetActive(false);
            _chargingBarImage = _chargingBarBackground.GetComponentsInChildren<Image>()[1];
            PlayerHPCanvas playerHpCanvas = SystemManager.Instance.UIManager.GetUIPanel<PlayerHPCanvas>();
            _ballStackBarImage = playerHpCanvas.PlayerBallStackImage;
            //_ballStackText = playerHpCanvas.PlayerBallStackText;
            _batSpriteRenderer = _batSwing.GetComponent<SpriteRenderer>();
            _batSpriteRenderer.enabled = false;
            _currentBallStack = _ballStackMax;
            _ballStackBarImage.fillAmount = 0;
            _ballStackText.text = _currentBallStack.ToString();
            _swingAreaColor = new Color(0f, 0f, 1f, 0.2f);
            _meshFilter = _swingAreaPos.gameObject.GetComponent<MeshFilter>();
            _meshRenderer = _swingAreaPos.gameObject.GetComponent<MeshRenderer>();
            _mesh = CreateMesh(_atkRadius, _atkCentralAngle, 32);
            _meshFilter.mesh = _mesh;
            _meshRenderer.material = new Material(Shader.Find("Sprites/Default"));
            _meshRenderer.material.color = _swingAreaColor;
            _meshRenderer.enabled = false;
        }

        private void Update()
        {
            AttackCoolTime();
            BallRecovery();
        }

        private void LateUpdate()
        {
            _playerCanvas.transform.position = transform.position; // 이 코드 캔버스 LateUpdate로 옮기기
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
                _playerManager.PlayerCurrentChargingTimeEvent.Invoke(_currentChargingTime);
                if (_currentChargingTime > _chargingMaxTimes[0])
                {
                    if (!_chargingBarBackground.gameObject.activeSelf)
                    {
                        _chargingBarBackground.gameObject.SetActive(true);
                    }

                    _chargingBarImage.fillAmount = QT.Util.Math.Remap(_currentChargingTime,
                        _chargingMaxTimes[_chargingMaxTimes.Length - 1], _chargingMaxTimes[0]);
                }
            }
        }

        private void KeyEThrow()
        {
            if (_currentBallStack == 0)
                return;
            AttackBallInstate();
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
                _meshRenderer.enabled = true;
            }
        }

        private void KeyUpAttack()
        {
            AttackCheck();
            _isMouseDownCheck = false;
            _meshRenderer.enabled = false;

        }

        private void AttackCheck()
        {
            if (!_isMouseDownCheck)
                return;
            _batSpriteRenderer.enabled = true;
            _beforeChargingTime = _currentChargingTime;
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
            _currentChargingTime = 0f;
            _playerManager.PlayerCurrentChargingTimeEvent.Invoke(_currentChargingTime);
        }

        private void AttackBallInstate()
        {
            float bulletAngleDegree = Util.Math.GetDegree(transform.position, Camera.main.ScreenToWorldPoint(Input.mousePosition));
            BallMove ballMove = Instantiate(_bulletObject).GetComponent<BallMove>();
            ballMove.transform.position = transform.position;
            ballMove.transform.rotation = Quaternion.Euler(0, 0, bulletAngleDegree);
            ballMove.BulletSpeed = 0;
            _currentBallStack--;
        }

        private void AttackBatSwing()
        {
            float rotationSpeed;
            _playerManager.ChargeAtkPierce = ChargeAtkPierce.None;
            if (_isUpDown == true)
            {
                _batPos.transform.localRotation = Quaternion.Euler(0f, 0f, _upAtkCentralAngle);
                rotationSpeed = Mathf.DeltaAngle(_batPos.localEulerAngles.z, _downAtkCentralAngle) / _rotationTime;
                StartCoroutine(BatSwing(_batPos, rotationSpeed, _downAtkCentralAngle));
            }
            else
            {
                _batPos.transform.localRotation = Quaternion.Euler(0f, 0f, _downAtkCentralAngle);
                rotationSpeed = (Mathf.DeltaAngle(_batPos.localEulerAngles.z, _upAtkCentralAngle) / _rotationTime) * -1f;
                StartCoroutine(BatSwing(_batPos, rotationSpeed, _upAtkCentralAngle));
            }

            _currentAtkCoolTime = 0f;
            if (!_chargingBarBackground.gameObject.activeSelf)
            {
                _playerManager.ChargeAtkShootEvent.Invoke(_atkShootSpd[0]);
                _playerManager.BatSwingRigidHitEvent.Invoke(_swingRigidDmg[0]);
                _playerManager.ChargeBounceValueEvent.Invoke(_chargeBounceValues[0]);
            }
        }

        private void ChargingBatSwing()
        {
            AttackBatSwing();
            for (int i = _chargingMaxTimes.Length - 1; i >= 0; i--)
            {
                if (_chargingMaxTimes[i] < _currentChargingTime)
                {
                    _playerManager.ChargeAtkPierce = (ChargeAtkPierce) (1 << i);
                    _playerManager.ChargeAtkShootEvent.Invoke(_atkShootSpd[i]);
                    _playerManager.BatSwingRigidHitEvent.Invoke(_swingRigidDmg[i]);
                    _playerManager.ChargeBounceValueEvent.Invoke(_chargeBounceValues[i]);
                    break;
                }
            }

            _chargingBarBackground.gameObject.SetActive(false);
        }


        private IEnumerator BatSwing(Transform targetTransform, float rotateSpeed, float targetZ)
        {
            _trailRenderer.emitting = true;
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
            _playerManager.BatSwingEndEvent.Invoke();
            if (_chargingMaxTimes[_chargingMaxTimes.Length - 1] < _beforeChargingTime && _isSwingBallHit)
            {
                _playerManager.BatSwingTimeScaleEvent.Invoke(true);
                Time.timeScale = 0.1f;
                yield return new WaitForSeconds(0.01f);
                Time.timeScale = 0.2f;
                yield return new WaitForSeconds(0.01f);
                Time.timeScale = 0.3f;
                yield return new WaitForSeconds(0.01f);
                Time.timeScale = 0.5f;
                yield return new WaitForSeconds(0.01f);
                Time.timeScale = 0.75f;
                yield return new WaitForSeconds(0.01f);
                Time.timeScale = 1.0f;
                _playerManager.BatSwingTimeScaleEvent.Invoke(false);
                _isSwingBallHit = false;
            }
            else
            {
                yield return new WaitForSeconds(0.1f);
            }
            _beforeChargingTime = 0f;
            _trailRenderer.emitting = false;
            _batSwing.enabled = false;
            _batSpriteRenderer.enabled = false;

        }

        #endregion

        private void PlayerSwingAngle()
        {
            float playerRotation = _eyeTransform.rotation.z;
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
            _ballStackBarImage.fillAmount = QT.Util.Math.Remap(_currentBallRecoveryTime, _ballRecoveryCoolTime, 0);
            _ballStackText.text = _currentBallStack.ToString();
        }

        private Mesh CreateMesh(float radius, float angle, int segments)
        {
            Mesh mesh = new Mesh();
            int vertexCount = segments + 2;
            int indexCount = segments * 3;
            Vector3[] vertices = new Vector3[vertexCount];
            int[] indices = new int[indexCount];
            float angleRad = angle * Mathf.Deg2Rad;
            float angleStep = angleRad / segments;
            float currentAngle = -angleRad / 2f;
            vertices[0] = Vector3.zero;
            for (int i = 0; i <= segments; i++)
            {
                vertices[i + 1] = new Vector3(Mathf.Cos(currentAngle), Mathf.Sin(currentAngle), 0f) * radius;
                currentAngle += angleStep;
            }
            for (int i = 0; i < segments; i++)
            {
                indices[i * 3] = 0;
                indices[i * 3 + 1] = i + 1;
                indices[i * 3 + 2] = i + 2;
            }
            mesh.vertices = vertices;
            mesh.triangles = indices;
            mesh.RecalculateBounds();
            return mesh;
        }

        private void SwingBallHitCheck()
        {
            _isSwingBallHit = true;
        }
    }
}