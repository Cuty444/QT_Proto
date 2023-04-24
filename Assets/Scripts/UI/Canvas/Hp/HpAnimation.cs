using System.Collections;
using System.Collections.Generic;
using QT.Util;
using Spine.Unity;
using UnityEngine;
using UnityEngine.UI;

namespace QT
{
    public class HpAnimation : MonoBehaviour
    {
        [SerializeField] private Image _image;
        [SerializeField] private SkeletonGraphic _skeletonGraphic;
        private bool _isFirstAnimation;

        public void StartAni()
        {
            if (_isFirstAnimation)
                return;
            _skeletonGraphic.enabled = true;
            _image.enabled = false;
            
            StartCoroutine(UnityUtil.WaitForFunc(() =>
            {
                gameObject.SetActive(false);
                _image.enabled = true;
            }, 0.5f));
        }

    }
}
