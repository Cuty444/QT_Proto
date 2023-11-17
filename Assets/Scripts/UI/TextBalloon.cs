using System.Collections;
using System.Collections.Generic;
using QT.Util;
using TMPro;
using UnityEngine;

namespace QT
{
    public class TextBalloon : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _desc;
        
        [SerializeField] private TweenAnimator _descAnimation;
        [SerializeField] private TweenAnimator _releaseAnimation;

        public void Show(string text, float duration)
        {
            _releaseAnimation.Pause();
            _descAnimation.ReStart();
            _desc.DOTMP(text, duration);
        }
        
        public void Hide()
        {
            _descAnimation.Pause();
            _releaseAnimation.ReStart();
        }
        
    }
}
