using System.Collections;
using Cysharp.Threading.Tasks;
using Spine.Unity;
using QT.Core;
using QT.Util;
using QT.Core.Map;
using UnityEngine;
using UnityEngine.UI;

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
        private GameOverCanvas _gameOverCanvas;

        public override void OnCreate(UIPanel view)
        {
            base.OnCreate(view);
            _gameOverCanvas = UIView as GameOverCanvas;
        }

        public override void Show()
        {
            if (_gameOverCanvas.gameObject.activeInHierarchy)
            {
                return;
            }
            
            base.Show();

            Time.timeScale = 0;

            ProjectileManager.Instance.Clear();
            HitAbleManager.Instance.Clear();

            _gameOverCanvas.StopAllCoroutines();
            _gameOverCanvas.StartCoroutine(WaitForReleaseAnimation(3,2));
        }


        private IEnumerator WaitForReleaseAnimation(float waitTime, int SceneNumber)
        {
            yield return new WaitForSecondsRealtime(waitTime);
            
            Time.timeScale = 1;
            SystemManager.Instance.LoadingManager.LoadScene(SceneNumber);
        }

    }
    
}
