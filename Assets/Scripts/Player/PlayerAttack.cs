using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QT.Core;
using QT.Core.Input;
using QT.Core.Player;
namespace QT.Player
{
    public class PlayerAttack : MonoBehaviour
    {
        private float _coolTime;
        private float _bulletSpeed;
        [Header("Bullet")]
        [SerializeField] private GameObject _bulletObject;
        //[Header("Arm")]
        //[SerializeField] private Transform _armTransform;
        [SerializeField] private TrailRenderer _trailRenderer;
        [SerializeField] private Transform _batPos;
        [SerializeField] private BoxCollider2D _batBoxCollider2D;
        [SerializeField] private BatSwing _batSwing;
        [SerializeField] private float _rotationTime = 0.1f;
        private Vector2 _attackDirection;
        private float _currentCoolTime;
        private GameObject _tempBallObject = null;
        private bool _isUpDown = true;
        // Start is called before the first frame update
        void Start()
        {
            PlayerSystem playerSystem = SystemManager.Instance.GetSystem<PlayerSystem>();
            _bulletSpeed = playerSystem.BallTable.ThrowSpd;
            _coolTime = playerSystem.BatTable.AtkCooldown;
            InputSystem inputSystem = SystemManager.Instance.GetSystem<InputSystem>();
            inputSystem.OnKeyDownAttackEvent.AddListener(SetAttackDirection);
            //inputSystem.OnRightKeyDownGrapEvent.AddListener(GrapCheck);
            _currentCoolTime = _coolTime;
            _batSwing.enabled = false;
        }

        private void Update()
        {
            _currentCoolTime += Time.deltaTime;
            PlayerAngle();
        }


        private void SetAttackDirection(Vector2 dir)
        {
            _attackDirection = Camera.main.ScreenToWorldPoint(dir);
            AttackCheck();
        }

        private void AttackCheck()
        {
            if (_currentCoolTime < _coolTime)
                return;
            if(_tempBallObject == null)
            {
                AttackBulletInstate();

            }
            else
            {
                AttackBatSwing();
            }
        }

        private void AttackBulletInstate()
        {
            float bulletAngleRadian = Mathf.Atan2(_attackDirection.y - transform.position.y, _attackDirection.x - transform.position.x);
            float bulletAngleDegree = 180 / Mathf.PI * bulletAngleRadian;
            BallMove Ball = Instantiate(_bulletObject).GetComponent<BallMove>();
            Ball.transform.position = transform.position;
            Ball.transform.rotation = Quaternion.Euler(0, 0, bulletAngleDegree);
            Ball.BulletSpeed = _bulletSpeed;
            //Ball.BulletRange = _bulletRange;
            Ball.IsShot = true;
            _tempBallObject = Ball.gameObject;
            //_currentCoolTime = 0f;
        }

        private void AttackBatSwing()
        {
            float rotationSpeed;
            if (_isUpDown == true)
            {
                transform.localRotation = Quaternion.Euler(0f, 0f, -150f);
                rotationSpeed = Mathf.DeltaAngle(_batPos.localEulerAngles.z, -30f) / _rotationTime;
                StartCoroutine(BatSwing(_batPos,rotationSpeed, -30f, _batPos.localEulerAngles.z));
            }
            else
            {
                transform.localRotation = Quaternion.Euler(0f, 0f, -30f);
                rotationSpeed = (Mathf.DeltaAngle(_batPos.localEulerAngles.z, -150f) / _rotationTime) * -1f;
                StartCoroutine(BatSwing(_batPos, rotationSpeed, -150f, _batPos.localEulerAngles.z));
            }
            //float rotationSpeed = Mathf.DeltaAngle(_batPos.localEulerAngles.z, isUpDown ? -30f : -150f) / rotationTime;
            //_batPos.rotation = Quaternion.RotateTowards(_batPos.rotation, targetRoation, Time.deltaTime * _swingSpeed);
            _isUpDown = !_isUpDown;
            _currentCoolTime = 0f;
        }

        private void PlayerAngle()
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            float playerAngleRadian = Mathf.Atan2(mousePos.y - transform.position.y, mousePos.x - transform.position.x);
            float playerAngleDegree = 180 / Mathf.PI * playerAngleRadian;
            transform.rotation = Quaternion.Euler(0, 0, playerAngleDegree);
        }

        private IEnumerator BatSwing(Transform targetTransform,float rotateSpeed,float targetZ,float startZ)
        {
            _trailRenderer.enabled = true;
            _batSwing.enabled = true;
            Quaternion targetRotation = Quaternion.Euler(0f, 0f, targetZ);
            Quaternion targetStartRotation = Quaternion.Euler(0f, 0f, startZ);
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
            yield return new WaitForSeconds(0.23f);
            _batSwing.enabled = false;
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