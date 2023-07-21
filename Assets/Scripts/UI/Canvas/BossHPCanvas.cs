using QT.Core;
using UnityEngine;
using UnityEngine.UI;
using QT.InGame;

namespace QT.UI
{
    public class BossHPCanvas : UIPanel
    {
        [SerializeField] private Image _hpImage;
        
        [SerializeField] private UITweenAnimator _popAnimation;
        [SerializeField] private UITweenAnimator _releaseAnimation;
        [SerializeField] private UITweenAnimator _hitAnimation;

        public override void OnOpen()
        {
            base.OnOpen();
            SystemManager.Instance.SoundManager.PlayBGM(SystemManager.Instance.SoundManager.SoundData.BossStageBGM);
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
