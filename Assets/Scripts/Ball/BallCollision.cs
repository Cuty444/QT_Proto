using QT.Core;
using QT.Core.Data;
using QT.Core.Player;
using QT.Data;
using QT.Player;
using UnityEngine;

namespace QT.Ball
{
    public class BallCollision : MonoBehaviour
    {
        #region StartData_Declaration

        private GlobalDataSystem _globalDataSystem;
        private PlayerSystem _playerSystem;

        private Transform _playerTransform;
        private Transform _eyeTransform;
        private float[] _chargingMaxTimes;
        private float _atkCentralAngle;
        private float _atkRadius;
        private int[] _chargeBounceValues;

        #endregion

        #region Global_Declaration

        private BallMove _ballMove;
        
        private LineRenderer _lineRenderer;

        private LayerMask _reflectLayerMask;

        private float _currentChargingTime;

        private float _shootSpd;

        private int _chargeBounceValue;

        private int _currentChargeBounceValue;

        private bool _isWallBounceCheck;

        #endregion

        void Start()
        {
            _globalDataSystem = SystemManager.Instance.GetSystem<GlobalDataSystem>();
            _chargingMaxTimes = _globalDataSystem.BatTable.ChargingMaxTimes;
            _atkCentralAngle = _globalDataSystem.BatTable.AtkCentralAngle;
            _atkRadius = _globalDataSystem.BatTable.ATKRad;
            _chargeBounceValues = _globalDataSystem.BatTable.ChargeBounceValue;
            _playerSystem = SystemManager.Instance.GetSystem<PlayerSystem>();
            _playerSystem.PlayerCurrentChargingTimeEvent.AddListener(SetCurrentChargingTime);
            _playerSystem.BatSwingEndEvent.AddListener(BallPositionSwingCheck);
            _playerSystem.ChargeAtkShootEvent.AddListener(SetShootSpeed);
            _playerSystem.ChargeBounceValueEvent.AddListener(SetCurrentChargingBounceValue);
            _playerTransform = _playerSystem.PlayerTransform;
            _eyeTransform = _playerTransform.GetComponent<PlayerAttack>().EyeTransform;
            _lineRenderer = GetComponentInChildren<LineRenderer>();
            _reflectLayerMask = 1 << LayerMask.NameToLayer("Wall");
            _ballMove = GetComponent<BallMove>();
        }

        private void Update()
        {
            SwingAreaInBallLineDraw();
        }
        
        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (gameObject.layer == LayerMask.NameToLayer("BallBounce"))
                return;
            if (collision.gameObject.layer == LayerMask.NameToLayer("Wall"))
            {
                _currentChargeBounceValue--;
                if (_currentChargeBounceValue <= 0)
                {
                    _ballMove.BulletSpeed = 0f;
                    _ballMove.ForceChange();
                    gameObject.layer = LayerMask.NameToLayer("BallBounce");
                    //_rigidbody2D.MovePosition(_lineRenderer.GetPosition(_lineRenderer.positionCount - 1));
                    //_rigidbody2D.MovePosition( GetTargetPosition(_lineRenderer.GetPosition(_lineRenderer.positionCount - 2),
                    //    _lineRenderer.GetPosition(_lineRenderer.positionCount - 3), transform.localScale.x * 0.5f));
                    //GameObject bounceZeroObject = Instantiate(gameObject,
                    //    _lineRenderer.GetPosition(_lineRenderer.positionCount - 2), Quaternion.identity);
                    //bounceZeroObject.layer = LayerMask.NameToLayer("BallBounce");
                    //gameObject.SetActive(false);
                }
            }
        }

        private void SwingAreaInBallLineDraw() // 공격 범위내 공의 궤적들 라인 Draw
        {
            if (_currentChargingTime < _chargingMaxTimes[0])
            {
                _lineRenderer.enabled = false;
                return;
            }

            Vector2 startPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if (SwingAreaCollision(_playerTransform, transform))
            {
                Vector2 dir = (startPos - (Vector2) transform.position).normalized;
                dir = PlayerMouseAngleCorrectionBall(dir, _playerTransform, transform, startPos);
                RayCastAngleIncidence(_lineRenderer, dir);
            }
            else
            {
                _lineRenderer.enabled = false;
            }
        }

        private void BallPositionSwingCheck()
        {
            if (SwingAreaCollision(_playerTransform, transform))
            {
                Vector2 _attackDirection = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                float bulletAngleDegree;
                if (PlayerMouseAngleCorrectionBallCheck(_playerTransform, transform, _attackDirection))
                {
                    bulletAngleDegree = QT.Util.Math.GetDegree(_playerTransform.position, _attackDirection);
                }
                else
                {
                    bulletAngleDegree = QT.Util.Math.GetDegree(transform.position, _attackDirection);
                }

                _ballMove.transform.rotation = Quaternion.Euler(0, 0, bulletAngleDegree);
                _ballMove.BulletSpeed = _shootSpd;
                _ballMove.ForceChange();
                _ballMove.SwingBallHit();
                if (ChargeAtkPierce.None == _playerSystem.ChargeAtkPierce)
                {
                    gameObject.layer = LayerMask.NameToLayer("Ball");
                }
                else if (_globalDataSystem.BatTable.ChargeAtkPierce.HasFlag(_playerSystem.ChargeAtkPierce))
                {
                    gameObject.layer = LayerMask.NameToLayer("BallHit");
                }
                else
                {
                    gameObject.layer = LayerMask.NameToLayer("Ball");
                }
                _playerSystem.BatSwingBallHitEvent.Invoke();
            }
        }

