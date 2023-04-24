using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using QT.UI;
using QT.Util;
using Spine.Unity;

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
        public Image PlayerHPImage => _playerHPImage;
        public Image PlayerInvicibleImage => _playerInvicibleImage;
        public Image PlayerDodgeCoolBarImage => _playerDodgeCoolBarImage;
        public Image PlayerDodgeCoolBackgroundImage => _playerDodgeCoolBackgroundImage;

        private List<Image> _playerHpList = new List<Image>();

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
            //int checkHp = 50;
            //for (int i = 0; i < _playerHpList.Count; i++)
            //{
            //    int value = 0;
            //    if (checkHp > hp.StatusValue)
            //    {
            //        value = 2;
            //        if (checkHp - 25 <= hp.StatusValue)
            //        {
            //            value = 1;
            //        }
            //    }
            //    ImageChange(_playerHpList[i], value);
            //    checkHp += 50;
            //}
        }

        public void ImageChange(Image image,int value)
        {
            image.sprite = _playerHpImage[value];
            if(value == 2)
                image.GetComponentInChildren<HpAnimation>()?.StartAni();
        }

        public void ThrowProjectileGauge(bool isActive)
        {
            if (isActive)
            {
                _skeletonGraphicRecharge.enabled = true;
                _skeletonGraphicRecharge.AnimationState.SetAnimation(1, "animation",false);
                StartCoroutine(UnityUtil.WaitForFunc(() =>
                {
                    _skeletonGraphicRecharge.enabled = false;
                },0.43f));
            }
            _playerBallStackObject.SetActive(isActive);
        }
        

    }

}