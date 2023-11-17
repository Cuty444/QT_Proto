using System;
using System.Collections;
using System.Collections.Generic;
using QT.Core;
using QT.Util;
using Spine.Unity;
using UnityEngine;

namespace QT
{
    public class StoreNpcInteraction : MonoBehaviour, IHitAble
    {
        private readonly int AnimationHitHash = Animator.StringToHash("Hit");
        private readonly int AnimationSoldHash = Animator.StringToHash("Sold");
        [SerializeField] private UIItemDesc _uiItemDesc;

        public int InstanceId => gameObject.GetInstanceID();
        public Vector2 Position => transform.position;
        [field: SerializeField] public float ColliderRad { get; private set; }
        public bool IsClearTarget => false;
        public bool IsDead => false;

        private Animator _animator;

        private ShopMapData _shopMapData;

        private PlayerManager _playerManager;

        private void Awake()
        {
            _animator = GetComponentInChildren<Animator>();
            _shopMapData = GetComponentInParent<ShopMapData>();
            _uiItemDesc.SetGoldCost("X " + _shopMapData.GetReRollGold());
            SystemManager.Instance.PlayerManager.AddItemEvent.AddListener(Sold);
            _shopMapData._rerollEvnet.AddListener(Sold);
            _playerManager = SystemManager.Instance.PlayerManager;
            _uiItemDesc.Hide();
        }

        private void OnEnable()
        {
            HitAbleManager.Instance.Register(this);
        }

        private void OnDisable()
        {
            HitAbleManager.Instance.UnRegister(this);
        }

        private void OnDestroy()
        {
            SystemManager.Instance?.PlayerManager.AddItemEvent.RemoveListener(Sold);
            _shopMapData?._rerollEvnet.RemoveListener(Sold);
        }

        private void Hit()
        {
            _animator.SetTrigger(AnimationHitHash);
            StartCoroutine(UnityUtil.WaitForFunc(() =>
            {
                _animator.ResetTrigger(AnimationHitHash);
            }, 0.1f));
        }

        private void Sold()
        {
            if (!gameObject.activeInHierarchy)
            {
                return;
            }
            
            _animator.SetTrigger(AnimationSoldHash);
            StartCoroutine(UnityUtil.WaitForFunc(() =>
            {
                _animator.ResetTrigger(AnimationSoldHash);
            }, 0.1f));
        }
        
        public void Hit(Vector2 dir, float power,AttackType attackType)
        {
            Hit();
        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Player") ||
                other.gameObject.layer == LayerMask.NameToLayer("PlayerDodge"))
            {
                _playerManager.PlayerItemInteraction.AddListener(ReRoll);
                _uiItemDesc.Show();
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Player") ||
                other.gameObject.layer == LayerMask.NameToLayer("PlayerDodge"))
            {
                _playerManager.PlayerItemInteraction.RemoveListener(ReRoll);
                _uiItemDesc.Hide();
            }
        }

        private void ReRoll()
        {
            int price = _shopMapData.GetReRollGold();
            if (price > _playerManager.Gold)
                return;
            _playerManager.OnGoldValueChanged.Invoke(-price);
            _shopMapData._rerollEvnet.Invoke();
            _uiItemDesc.SetGoldCost("X " + _shopMapData.GetReRollGold());
        }
    }
}
