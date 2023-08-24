using System;
using System.Collections;
using System.Collections.Generic;
using QT.Core;
using UnityEngine;
using UnityEngine.UI;
using QT.UI;
using QT.Util;
using Spine.Unity;
using TMPro;
using System.Linq;
using QT.InGame;
namespace QT.UI
{
    public class PlayerHPCanvas : UIPanel
    {
        [SerializeField] private Image _playerHPImage;
        [SerializeField] private Image _playerInvicibleImage;
        [SerializeField] private GameObject _playerBallStackObject;
        [SerializeField] private Image _playerDodgeCoolBarImage;
        [SerializeField] private Image _playerDodgeCoolBackgroundImage;
        [SerializeField] private Transform _playerHpTransform;
        [SerializeField] private GameObject _playerHpObject;
        [SerializeField] private Sprite[] _playerHpImage;
        [SerializeField] private SkeletonGraphic _skeletonGraphicRecharge;
        [SerializeField] private TextMeshProUGUI _goldCostText;

        [Space]
        [SerializeField] private GameObject _activeCoolTimeObject;
        [SerializeField] private Image _activeImage;
        [SerializeField] private Image _activeCoolDownImage;

        public Image PlayerHPImage => _playerHPImage;
        public Image PlayerInvicibleImage => _playerInvicibleImage;
        public Image PlayerDodgeCoolBarImage => _playerDodgeCoolBarImage;
        public Image PlayerDodgeCoolBackgroundImage => _playerDodgeCoolBackgroundImage;
        
        private List<Image> _playerHpList = new List<Image>();
        private int beforeHp = 0;
        [SerializeField] private UITweenAnimator _goldAnimation;

        private PlayerManager _playerManager;

        private int _lastActiveId;

        private void Start()
        {
            _playerManager = SystemManager.Instance.PlayerManager;

            //playerManager.PlayerCreateEvent.AddListener((arg) =>
            //{
            //    arg.GetStat(PlayerStats.HP).OnValueChanged.AddListener(()=>SetHpUpdate(arg.GetStatus(PlayerStats.HP)));
            //    arg.GetStatus(PlayerStats.HP).OnStatusChanged.AddListener(()=>SetHpUpdate(arg.GetStatus(PlayerStats.HP)));
            //});
        }

        private void Update()
        {
            CheckCoolDown();
        }

        private async void CheckCoolDown()
        {
            var activeItem = _playerManager?.Player.Inventory.ActiveItem;
            
            _activeCoolTimeObject.SetActive(activeItem != null);
            if (activeItem != null)
            {
                var data = activeItem.ItemGameData;
                if (_lastActiveId != data.Index)
                {
                    _activeImage.sprite = await SystemManager.Instance.ResourceManager.LoadAsset<Sprite>(data.ItemIconPath, true);
                }
                
                float coolTime = activeItem.GetCoolTimeProgress();
                _activeCoolDownImage.fillAmount = coolTime;
                
                _lastActiveId = activeItem.ItemGameData.Index;
            }
        }

        public void SetHp(Status hp)
        {
            for (int i = 0; i < _playerHpList.Count; i++)
            {
                ImageChange(_playerHpList[i],0);
            }
            for (int i = _playerHpList.Count * 25; i < hp.Value; i += 25)
            {
                _playerHpList.Add(Instantiate(_playerHpObject,_playerHpTransform).GetComponent<Image>());
            }

            if (_playerHpList.Count * 25 > hp.Value)
            {
                int index = (int) ((_playerHpList.Count * 25 - hp.Value) / 25);
                for (int i = 0; i < index; i++)
                {
                    Destroy(_playerHpList.Last().gameObject);
                    _playerHpList.Remove(_playerHpList.Last());
                }
            }

            beforeHp = (int)hp.Value;
        }

        public void SetHpUpdate(Status hp)
        {
            for (int i = _playerHpList.Count * 25; i < hp.Value; i += 25)
            {
                _playerHpList.Add(Instantiate(_playerHpObject,_playerHpTransform).GetComponent<Image>());
            }

            int beforeValue = beforeHp / 25;
            int afterValue = (int) (hp.Value / 25);
            if (beforeValue < afterValue)
            {
                hp.AddStatus((afterValue - beforeValue) * 25);
            }
            else if (beforeValue > afterValue)
            {
                int index = beforeValue-afterValue;
                for (int i = 0; i < index; i++)
                {
                    Destroy(_playerHpList.Last().gameObject);
                    _playerHpList.Remove(_playerHpList.Last());
                }
                hp.AddStatus(-(index * 25));
            }
            beforeHp = afterValue * 25;

            CurrentHpImageChange(hp);
        }
        
        public void CurrentHpImageChange(Status hp)
        { 
            int checkHp = 25;
            for (int i = 0; i < _playerHpList.Count; i++)
            {
                int value = 0;
                if (checkHp > hp.StatusValue)
                {
                    value = 2;
                }
                ImageChange(_playerHpList[i], value);
                checkHp += 25;
            }
        }

        public void ImageChange(Image image,int value)
        {
            image.sprite = _playerHpImage[value];
            //if(value == 2)
            //    image.GetComponentInChildren<HpAnimation>()?.StartAni();
        }

        public void ThrowProjectileGauge(bool isActive)
        {
            if (isActive)
            {
                //skeletonGraphicRecharge.enabled = true;
                //_skeletonGraphicRecharge.AnimationState.SetAnimation(1, "animation",false);
                //StartCoroutine(UnityUtil.WaitForFunc(() =>
                //{
                //    _skeletonGraphicRecharge.enabled = false;
                //},0.667f));
            }
            _playerBallStackObject.SetActive(isActive);
        }

        public void SetDodgeCoolTime(Status dodge)
        {
            var value = Util.Math.Remap(dodge.StatusValue, dodge.Value, 0f);
            
            _playerDodgeCoolBackgroundImage.enabled = value > 0;
            _playerDodgeCoolBarImage.fillAmount = value;
        }

        public void SetGoldText(int goldText)
        {
            var str = _playerManager.Gold.ToString();

            if (str != _goldCostText.text)
            {
                _goldCostText.text = str;
                _goldAnimation.ReStart();
            }
        }

        /// <summary>
        /// a 가 높은지
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        private bool FloatHighEqual(float a,float b)
        {
            if (a - 1f >= b || a + 1f >= b)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// a 가 낮은지
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        private bool FloatLowEqual(float a,float  b)
        {
            if (a - 1f <= b || a + 1 <= b)
            {
                return true;
            }

            return false;
        }
        
    }

}