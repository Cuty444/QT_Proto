using QT.Core;
using UnityEngine;
using UnityEngine.UI;
using QT.InGame;

namespace QT.UI
{
    public class BossHPCanvas : UIPanel
    {
        [SerializeField] private Image _hpImage;
        
        [SerializeField] private TweenAnimator _popAnimation;
        [SerializeField] private TweenAnimator _releaseAnimation;
        [SerializeField] private TweenAnimator _hitAnimation;

        public override void OnOpen()
        {
            base.OnOpen();
            _popAnimation.ReStart();
        }
        
        
        public void SetHPGuage(Status hp)
        {
            _hpImage.fillAmount = hp.StatusValue / hp.Value;
            
            if (_hpImage.fillAmount <= 0)
            {
                _releaseAnimation.ReStart();
            }
            else
            {
                _hitAnimation.ReStart();
            }
        }
    }
}