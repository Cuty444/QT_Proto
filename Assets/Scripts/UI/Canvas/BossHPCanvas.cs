using QT.Core;
using UnityEngine;
using UnityEngine.UI;

namespace QT.UI
{
    public class BossHPCanvas : UIPanel
    {
        [SerializeField] private Image _hpImage;
        
        private void OnEnable()
        {
            SystemManager.Instance.SoundManager.PlayBGM(SystemManager.Instance.SoundManager.SoundData.BossStageBGM);
        }
        
        public void SetHPGuage(Status hp)
        {
            _hpImage.fillAmount = hp.StatusValue / hp.Value;
        }
    }
}
