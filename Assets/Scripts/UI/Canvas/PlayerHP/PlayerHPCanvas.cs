using System;
using System.Collections;
using System.Collections.Generic;
using QT.Core;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using QT.InGame;

namespace QT.UI
{
    public class PlayerHPCanvas : UIPanel
    {
        [field:SerializeField] public Transform PlayerHpTransform { get; private set; }
        [field:SerializeField] public GameObject PlayerHpObject { get; private set; }
        [field:SerializeField] public Sprite[] PlayerHpImage { get; private set; }

        [field:Space]
        [field:SerializeField] public GameObject ActiveCoolTimeObject { get; private set; }
        [field:SerializeField] public GameObject ActiveGlowObject { get; private set; }
        [field:SerializeField] public Image ActiveImage { get; private set; }
        [field:SerializeField] public Image ActiveCoolDownImage { get; private set; }
        
        [field:Space]
        [field:SerializeField] public TextMeshProUGUI GoldCostText { get; private set; }
        [field:SerializeField] public TweenAnimator GoldAnimation { get; private set; }
    }

    public class PlayerHPCanvasModel : UIModelBase
    {
        public override UIType UIType => UIType.Panel;
        public override string PrefabPath => "PlayerHP.prefab";

        private PlayerHPCanvas _playerHPCanvas;

        public override void SetState(UIState state)
        {
            switch (state)
            {
                case UIState.InGame:
                    Show();
                    break;
                default:
                    ReleaseUI();
                    break;
            }
        }

        public override void OnCreate(UIPanel view)
        {
            base.OnCreate(view);
            
            _playerHPCanvas = UIView as PlayerHPCanvas;
        }
        
        
        private List<Image> _hpIcons = new ();
        private int _lastActiveId;

        public void UpdateInfo(Status hp, Item activeItem)
        {
            SetHp(hp);

            CheckCoolDown(activeItem);
        }

        private void SetHp(Status hp)
        {
            var maxIconCount = Mathf.Max(hp.BaseValue / 50, _hpIcons.Count);
            
            int checkHp = 0;
            for (int i = 0; i < maxIconCount; i++)
            {
                if (i >= _hpIcons.Count)
                {
                    _hpIcons.Add(GameObject.Instantiate(_playerHPCanvas.PlayerHpObject, _playerHPCanvas.PlayerHpTransform).GetComponent<Image>());
                }

                if (checkHp > hp.Value)
                {
                    _hpIcons[i].gameObject.SetActive(false);
                    continue;
                }

                _hpIcons[i].gameObject.SetActive(true);


                int value = 2;

                var compare = hp.StatusValue - checkHp;
                if (compare >= 50)
                {
                    value = 0;
                }
                else if (compare >= 25)
                {
                    value = 1;
                }

                _hpIcons[i].sprite = _playerHPCanvas.PlayerHpImage[value];
                
                checkHp += 50;
            }
        }
        
        
        private async void CheckCoolDown(Item activeItem)
        {
            _playerHPCanvas.ActiveCoolTimeObject.SetActive(activeItem != null);
            if (activeItem != null)
            {
                var data = activeItem.ItemGameData;
                if (_lastActiveId != data.Index)
                {
                    _playerHPCanvas.ActiveImage.sprite = await SystemManager.Instance.ResourceManager.LoadAsset<Sprite>(data.ItemIconPath, true);
                }
                
                float coolTime = activeItem.GetCoolTimeProgress();
                _playerHPCanvas.ActiveCoolDownImage.fillAmount = coolTime;
                
                _playerHPCanvas.ActiveGlowObject.SetActive(coolTime <= 0);
                _lastActiveId = activeItem.ItemGameData.Index;
            }
        }
        


        public void SetGoldText(int gold)
        {
            var str = gold.ToString();

            if (str != _playerHPCanvas.GoldCostText.text)
            {
                _playerHPCanvas.GoldCostText.text = str;
                _playerHPCanvas.GoldAnimation.ReStart();
            }
        }
    }
    
}