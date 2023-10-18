using System.Collections;
using QT.Core;
using UnityEngine;

namespace QT.UI
{
    public class EndingCanvas : UIPanel
    {
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
        }

        public override void Show()
        {
            base.Show();

            Time.timeScale = 0;

            ProjectileManager.Instance.Clear();
            HitAbleManager.Instance.Clear();

            _endingCanvas.StopAllCoroutines();
            _endingCanvas.StartCoroutine(WaitForReleaseAnimation(3,2));
        }


        private IEnumerator WaitForReleaseAnimation(float waitTime, int SceneNumber)
        {
            yield return new WaitForSecondsRealtime(waitTime);
            
            ReleaseUI();
            
            Time.timeScale = 1;
            
            SystemManager.Instance.PlayerManager.Reset();;
            SystemManager.Instance.LoadingManager.LoadScene(SceneNumber);
        }

    }
    
}
