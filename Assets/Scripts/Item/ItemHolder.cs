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
        private const string AltarDustEffectPath = "Effect/Prefabs/FX_Altar.prefab";
        private const string InitDustEffectPath = "Effect/Prefabs/FX_Ppyong_Dust.prefab";
        private readonly int AnimationExitHash = Animator.StringToHash("Exit");
        
        
        public DropGameType DropType;
        
        public ItemGameData ItemGameData { get; private set; }
        
        [SerializeField] private GameObject _iconObject;
        [SerializeField] private SpriteRenderer _iconImage;
        [SerializeField] private SpriteRenderer _activeIconImage;
        [SerializeField] private SpriteRenderer _frameImage;
        
[Header("프레임 스프라이트")]
        [SerializeField] private Sprite _frameNormal;
        [SerializeField] private Sprite _frameRare;
        [SerializeField] private Sprite _frameCursed;
        

        [SerializeField] private TweenAnimator _alterAnimator;
        [SerializeField] private GameObject _soldObject;
        [SerializeField] private Collider2D[] _colliders;

        [SerializeField] private UIItemDesc _itemDesc;

        private PlayerManager _playerManager;
        private UnityAction _onGainItem;

        private bool _used;
        private bool _isFirst = false;
        

        public void Init(ItemGameData itemGameData, UnityAction onGainItem = null)
        {
            ItemGameData = itemGameData;
            _onGainItem = onGainItem;

            if (ItemGameData.GradeType == ItemGameData.GradeTypes.Active)
            {
                SystemManager.Instance.ResourceManager.LoadSpriteRenderer(ItemGameData.ItemIconPath, _activeIconImage);
                _activeIconImage.gameObject.SetActive(true);
                _iconImage.gameObject.SetActive(false);
                _frameImage.gameObject.SetActive(false);
            }
            else
            {
                _activeIconImage.gameObject.SetActive(false);
                _iconImage.gameObject.SetActive(true);
                _frameImage.gameObject.SetActive(true);
                SystemManager.Instance.ResourceManager.LoadSpriteRenderer(ItemGameData.ItemIconPath, _iconImage);
                
                switch (ItemGameData.GradeType)
                {
                    case ItemGameData.GradeTypes.Cursed:
                        _frameImage.sprite = _frameCursed;
                        break;
                    case ItemGameData.GradeTypes.Rare:
                        _frameImage.sprite = _frameRare;
                        break;
                    default:
                        _frameImage.sprite = _frameNormal;
                        break;
                }
            }

            switch (DropType)
            {
                case DropGameType.Start:
                    _alterAnimator.gameObject.SetActive(true);
                    
                    _alterAnimator.ReStart();
                    SystemManager.Instance.ResourceManager.EmitParticle(AltarDustEffectPath, _alterAnimator.transform.position);
                    SystemManager.Instance.SoundManager.PlayOneShot(SystemManager.Instance.SoundManager.SoundData.Altar_AppearSFX);
                    break;
                default:
                    _soldObject.SetActive(false);
                    break;
            }

            if (DropType == DropGameType.Shop)
            {
                if (!_isFirst)
                {
                    _isFirst = true;
                    return;
                }
                SystemManager.Instance.ResourceManager.EmitParticle(InitDustEffectPath, _iconObject.transform.position);
            }
            SetColliders(true);
            
            _itemDesc.SetData(itemGameData);
            
            _used = false;
        }

        private void Awake()
        {
            _playerManager = SystemManager.Instance.PlayerManager;
            
            _itemDesc.Reset();
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

        private async void GainItem()
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

            // if (ItemGameData.GradeType == ItemGameData.GradeTypes.Active && _playerManager.Player.Inventory.ActiveItem != null)
            // {
            //     var playerActive = _playerManager.Player.Inventory.ActiveItem.ItemGameData;
            //     if (playerActive == ItemGameData)
            //     {
            //         _itemDesc.PlayFailButtonAnimation();
            //         return;
            //     }
            //     
            //     (await SystemManager.Instance.UIManager.Get<UIActiveItemSelectCanvasModel>()).Initialize(playerActive, ItemGameData, GainItem);
            // }
            //else
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
            
            _playerManager.Player.AddItem(ItemGameData);
            
            _onGainItem?.Invoke();
            
            _iconObject.gameObject.SetActive(false);
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
             _alterAnimator?.PlayBackwards();
             SystemManager.Instance.ResourceManager.EmitParticle(AltarDustEffectPath, _alterAnimator.transform.position);
             SystemManager.Instance.SoundManager.PlayOneShot(SystemManager.Instance.SoundManager.SoundData.Altar_AppearSFX);
             
             _iconObject.gameObject.SetActive(false);
             SetColliders(false);
             ClearItem();
         }

        public bool IsUsed()
        {
            return _used;
        }
    }
}
