using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace QT.InGame
{
    public class EnemyHPIndicator : MonoBehaviour
    {
        [SerializeField] private Image _hpImage;
        private Canvas _canvas;
        
        private void Awake()
        {
            _canvas = GetComponent<Canvas>();
        }

        private void OnEnable()
        {
            _canvas.worldCamera = Camera.main;
        }
        
        public void SetHP(Status hp)
        {
            _hpImage.fillAmount = Util.Math.Remap(hp, hp.BaseValue, 0f);
            gameObject.SetActive(hp > 0);

        }
    }
}
