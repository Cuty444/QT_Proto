using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using QT.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace QT
{
    public class ItemObject : MonoBehaviour
    {
        public int ItemID;
        public DropGameType DropType;
        [SerializeField] private GameObject _itemScriptCanvas;
        [SerializeField] private SpriteRenderer _itemSprite;
        [SerializeField] private TextMeshProUGUI _itemName;
        [SerializeField] private TextMeshProUGUI _itemDesc;
        [SerializeField] private TextMeshProUGUI _itemCost;
        [SerializeField] private Transform _goldTransform;
        [SerializeField] private Transform _hpTransform;
        [SerializeField] private Transform _blankTransform;
        [SerializeField] private Image[] _hpImages;
        [SerializeField] private GameObject _altarObject;
        [SerializeField] private UITweenAnimator _popAnimation;
        [SerializeField] private SpriteRenderer _soldSprite;
        [SerializeField] private BoxCollider2D _boxCollider2D;
        [SerializeField] private BoxCollider2D _characterCollider2D;
        [SerializeField] private Transform _textPanel;
        [SerializeField] private GameObject _buyObject;
        [SerializeField] private Image _gainImage;
        [SerializeField] private Animator _buyFailAnimator;
        [SerializeField] private Animator _buyFailBackgroundAnimator;
        private PlayerManager _playerManager;
        [HideInInspector]public ItemSelectMapData _itemSelectMapData;
        public ItemGameData ItemGameData { get; private set; }
        public List<ItemEffectOld> ItemEffectData { get; private set; } = new ();

        private Animator _animator;
        private readonly int AnimationExitHash = Animator.StringToHash("Exit");
        private CircleCollider2D _altarItemCollider2D;

        private bool isAltar = false;
        private void Start()
        {
            _playerManager = SystemManager.Instance.PlayerManager;
            var dataManager = SystemManager.Instance.DataManager;
            _animator = _altarObject.GetComponent<Animator>();
            if (DropType == DropGameType.Shop /*|| DropType == DropGameType.HpShop*/)
            {
                ItemGameData = dataManager.GetDataBase<ItemGameDataBase>().GetData(ItemID);
                if (DropType == DropGameType.Shop)
                {
                    _goldTransform.gameObject.SetActive(true);
                    _hpTransform.gameObject.SetActive(false);
                }
                //else
                //{
                //    _goldTransform.gameObject.SetActive(false);
                //    _hpTransform.gameObject.SetActive(true);
                //}
                _buyFailAnimator.Play("ItemFailAnimation", -1, 1f);
                _buyFailBackgroundAnimator.Play("ItemFailBackGroundAnimation", -1, 1f);
                _altarObject.gameObject.SetActive(false);
                _buyObject.SetActive(true);
                _gainImage.gameObject.SetActive(false);
            }
            else if (DropType == DropGameType.Start || DropType == DropGameType.Select)
            {
                ItemGameData = dataManager.GetDataBase<ItemGameDataBase>().GetData(ItemID);
                _goldTransform.gameObject.SetActive(false);
                _hpTransform.gameObject.SetActive(false);
                _blankTransform.gameObject.SetActive(true);
                _buyObject.SetActive(false);
                _gainImage.gameObject.SetActive(true);
                Destroy(_boxCollider2D);
                _altarItemCollider2D = gameObject.AddComponent<CircleCollider2D>();
                _altarItemCollider2D.radius = 0.75f;
                _altarItemCollider2D.isTrigger = true;
                _altarItemCollider2D.offset = new Vector2(0f, 0.5f);
                isAltar = true;
                _itemSprite.enabled = false;
                SystemManager.Instance.SoundManager.PlayOneShot(SystemManager.Instance.SoundManager.SoundData.Altar_AppearSFX);
            }
            //else if (/*DropType == DropGameType.Boss || DropType == DropGameType.ItemReward ||*/
            //         DropType == DropGameType.Select)
            //{
            //    //var list = SystemManager.Instance.ItemDataManager.GetDropItemList(DropType, 1);
            //    //ItemGameData = dataManager.GetDataBase<ItemGameDataBase>().GetData(list[0]);
            //}
            if (ItemGameData != null)
            {
                ItemEffectData = dataManager.GetDataBase<ItemEffectGameDataBase>()
                    .GetData(ItemGameData.ItemEffectDataId);
            }

            ResourceManager resourceManager = SystemManager.Instance.ResourceManager;
            resourceManager.LoadSprite(ItemGameData.ItemIconPath, _itemSprite);
            _itemName.text = ItemGameData.Name;
            _itemCost.text = ItemGameData.CostGold.ToString();
            _itemDesc.text = DataStringChanger(DataStringChanger(ItemGameData.Desc,"20","11.5"),"16","11.5");
            for (int i = 25; i <= ItemGameData.CostHp; i += 25)
            {
                _hpImages[i/25].gameObject.SetActive(true);
            }
            LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)_itemScriptCanvas.transform);
            LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)_itemName.transform);
            LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)_itemDesc.transform);
            
            _popAnimation.PlayBackwards();
        }

        private void Update()
        {
            if (isAltar)
            {
                if (_animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.7f)
                {
                    _itemSprite.enabled = true;
                }
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Player") || other.gameObject.layer == LayerMask.NameToLayer("PlayerDodge"))
            {
                _popAnimation.ReStart();
                if (DropType == DropGameType.Shop /*|| DropType == DropGameType.HpShop*/)
                {
                    _playerManager.PlayerItemInteraction.AddListener(ItemBuy);
                }
                else if (DropType == DropGameType.Start || DropType == DropGameType.Select)
                {
                    _playerManager.PlayerItemInteraction.AddListener(ItemGain);
                }
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Player")|| other.gameObject.layer == LayerMask.NameToLayer("PlayerDodge"))
            {
                ItemClear();
            }
        }

        public void ItemBuy()
        {
            if (DropType == DropGameType.Shop)
            {
                if (!_playerManager.Player.GetGoldComparison(ItemGameData.CostGold))
                {
                    SystemManager.Instance.SoundManager.PlayOneShot(SystemManager.Instance.SoundManager.SoundData.Shop_BuyErrorSFX);
                    _buyFailAnimator.Play("ItemFailAnimation", -1, 0f);
                    _buyFailBackgroundAnimator.Play("ItemFailBackGroundAnimation", -1, 0f);
                    return;
                }
                _playerManager.OnGoldValueChanged.Invoke(_playerManager.Player.GetGoldCost() - ItemGameData.CostGold);
            }
            //else if(DropType == DropGameType.HpShop)
            //{
            //    if (!_playerManager.Player.GetHpComparision(ItemGameData.CostHp))
            //        return;
            //    _playerManager.OnDamageEvent.Invoke(Vector2.zero, ItemGameData.CostHp);
            //}

            _itemSprite.enabled = false;
            _soldSprite.enabled = true;
            _boxCollider2D.enabled = false;
            _textPanel.gameObject.SetActive(false);
            _playerManager.GainItemSprite.Invoke(_itemSprite.sprite);
            SystemManager.Instance.SoundManager.PlayOneShot(SystemManager.Instance.SoundManager.SoundData.Item_GetSFX);
            SystemManager.Instance.SoundManager.PlayOneShot(SystemManager.Instance.SoundManager.SoundData.Shop_BuySFX);
            _playerManager.Player.Inventory.AddItem(ItemID);
            //Destroy(gameObject);
        }

        public void ItemGain()
        {
            _itemSelectMapData.ItemSelectGainEnd();
            _textPanel.gameObject.SetActive(false);
            SystemManager.Instance.SoundManager.PlayOneShot(SystemManager.Instance.SoundManager.SoundData.Item_GetSFX);
            _playerManager.GainItemSprite.Invoke(_itemSprite.sprite);
            _playerManager.Player.Inventory.AddItem(ItemID);
        }

        public void EndAnimation()
        {
            _animator.SetTrigger(AnimationExitHash);
            isAltar = false;
            _itemSprite.enabled = false;
            _characterCollider2D.enabled = false;
            _altarItemCollider2D.enabled = false;
            ItemClear();
        }
        
        private void ItemClear()
        {
            _popAnimation.PlayBackwards();
            if (DropType == DropGameType.Shop /*|| DropType == DropGameType.HpShop*/)
            {
                _playerManager.PlayerItemInteraction.RemoveListener(ItemBuy);
            }
            else if (DropType == DropGameType.Start || DropType == DropGameType.Select)
            {
                _playerManager.PlayerItemInteraction.RemoveListener(ItemGain);
            }
        }
        
        private string DataStringChanger(string original, string before,string after)
        {
            List<StringBuilder> textList = new List<StringBuilder>();

            var temp = original.Split("<");
            for (int i = 0; i < temp.Length; i++)
            {
                if (temp[i].IndexOf("size=") >= 0)
                {
                    textList.Add(new StringBuilder("<"+temp[i].Replace(before, after)));
                }
                else
                {
                    if (i == 0)
                    {
                       textList.Add(new StringBuilder(temp[i]));
                    }
                    else
                    {
                       textList.Add(new StringBuilder("<"+temp[i]));
                    }
                }
            }

            StringBuilder finalText = new StringBuilder();
            for (int i = 0; i < textList.Count; i++)
            {
                finalText.Append(textList[i]);
            }
            return finalText.ToString();
        }
    }
}
