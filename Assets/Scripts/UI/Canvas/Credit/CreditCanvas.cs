using System.Collections;
using QT.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace QT.UI
{
    public class CreditCanvas : UIPanel
    {
        [field:SerializeField] public Button FinishButton { get; private set; }
        [field:SerializeField] public float SkipIgnoreTime { get; private set; }
        
        [field:Space]
        [field:SerializeField] public TextMeshProUGUI Name { get; private set; }
        [field:Space]
        [field:SerializeField] public TweenAnimator PopAnimator { get; private set; }
        [field:SerializeField] public TweenAnimator ReleaseAnimator { get; private set; }
    }
    
    
    public class CreditCanvasModel : UIModelBase
    {
        public override bool UseStack => false;
        public override UIType UIType => UIType.Popup;
        public override string PrefabPath => "End_credits.prefab";
        
        private UIInputActions _inputActions;
        private CreditCanvas _creditCanvas;

        private float _startTime;
        
        public override void OnCreate(UIPanel view)
        {
            base.OnCreate(view);
            _creditCanvas = UIView as CreditCanvas;
            
            _creditCanvas.FinishButton.onClick.AddListener(OnClickFinishButton);
        }

        public override void Show()
        {
            base.Show();
            
            SystemManager.Instance.SoundManager.PlayBGM(SystemManager.Instance.SoundManager.SoundData.ClearBGM);

            _startTime = Time.unscaledTime;
            Time.timeScale = 0;

            ProjectileManager.Instance.Clear();
            HitAbleManager.Instance.Clear();

            _creditCanvas.Name.text = System.Environment.UserName;
            
            _creditCanvas.PopAnimator.ReStart();
        }

        private void OnClickFinishButton()
        {
            Time.timeScale = 1;
            
            if(_startTime + _creditCanvas.SkipIgnoreTime > Time.unscaledTime)
                return;
            
            ReleaseUI();
            
            SystemManager.Instance.PlayerManager.Reset();
            SystemManager.Instance.LoadingManager.LoadScene(SceneNumber.Title);
        }
        
    }
    
}
