using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QT
{
    public class NpcTextPopup : MonoBehaviour
    {
        [SerializeField] private TweenAnimator _descAnimation;

        private void Awake()
        {
            Hide();
        }

        public void Show()
        {
            _descAnimation.ReStart();
        }

        public void Hide()
        {
            _descAnimation.Reset();
        }
    }
}
