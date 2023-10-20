using System.Collections;
using QT.Core;
using UnityEngine;
using UnityEngine.UI;

namespace QT.UI
{
    public class EndingCanvas : UIPanel
    {
        [field:SerializeField] public Button FinishButton { get; private set; }
        [field:Space]
        [field:SerializeField] public TweenAnimator PopAnimator { get; private set; }
        [field:SerializeField] public TweenAnimator ReleaseAnimator { get; private set; }
    }
    
    
    public class EndingCanvasModel : UIModelBase
    {
        public override UIType UIType => UIType.Popup;
        public override string PrefabPath => "Ending.prefab";
        
        private UIInputActions _inputActions;
        private EndingCanvas _endingCanvas;

        
        public override void OnCreate(UIPanel view)
        {
            base.OnCreate(view);
            _endingCanvas = UIView as EndingCanvas;
            
            _endingCanvas.FinishButton.onClick.AddListener(OnClickFinishButton);
        }

        public override void Show()
        {
            base.Show();
            SystemManager.Instance.SoundManager.PlayBGM(SystemManager.Instance.SoundManager.SoundData.ClearBGM);

            Time.timeScale = 0;

            ProjectileManager.Instance.Clear();
            HitAbleManager.Instance.Clear();
        }

        private void OnClickFinishButton()
        {
            Time.timeScale = 1;
            
            ReleaseUI();
            
            SystemManager.Instance.PlayerManager.Reset();;
            SystemManager.Instance.LoadingManager.LoadScene(2);
        }
        
    }
    
}
