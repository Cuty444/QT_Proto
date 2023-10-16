using QT.Core;
using QT.UI;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;
using UnityEngine.Video;

namespace QT.Tutorial
{
    public class TutorialCanvas : UIPanel
    {
        [field:SerializeField] public VideoClip[] VideoClips;
        [field:SerializeField] public RectTransform VideoParent;
        [field:SerializeField] public VideoPlayer VideoPlayer;
        
        [field:Space]
        [field:SerializeField] public HorizontalScrollSnap ScrollSnap { get; private set; }
        [field:SerializeField] public Button ExitButton;
        
        [field:Space]
        [field:SerializeField] public TweenAnimator PopAnimation { get; private set; }
    }


    public class TutorialCanvasModel : UIModelBase
    {
        public override UIType UIType => UIType.Popup;
        public override string PrefabPath => "Tutorial.prefab";
        
        private UIInputActions _inputActions;
        private TutorialCanvas _tutorialCanvas;
        
        public override void OnCreate(UIPanel view)
        {
            base.OnCreate(view);
            _tutorialCanvas = UIView as TutorialCanvas;
            
            _tutorialCanvas.ExitButton.onClick.AddListener(ReleaseUI);
            
            _inputActions = new UIInputActions();
            _inputActions.UI.Escape.started += (x) => ReleaseUI();
        }
        
        
        public override void Show()
        {
            base.Show();
            OnPageChanged(0);
            
            _tutorialCanvas.PopAnimation.ReStart();
        }
        
        
        public void OnPageChanged(int index)
        {  
            if (index < 0 || index >= _tutorialCanvas.VideoClips.Length || _tutorialCanvas.VideoClips[index] == null)
            {
                _tutorialCanvas.VideoParent.gameObject.SetActive(false);
                return;
            }
            
            _tutorialCanvas.ScrollSnap.CurrentPageObject(out var page);
            
            _tutorialCanvas.VideoParent.SetParent(page.transform);
            _tutorialCanvas.VideoParent.anchoredPosition = Vector3.zero;
            _tutorialCanvas.VideoParent.gameObject.SetActive(true);
            
            _tutorialCanvas.VideoPlayer.clip = _tutorialCanvas.VideoClips[index];
            _tutorialCanvas.VideoPlayer.Play();
        }
    }
}