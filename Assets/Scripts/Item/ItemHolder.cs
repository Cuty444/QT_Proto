using System;
using System.Collections;
using System.Collections.Generic;
using QT.Core;
using QT.UI;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace QT
{
    public class ItemHolder : MonoBehaviour
    {
        private readonly int AnimationExitHash = Animator.StringToHash("Exit");
        
        
        public DropGameType DropType;
        
        public ItemGameData ItemGameData { get; private set; }
        
        [SerializeField] private Image _iconImage;

        [SerializeField] private Animator _alterAnimator;
        [SerializeField] private GameObject _soldObject;
        [SerializeField] private Collider2D[] _colliders;

        [SerializeField] private UIItemDesc _itemDesc;

        private PlayerManager _playerManager;
        private UnityAction _onGainItem;

        private bool _used;


        public void Init(ItemGameData itemGameData, UnityAction onGainItem = null)
        {
            ItemGameData = itemGameData;
            _onGainItem = onGainItem;
            
            SystemManager.Instance.ResourceManager.LoadUIImage(ItemGameData.ItemIconPath, _iconImage);

            switch (DropType)
            {
                case DropGameType.Start:
                    _alterAnimator.gameObject.SetActive(true);
                    break;
                default:
                    _alterAnimator.gameObject.SetActive(false);
                    _soldObject.SetActive(false);
                    break;
            }

            SetColliders(true);
            
            _itemDesc.SetData(itemGameData);
            
            _used = false;
        }

        private void Awake()
        {
            _playerManager = SystemManager.Instance.PlayerManager;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Player") ||
                other.gameObject.layer == LayerMask.NameToLayer("PlayerDodge"))
            {
                _itemDesc.Show();
                
                _playerManager.PlayerItemInteraction.AddListener(GainItem);
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Player") ||
                other.gameObject.layer == LayerMask.NameToLayer("PlayerDodge"))
            {
                _itemDesc.Hide();
                ClearItem();
            }
        }

        private void OnDisable()
        {
            ClearItem();
        }

        private void GainItem()
        {
            if (_used)
            {
                ClearItem();
                return;
            }
            
            if (DropType == DropGameType.Shop)
            {
                if (_playerManager.Gold < ItemGameData.CostGold)
                {
                    SystemManager.Instance.SoundManager.PlayOneShot(SystemManager.Instance.SoundManager.SoundData.Shop_BuyErrorSFX);
                    _itemDesc.PlayFailButtonAnimation();
                    
                    return;
                }
            }

            if (ItemGameData.GradeType == ItemGameData.GradeTypes.Active && _playerManager.Player.Inventory.ActiveItem != null)
            {
                var playerActive = _playerManager.Player.Inventory.ActiveItem.ItemGameData;
                if (playerActive == ItemGameData)
                {
                    _itemDesc.PlayFailButtonAnimation();
                    return;
                }
                
                SystemManager.Instance.UIManager.GetUIPanel<UIActiveItemSelectCanvas>().Show(playerActive, ItemGameData, GainItem);
            }
            else
            {
                GainItem(ItemGameData);
            }
        }
        
        private void GainItem(ItemGameData itemGameData)
        {
            if (_used) return;
            if(itemGameData != ItemGameData) return;
            
            if (DropType == DropGameType.Shop)
            {
                _playerManager.OnGoldValueChanged.Invoke(-ItemGameData.CostGold);
                SystemManager.Instance.SoundManager.PlayOneShot(SystemManager.Instance.SoundManager.SoundData
                    .Shop_BuySFX);

                _soldObject.SetActive(true);
            }
            
            SystemManager.Instance.SoundManager.PlayOneShot(SystemManager.Instance.SoundManager.SoundData.Item_GetSFX);
            
            _playerManager.GainItemSprite.Invoke(_iconImage.sprite);
            _playerManager.Player.Inventory.AddItem(ItemGameData.Index);
            
            _onGainItem?.Invoke();
            
            _iconImage.gameObject.SetActive(false);
            SetColliders(false);
            
            _used = true;
        }
        
        private void ClearItem()
        {
            _playerManager.PlayerItemInteraction.RemoveListener(GainItem);
        }

        private void SetColliders(bool active)
        {
            foreach (var collider in _colliders)
            {
                collider.enabled = active;
            }
        }
        
        public void EndAnimation()
         {
             _alterAnimator.SetTrigger(AnimationExitHash);
             
             _iconImage.gameObject.SetActive(false);
             SetColliders(false);
             ClearItem();
         }

    }
}
