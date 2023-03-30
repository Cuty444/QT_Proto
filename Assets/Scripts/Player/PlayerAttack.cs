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
using UnityEditor;

namespace QT.Player
{
    public class PlayerAttack : MonoBehaviour
    {
        #region Inspector_Definition

        [Header("Bullet")] [SerializeField] private GameObject _bulletObject;
        [SerializeField] private TrailRenderer _trailRenderer;
        [SerializeField] private Transform _eyePos;
        [SerializeField] private Transform _swingAreaPos;
        [SerializeField] private Transform _batPos;
        [SerializeField] private BatSwing _batSwing;
        [SerializeField] private float _rotationTime = 0.1f; // 공속 부분과 동기화 필요 임시 수치
        [SerializeField] private float _ballRecoveryCoolTime = 5f;
        #endregion

        #region StartData_Declaration
        
        private GlobalDataSystem _globalDataSystem;
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
        private float _atkRadius;
        private int[] _swingRigidDmg;
        private int _ballStackMax;

        #endregion

        #region Global_Declaration

        private SpriteRenderer _batSpriteRenderer;

        private List<GameObject> _ballList = new List<GameObject>();

        private Color _swingAreaColor;

        private float _currentAtkCoolTime;
        private float _currentChargingTime;
        private float _currentBallRecoveryTime;
        private float _upAtkCentralAngle;
        private float _downAtkCentralAngle;
        private float _shootSpd;

        private int _currentBallStack;

        private bool _isUpDown = true;
        private bool _isMouseDownCheck = false;
        
        private Mesh _mesh;
        private MeshFilter _meshFilter;
        private MeshRenderer _meshRenderer;


        #endregion

        public Transform _batAngleTransform;
        private LayerMask _reflectLayerMask;
        void Start()
        {
            _globalDataSystem = SystemManager.Instance.GetSystem<GlobalDataSystem>();
            _bulletSpeed = _globalDataSystem.BallTable.ThrowSpd;
            _atkCoolTime = _globalDataSystem.BatTable.AtkCooldown;
            _chargingMaxTimes = _globalDataSystem.BatTable.ChargingMaxTimes;
            _atkShootSpd = _globalDataSystem.BatTable.AtkShootSpd;
            _swingRigidDmg = _globalDataSystem.BatTable.SwingRigidDmg;
            _atkCentralAngle = _globalDataSystem.BatTable.AtkCentralAngle;
            float halfAngle = _atkCentralAngle * 0.5f;
            _upAtkCentralAngle = -90.0f - halfAngle;
            _downAtkCentralAngle = -90.0f + halfAngle;
            _atkRadius = _globalDataSystem.BatTable.ATKRad;
            _ballStackMax = _globalDataSystem.CharacterTable.BallStackMax;
            InputSystem inputSystem = SystemManager.Instance.GetSystem<InputSystem>();
            inputSystem.OnKeyDownAttackEvent.AddListener(KeyDownAttack);
            inputSystem.OnKeyUpAttackEvent.AddListener(KeyUpAttack);
            inputSystem.OnKeyEThrowEvent.AddListener(KeyEThrow);
            _playerSystem = SystemManager.Instance.GetSystem<PlayerSystem>();
            _playerSystem.ChargeAtkShootEvent.AddListener(SetShootSpeed);
            _playerSystem.PlayerBallDestroyedEvent.AddListener(BallDestroyed);
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
            _swingAreaColor = new Color(0f, 0f, 1f, 0.2f);
            _meshFilter = _swingAreaPos.gameObject.GetComponent<MeshFilter>();
            _meshRenderer = _swingAreaPos.gameObject.GetComponent<MeshRenderer>();
            _mesh = CreateMesh(_atkRadius, _atkCentralAngle, 32);
            _meshFilter.mesh = _mesh;
            _meshRenderer.material = new Material(Shader.Find("Sprites/Default"));
            _meshRenderer.material.color = _swingAreaColor;
            _meshRenderer.enabled = false;
            _reflectLayerMask = 1 << LayerMask.NameToLayer("Wall");
        }

        private void Update()
        {
            AttackCoolTime();
            BallRecovery();
            SwingAreaInBallLineDraw();
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
                _currentChargingTime = 0f;
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

            BallPositionSwingCheck();
        }

        private void AttackBallInstate()
        {
            float bulletAngleDegree = Util.Math.GetDegree(transform.position, Camera.main.ScreenToWorldPoint(Input.mousePosition));
            BallMove ballMove = Instantiate(_bulletObject).GetComponent<BallMove>();
            ballMove.transform.position = transform.position;
            ballMove.transform.rotation = Quaternion.Euler(0, 0, bulletAngleDegree);
            ballMove.BulletSpeed = 0;
            _ballList.Add(ballMove.gameObject);
            _currentBallStack--;
        }

        private void BallDestroyed(GameObject bulletObject)
        {
            if (_ballList.Contains(bulletObject))
            {
                _ballList.Remove(bulletObject);
            }
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
            yield return new WaitForSeconds(0.1f);
            _trailRenderer.emitting = false;
            _batSwing.enabled = false;
            _batSpriteRenderer.enabled = false;
            _playerSystem.BatSwingEndEvent.Invoke();
        }

