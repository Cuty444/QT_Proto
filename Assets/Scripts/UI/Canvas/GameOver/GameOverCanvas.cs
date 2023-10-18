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
    public class GameOverCanvas : UIPanel
    {
        [field:Space]
        [field:SerializeField] public Button RetryButton { get; private set; }
        [field:SerializeField] public Button TitleButton { get; private set; }
        
        [field:Space]
        [field:SerializeField] public SkeletonGraphic SpineAnimation { get; private set; }

        [field:Space]
        [field:SerializeField] public TweenAnimator PopAnimator { get; private set; }
        [field:SerializeField] public TweenAnimator ReleaseAnimator { get; private set; }
    }
    
    
    public class GameOverCanvasModel : UIModelBase
    {
        private readonly int ButtonCilckAnimation = Animator.StringToHash("OnClick");
        
        public override UIType UIType => UIType.Panel;
        public override string PrefabPath => "GameOver.prefab";
        
        private UIInputActions _inputActions;
        private GameOverCanvas _gameOverCanvas;
       
        
        public override void SetState(UIState state)
        {
            switch (state)
            {
                case UIState.GameOver:
                    Show();
                    break;
                default:
                    if (state != UIState.Loading)
                    {
                        ReleaseUI();
                    }

                    break;
            }
        }
        
        public override void OnCreate(UIPanel view)
        {
            base.OnCreate(view);
            _gameOverCanvas = UIView as GameOverCanvas;
            
            _gameOverCanvas.TitleButton.onClick.AddListener(OnClickExit);
            _gameOverCanvas.RetryButton.onClick.AddListener(OnClickRetry);
            
            _inputActions = new UIInputActions();
        }

        public override void Show()
        {
            if (_gameOverCanvas.gameObject.activeInHierarchy)
            {
                return;
            }
            
            SetInputs(true);
            _gameOverCanvas.StopAllCoroutines();

            base.Show();

            Time.timeScale = 0;

            _gameOverCanvas.ReleaseAnimator.Pause();
            _gameOverCanvas.PopAnimator.ReStart();

            _gameOverCanvas.SpineAnimation.AnimationState.SetAnimation(1, "over_Start", false);
            ProjectileManager.Instance.Clear();
            HitAbleManager.Instance.Clear();
        }

        public override void ReleaseUI()
        {
            if (!_gameOverCanvas.gameObject.activeInHierarchy)
            {
                return;
            }

            base.ReleaseUI();
        }

        private async void OnClickRetry()
        {
            _gameOverCanvas.RetryButton.GetComponent<Animator>().SetTrigger(ButtonCilckAnimation);
            
            SetInputs(false);
            
            SystemManager.Instance.SoundManager.PlayOneShot(SystemManager.Instance.SoundManager.SoundData.UIGameStartSFX);
            
            _gameOverCanvas.PopAnimator.Pause();
            _gameOverCanvas.ReleaseAnimator.ReStart();
            //_gameOverCanvas.SpineAnimation.AnimationState.SetAnimation(1, "S_GameOver_Replay",false);
            
            await SystemManager.Instance.StageLoadManager.StageLoad(1);
            SystemManager.Instance.GetSystem<DungeonMapSystem>().DungenMapGenerate();
            
            var sequenceLength = _gameOverCanvas.ReleaseAnimator.SequenceLength;
            
            _gameOverCanvas.StopAllCoroutines();
            _gameOverCanvas.StartCoroutine(WaitForReleaseAnimation(sequenceLength,1));
        }
        
        private void OnClickExit()
        {
            _gameOverCanvas.TitleButton.GetComponent<Animator>().SetTrigger(ButtonCilckAnimation);
            
            SetInputs(false);
            
            SystemManager.Instance.SoundManager.PlayOneShot(SystemManager.Instance.SoundManager.SoundData.UIGameStartSFX);
            
            _gameOverCanvas.PopAnimator.Pause();
            _gameOverCanvas.ReleaseAnimator.ReStart();

            var sequenceLength = 0;//_gameOverCanvas.ReleaseAnimator.SequenceLength;
            
            _gameOverCanvas.StopAllCoroutines();
            _gameOverCanvas.StartCoroutine(WaitForReleaseAnimation(sequenceLength,2));
        }


        private void SetInputs(bool enable)
        {
            if(enable)
            {
                _inputActions.Enable();
            }
            else
            {
                _inputActions.Disable();
            }
            
            _gameOverCanvas.TitleButton.enabled = enable;
            _gameOverCanvas.RetryButton.enabled = enable;
        }

        private IEnumerator WaitForReleaseAnimation(float waitTime, int SceneNumber)
        {
            yield return new WaitForSecondsRealtime(waitTime);
            
            Time.timeScale = 1;
            SystemManager.Instance.PlayerManager.Reset();;
            SystemManager.Instance.LoadingManager.LoadScene(SceneNumber);
        }

    }
    
}
