using System;
using System.Collections;
using System.Collections.Generic;
using QT.Core;
using TMPro;
using UnityEngine;

namespace QT
{
    public class UILocalApplier : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI Text;
        public string StringKey;
        
        private LocaleGameDataBase _localeGameDataBase;
        
        private void Awake()
        {
            _localeGameDataBase = SystemManager.Instance.DataManager.GetDataBase<LocaleGameDataBase>();
        }

        private void OnEnable()
        {
            OnLocaleChanged(_localeGameDataBase);
            _localeGameDataBase.OnLocaleChanged.AddListener(OnLocaleChanged);
        }

        private void OnDisable()
        {
            _localeGameDataBase.OnLocaleChanged.RemoveListener(OnLocaleChanged);
        }
        
        private void OnLocaleChanged(LocaleGameDataBase localeGameDataBase)
        {
            Text.text = localeGameDataBase.GetString(StringKey);
        }
    }
}
