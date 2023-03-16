using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using QT.Core;
using QT.Core.Input;
using QT.Core.Player;
using QT.Data;
using QT.UI;

namespace QT.Player
{
    public class PlayerAttack : MonoBehaviour
    {
        [Header("Bullet")]
        [SerializeField] private GameObject _bulletObject;
        //[Header("Arm")]
        //[SerializeField] private Transform _armTransform;
        [SerializeField] private TrailRenderer _trailRenderer;
        [SerializeField] private Transform _batPos;
        [SerializeField] private BatSwing _batSwing;
        [SerializeField] private float _rotationTime = 0.1f;

        private Image _chargingBarImage;
        
        private Vector2 _attackDirection;
        private float _currentAtkCoolTime;
        private float _currentChargingTime;
        private GameObject _tempBallObject = null;
        private bool _isUpDown = true;
        private float[] _chargingMaxTimes;
        private bool _isMouseDownCheck = false;
        private float _coolTime;
        private float _bulletSpeed;
        private float[] _atkShootSpd;
        private int[] _swingRigidDmg;

        private PlayerSystem _playerSystem;
        private PlayerCanvas _playerCanvas;
        private RectTransform _chargingBarBackground;
        // Start is called before the first frame update
        void Start()
        {
            GlobalDataSystem globalDataSystem = SystemManager.Instance.GetSystem<GlobalDataSystem>();
            _bulletSpeed = globalDataSystem.BallTable.ThrowSpd;
            _coolTime = globalDataSystem.BatTable.AtkCooldown;
            _chargingMaxTimes = globalDataSystem.BatTable.ChargingMaxTimes;
            _atkShootSpd = globalDataSystem.BatTable.AtkShootSpd;
            _swingRigidDmg = globalDataSystem.BatTable.SwingRigidDmg;
            InputSystem inputSystem = SystemManager.Instance.GetSystem<InputSystem>();
            inputSystem.OnKeyDownAttackEvent.AddListener(KeyDownAttack);
            inputSystem.OnKeyUpAttackEvent.AddListener(KeyUpAttack);
            _playerSystem = SystemManager.Instance.GetSystem<PlayerSystem>();
            _playerSystem.BallMinSpdDestroyedEvent.AddListener(BallObjectDestroyedChecking);
            //inputSystem.OnRightKeyDownGrapEvent.AddListener(GrapCheck);
            _currentAtkCoolTime = _coolTime;
            _currentChargingTime = 0f;
            _isMouseDownCheck = false;
            _playerCanvas = UIManager.Instance.GetUIPanel<PlayerCanvas>();
            _chargingBarBackground = _playerCanvas.ChargingBarBackground;
            _chargingBarBackground.gameObject.SetActive(false);
            _chargingBarImage = _chargingBarBackground.GetComponentsInChildren<Image>()[1];
        }

        private void Update()
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

        private void LateUpdate()
        {
            _playerCanvas.transform.position = transform.position;
        }

        private void KeyDownAttack()
        {
            _attackDirection = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if(_tempBallObject == null)
            {
                AttackBulletInstate();

            }
            else if (_currentAtkCoolTime < _coolTime)
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
            if(_currentChargingTime <= _chargingMaxTimes[0])
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
            float bulletAngleDegree = Util.Math.GetDegree(transform.position, _attackDirection);
            BallMove Ball = Instantiate(_bulletObject).GetComponent<BallMove>();
            Ball.transform.position = transform.position;
            Ball.transform.rotation = Quaternion.Euler(0, 0, bulletAngleDegree);
            Ball.BulletSpeed = _bulletSpeed;
            Ball.IsShot = true;
            _tempBallObject = Ball.gameObject;
        }

        private void AttackBatSwing()
        {
            float rotationSpeed;
            _playerSystem.ChargeAtkPierce = ChargeAtkPierce.None;
            if (_isUpDown == true)
            {
                transform.localRotation = Quaternion.Euler(0f, 0f, -150f);
                rotationSpeed = Mathf.DeltaAngle(_batPos.localEulerAngles.z, -30f) / _rotationTime;
                StartCoroutine(BatSwing(_batPos,rotationSpeed, -30f));
            }
            else
            {
                transform.localRotation = Quaternion.Euler(0f, 0f, -30f);
                rotationSpeed = (Mathf.DeltaAngle(_batPos.localEulerAngles.z, -150f) / _rotationTime) * -1f;
                StartCoroutine(BatSwing(_batPos, rotationSpeed, -150f));
            }
            _isUpDown = !_isUpDown;
            _currentAtkCoolTime = 0f;
            if(!_chargingBarBackground.gameObject.activeSelf)
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
                    _playerSystem.ChargeAtkPierce = (ChargeAtkPierce)(1 << i);
                    _playerSystem.ChargeAtkShootEvent.Invoke(_atkShootSpd[i]);
                    _playerSystem.BatSwingRigidHitEvent.Invoke(_swingRigidDmg[i]);
                    break;
                }
            }
            _chargingBarBackground.gameObject.SetActive(false);
        }
        

        private IEnumerator BatSwing(Transform targetTransform,float rotateSpeed,float targetZ)
        {
            _trailRenderer.enabled = true;
            _batSwing.enabled = true;
            Quaternion targetRotation = Quaternion.Euler(0f, 0f, targetZ);
            float currentRotationTime = 0.0f;
            while(_rotationTime > currentRotationTime)
            {
                targetTransform.localRotation = Quaternion.RotateTowards(targetTransform.localRotation, targetRotation, rotateSpeed * Time.deltaTime);
                yield return null;
                currentRotationTime += Time.deltaTime;
            }
            targetTransform.localRotation = Quaternion.RotateTowards(targetTransform.localRotation, targetRotation, rotateSpeed * Time.deltaTime);
            yield return new WaitForSeconds(0.1f);
            _trailRenderer.enabled = false;
            //yield return new WaitForSeconds(0.23f);
            _batSwing.enabled = false;
        }

        private void BallObjectDestroyedChecking(GameObject gameObject)
        {
            //_tempBallObject = null;
        }

        //private void GrapCheck()
        //{
        //    if (_currentGrapCoolTime < _grapCoolTime)
        //        return;
        //    Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //    //Ray2D ray = new Ray2D(pos, Vector2.zero);
        //    int layerMask = 1 << LayerMask.NameToLayer("Grap");
        //    //layerMask = ~layerMask;
        //    RaycastHit2D hit = Physics2D.Raycast(pos, transform.forward, 30f, layerMask);
        //    Debug.DrawRay(pos, transform.forward * 30f, Color.red, 3f);
        //
        //    if (hit) //마우스 근처에 오브젝트가 있는지 확인
        //    {
        //        //있으면 오브젝트를 저장한다.
        //        GameObject target = hit.collider.gameObject;
        //        if (_grapRange < Vector2.Distance(target.transform.position , transform.position))
        //            return;
        //        PlayerGrapObject grapObject = target.GetComponent<PlayerGrapObject>();
        //        grapObject.StartPos = grapObject.transform.position;
        //        grapObject.EndPos = _armTransform;
        //        grapObject.GrapTime = _grapTotalTime;
        //        grapObject.gameObject.layer = 0;
        //    }
        //}


    }
}