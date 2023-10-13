using System;
using System.Collections;
using System.Collections.Generic;
using QT.Core;
using UnityEngine;
using TMPro;

namespace QT
{
    public class UIItemDesc : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _name;
        [SerializeField] private TextMeshProUGUI _desc;
        [SerializeField] private TextMeshProUGUI _plusDesc;
        [SerializeField] private TextMeshProUGUI _minusDesc;
        
        [SerializeField] private TextMeshProUGUI _goldCost;

        [SerializeField] private TweenAnimator _descAnimation;
        [SerializeField] private TweenAnimator _failButtonAnimation;
        
        
        private LocaleGameDataBase _localeGameDataBase;
        
        private ItemGameData _itemGameData;
        
        
        public void SetData(ItemGameData itemData)
        {
            _itemGameData = itemData;

            if(_goldCost != null)
                _goldCost.text = itemData.CostGold.ToString();
            
            SetText(_localeGameDataBase);
        }

        private void Awake()
        {
            _localeGameDataBase = SystemManager.Instance.DataManager.GetDataBase<LocaleGameDataBase>();
        }

        private void OnEnable()
        {
            _localeGameDataBase.OnLocaleChanged.AddListener(SetText);
        }

        private void OnDisable()
        {
            _localeGameDataBase.OnLocaleChanged.RemoveListener(SetText);
        }

        private void SetText(LocaleGameDataBase localeGameDataBase)
        {
            if (_itemGameData != null)
            {
                _name.text = _itemGameData.Name;
                _desc.text = _itemGameData.Desc;

                _plusDesc.text = _itemGameData.PlusDesc;
                _plusDesc.gameObject.SetActive(!string.IsNullOrWhiteSpace(_itemGameData.PlusDesc));

                _minusDesc.text = _itemGameData.MinusDesc;
                _minusDesc.gameObject.SetActive(!string.IsNullOrWhiteSpace(_itemGameData.MinusDesc));
            }
        }
        
        public void Show()
        {
            _descAnimation.ReStart();
        }

        public void Hide()
        {
            _descAnimation.Reset();
        }

        public void PlayFailButtonAnimation()
        {
            _failButtonAnimation?.ReStart();
        }
    }
}