using QT.Core;
using UnityEngine;
using UnityEngine.UI;
using QT.InGame;

namespace QT.UI
{
    public class BossHPCanvas : UIPanel
    {
        [field:SerializeField] public Image HpImage { get; private set; }
        
        [field:Space]
        [field:SerializeField] public TweenAnimator PopAnimator { get; private set; }
        [field:SerializeField] public TweenAnimator ReleaseAnimator { get; private set; }
        [field:SerializeField] public TweenAnimator HitAnimation { get; private set; }
    }
    
    
    public class BossHPCanvasModel : UIModelBase
    {
        public override UIType UIType => UIType.Popup;
        public override string PrefabPath => "BossHP.prefab";
        
        private BossHPCanvas _bossHpCanvas;

        
        public override void OnCreate(UIPanel view)
        {
            base.OnCreate(view);

            _bossHpCanvas = UIView as BossHPCanvas;
        }

        public override void Show()
        {
            base.Show();
            
            _bossHpCanvas.PopAnimator.ReStart();
        }

        public void SetHPGuage(Status hp)
        {
            _bossHpCanvas.HpImage.fillAmount = hp.StatusValue / hp.Value;
            
            if (_bossHpCanvas.HpImage.fillAmount <= 0)
            {
                _bossHpCanvas.ReleaseAnimator.ReStart();
            }
            else
            {
                _bossHpCanvas.HitAnimation.ReStart();
            }
        }
        
    }
}