        #region BallCollisionMathFunc

        private bool SwingAreaCollision(Transform originalTransform, Transform targetTransform)
        {
            Vector2 interV = targetTransform.position - originalTransform.position;
            float targetRadius = targetTransform.localScale.x / 2f;
            // target과 나 사이의 거리가 radius 보다 작다면
            if (interV.magnitude <= _atkRadius + targetRadius)
            {
                // '타겟-나 벡터'와 '내 정면 벡터'를 내적
                float dot = Vector2.Dot(interV.normalized, _eyeTransform.right);
                // 두 벡터 모두 단위 벡터이므로 내적 결과에 cos의 역을 취해서 theta를 구함
                float theta = Mathf.Acos(dot);
                // angleRange와 비교하기 위해 degree로 변환
                float degree = Mathf.Rad2Deg * theta;

                // 시야각 판별
                if (degree <= _atkCentralAngle / 2f)
                    return true;
            }

            return false;
        }

        private Vector2 PlayerMouseAngleCorrectionBall(Vector2 dir, Transform playerTransform,
            Transform targetTransform, Vector2 mousePos) // 플레이어와 탄막 사이의 마우스 각도 보정처리
        {
            Vector2 playerPos = playerTransform.position;
            Vector2 targetPos = targetTransform.position;

            float pt = Vector2.Distance(playerPos, targetPos);
            float pm = Vector2.Distance(playerPos, mousePos);
            if (pt > pm)
            {
                dir = (mousePos - (Vector2) playerTransform.position).normalized;
            }


            return dir;
        }

        private bool PlayerMouseAngleCorrectionBallCheck(Transform playerTransform, Transform targetTransform,
            Vector2 mousePos)
        {
            Vector2 playerPos = playerTransform.position;
            Vector2 targetPos = targetTransform.position;
            return Vector2.Distance(playerPos, targetPos) > Vector2.Distance(playerPos, mousePos);
        }

        private void RayCastAngleIncidence(LineRenderer lineRenderer,
                Vector2 reflectDirection) // 레이캐스트 입사각 처리후 반사각 계산
        {
            lineRenderer.enabled = true;
            lineRenderer.positionCount = 0;
            int reflectCount = 1;
            lineRenderer.positionCount = reflectCount;
            float radius = transform.localScale.x * 0.5f;
            lineRenderer.SetPosition(0, lineRenderer.transform.position);
            for (; reflectCount < _chargeBounceValue + 2; reflectCount++)
            {
                if (!BallBounceRayCast(lineRenderer, ref reflectDirection, reflectCount,radius))
                {
                    break;
                }
            }
            if (reflectCount > 3)
            {
                float endTime = QT.Util.Math.Remap(3, reflectCount, 0);
                Gradient gradient = new Gradient();
                gradient.SetKeys(new GradientColorKey[] { new GradientColorKey(Color.white, 0.0f), new GradientColorKey(Color.white, 1.0f) },
                    new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(1.0f, endTime), new GradientAlphaKey(0.0f, endTime + 0.03f), new GradientAlphaKey(0f, 1f) });
                lineRenderer.colorGradient = gradient;
            }
            else
            {
                Gradient gradient = new Gradient();
                gradient.SetKeys(new GradientColorKey[] { new GradientColorKey(Color.white, 0.0f), new GradientColorKey(Color.white, 1.0f) },
                    new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(1f, 1f) });
                lineRenderer.colorGradient = gradient;
            }
        }

        private bool BallBounceRayCast(LineRenderer lineRenderer, ref Vector2 reflectDirection, int reflectCount, float radius)
        {
            RaycastHit2D hit2D = Physics2D.CircleCast(lineRenderer.GetPosition((lineRenderer.positionCount - 1)),radius ,reflectDirection, Mathf.Infinity,
                _reflectLayerMask);
            if (hit2D.collider != null)
            {
                Vector2 hitPointMinusRadius = hit2D.point + (hit2D.normal * radius);
                reflectDirection = Vector2.Reflect(reflectDirection, hit2D.normal);
                lineRenderer.positionCount = reflectCount + 1;
                lineRenderer.SetPosition(reflectCount, hitPointMinusRadius);
                //lineRenderer.SetPosition(reflectCount, hitPointMinusRadius + reflectDirection);
                return true;
            }

            return false;
        }

        #endregion

        private void SetCurrentChargingTime(float time)
        {
            _currentChargingTime = time;
            _chargeBounceValue = _chargeBounceValues[0];
            for (int i = _chargingMaxTimes.Length - 1; i >= 0; i--)
            {
                if (_currentChargingTime > _chargingMaxTimes[i])
                {
                    _chargeBounceValue = _chargeBounceValues[i];
                    break;
                }
            }
        }

        private void SetCurrentChargingBounceValue(int value)
        {
            _currentChargeBounceValue = value;
        }


        private void SetShootSpeed(float speed)
        {
            _shootSpd = speed;
        }
        
    }
}