        private void SwingAreaInBallLineDraw() // 공격 범위내 공의 궤적들 라인 Draw
        {
            if (!_isMouseDownCheck)
                return;
            Vector2 startPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            for (int i = 0; i < _ballList.Count; i++)
            {
                if (SwingAreaCollision(_ballList[i].transform))
                {
                    Vector2 dir = (startPos - (Vector2)_ballList[i].transform.position).normalized;
                    dir = PlayerMouseAngleCorrectionBall(dir,transform,_ballList[i].transform,startPos);
                    RayCastAngleIncidence(_ballList[i].GetComponent<BallMove>()._lineRenderer,dir);
                }
            }
            
        }

        private void RayCastAngleIncidence(LineRenderer lineRenderer,Vector2 reflectDirection) // 레이캐스트 입사각 처리후 반사각 계산
        {
            lineRenderer.enabled = true;
            lineRenderer.positionCount = 2;
            lineRenderer.SetPosition(0,Vector2.zero);
            lineRenderer.SetPosition(1,Vector2.zero);
            int reflectCount = 2;
            
            RaycastHit2D hit2D = Physics2D.Raycast(lineRenderer.transform.position, reflectDirection, Mathf.Infinity,_reflectLayerMask);
            if (hit2D.collider != null)
            {
                Debug.Log("CollisionCheck");
                // 충돌 지점에서 입사각과 반사각 계산
                Vector2 incomingVec = reflectDirection;
                Vector2 normalVec = hit2D.normal;
                Vector2 reflectVec = Vector2.Reflect(incomingVec, normalVec);
                reflectDirection = reflectVec;

                // LineRenderer에 반사 지점 추가
                lineRenderer.SetPosition(0,lineRenderer.transform.position);
                lineRenderer.SetPosition(1, hit2D.point);
                reflectCount++;
                lineRenderer.positionCount = reflectCount;
                lineRenderer.SetPosition(2, hit2D.point + reflectDirection * 10f);
            }
        }
        
        private void BallPositionSwingCheck()
        {
            for (int i = 0; i < _ballList.Count; i++)
            {
                if (SwingAreaCollision(_ballList[i].transform))
                {
                    Vector2 _attackDirection = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    float bulletAngleDegree;
                    if(PlayerMouseAngleCorrectionBallCheck(transform, _ballList[i].transform,_attackDirection))
                    {
                        bulletAngleDegree = QT.Util.Math.GetDegree(transform.position, _attackDirection);
                    }
                    else
                    {
                        bulletAngleDegree = QT.Util.Math.GetDegree(_ballList[i].transform.position, _attackDirection);
                    }
                    BallMove Ball = _ballList[i].GetComponent<BallMove>();
                    Ball.transform.rotation = Quaternion.Euler(0, 0, bulletAngleDegree);
                    Ball.BulletSpeed = _shootSpd;
                    Ball.ForceChange();
                    Ball.SwingBallHit();
                    if (ChargeAtkPierce.None == _playerSystem.ChargeAtkPierce)
                    {
                        Ball.gameObject.layer = LayerMask.NameToLayer("Ball");
                    }
                    else if (_globalDataSystem.BatTable.ChargeAtkPierce.HasFlag(_playerSystem.ChargeAtkPierce))
                    {
                        Ball.gameObject.layer = LayerMask.NameToLayer("BallHit");
                    }
                    else
                    {
                        Ball.gameObject.layer = LayerMask.NameToLayer("Ball");
                    }
                }
            }
        }
        private bool SwingAreaCollision(Transform targetTransform)
        {
            Vector2 interV = targetTransform.position - transform.position;

            // target과 나 사이의 거리가 radius 보다 작다면
            if (interV.magnitude <= _atkRadius)
            {
                //Vector2 lookDir = AngleToDirZ(_eyePos.rotation.z);
                // '타겟-나 벡터'와 '내 정면 벡터'를 내적
                float dot = Vector2.Dot(interV.normalized, _eyePos.transform.right);
                // 두 벡터 모두 단위 벡터이므로 내적 결과에 cos의 역을 취해서 theta를 구함
                float theta = Mathf.Acos(dot);
                // angleRange와 비교하기 위해 degree로 변환
                float degree = Mathf.Rad2Deg * theta;

                // 시야각 판별
                if (degree <= _atkCentralAngle / 2f)
                    return true;
                else
                    return false;

            }
            else
                return false;
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

        private Vector2 PlayerMouseAngleCorrectionBall(Vector2 dir,Transform playerTransform,Transform targetTransform,Vector2 mousePos) // 플레이어와 탄막 사이의 마우스 각도 보정처리
        {
            Vector2 playerPos = playerTransform.position;
            Vector2 targetPos = targetTransform.position;

            float pt = Vector2.Distance(playerPos, targetPos);
            float pm = Vector2.Distance(playerPos, mousePos);
            if(pt > pm)
            {
                dir = (mousePos - (Vector2) playerTransform.position).normalized;
            }
            

            return dir;
        }

        private bool PlayerMouseAngleCorrectionBallCheck(Transform playerTransform, Transform targetTransform, Vector2 mousePos)
        {
            Vector2 playerPos = playerTransform.position;
            Vector2 targetPos = targetTransform.position;
            return Vector2.Distance(playerPos, targetPos) > Vector2.Distance(playerPos, mousePos);
        }
        
        private void SetShootSpeed(float speed)
        {
            _shootSpd = speed;
        }
    }
}