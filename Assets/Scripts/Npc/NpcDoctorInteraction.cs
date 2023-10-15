using System;
using System.Collections;
using System.Collections.Generic;
using QT.Core;
using UnityEngine;
using QT.Util;

namespace QT
{
    public class NpcDoctorInteraction : MonoBehaviour, IHitAble
    {
        private readonly int AnimationHitHash = Animator.StringToHash("Hit");
        private readonly int AnimationShiverHash = Animator.StringToHash("Shiver");
        private readonly int AnimationShiverStopHash = Animator.StringToHash("ShiverStop");
        private readonly int AnimationIdleHash = Animator.StringToHash("Idle");
        private readonly int AnimationTalkHash = Animator.StringToHash("Talk");

        public int InstanceId => gameObject.GetInstanceID();
        public Vector2 Position => transform.position;
        [field: SerializeField] public float ColliderRad { get; private set; }
        public bool IsClearTarget => false;
        public bool IsDead => false;

        [SerializeField] private Transform[] _waypointsTransform;
        [SerializeField] private float _moveSpeed;
        [SerializeField] private NpcTextPopup _npcTextPopup;
        
        private Animator _animator = null;

        private PlayerManager _playerManager;

        private bool _isMove;
        private int _currentWayPointIndex = 1;

        private bool _isHeal;
        private void Start()
        {
            _animator = GetComponentInChildren<Animator>();
            _playerManager = SystemManager.Instance.PlayerManager;
            _animator.SetTrigger(AnimationShiverHash);
            _playerManager.PlayerMapClearPosition.AddListener(StopShiver);
        }

        private void OnEnable()
        {
            HitAbleManager.Instance.Register(this);
        }

        private void OnDisable()
        {
            HitAbleManager.Instance.UnRegister(this);
            if (_animator == null)
                return;
            if (_currentWayPointIndex < _waypointsTransform.Length)
            {
                _isMove = true;
            }
        }
        
        private void Hit()
        {
            _animator.SetTrigger(AnimationHitHash);
            StartCoroutine(UnityUtil.WaitForFunc(() =>
            {
                _animator.ResetTrigger(AnimationHitHash);
            }, 0.1f));
        }
        
        public void Hit(Vector2 dir, float power,AttackType attackType)
        {
            Hit();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (_isHeal)
                return;
            if (other.gameObject.layer == LayerMask.NameToLayer("Player") ||
                other.gameObject.layer == LayerMask.NameToLayer("PlayerDodge"))
            {
                _npcTextPopup.Show();
                _playerManager.PlayerItemInteraction.AddListener(Heal);
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (_isHeal)
                return;
            if (other.gameObject.layer == LayerMask.NameToLayer("Player") ||
                other.gameObject.layer == LayerMask.NameToLayer("PlayerDodge"))
            {
                _npcTextPopup.Hide();
                if (!_isHeal)
                {
                    _playerManager.PlayerItemInteraction.RemoveListener(Heal);
                }
            }
        }
        
        private void StopShiver(Vector2Int position)
        {
            _animator.SetTrigger(AnimationShiverStopHash);
            _playerManager.PlayerMapClearPosition.RemoveListener(StopShiver);
            StartCoroutine(UnityUtil.WaitForFunc(MoveWayPoint,2.667f));
        }

        private void MoveWayPoint()
        {
            _isMove = true;
        }

        private void Update()
        {
            if (_isMove)
            {
                if (_currentWayPointIndex < _waypointsTransform.Length)
                {
                    transform.position = Vector2.MoveTowards(transform.position,
                        _waypointsTransform[_currentWayPointIndex].position, _moveSpeed * Time.deltaTime);
                    if (Vector2.Distance(_waypointsTransform[_currentWayPointIndex].position, transform.position) == 0f)
                    {
                        _currentWayPointIndex++;
                    }
                }
                else
                {
                    _isMove = false;
                    _animator.SetBool(AnimationIdleHash,true);

                }
            }
        }

        private void Heal()
        {
            _isHeal = true;
            _npcTextPopup.Hide();
            _animator.SetTrigger(AnimationTalkHash);
            _playerManager.Player.Heal(50f);
            _playerManager.AddItemEvent.Invoke();
            _playerManager.PlayerItemInteraction.RemoveListener(Heal);
        }
    }
}
