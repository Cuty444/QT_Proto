using Cysharp.Threading.Tasks;
using QT.Core;
using QT.Core.Map;
using QT.Ranking;
using QT.Tutorial;
using QT.Util;
using UnityEngine;
using UnityEngine.UI;

namespace QT.UI
{
    public class TitleCanvas : UIPanel
    {
        [field:SerializeField] public Button StartButton { get; private set; }
        [field:SerializeField] public Button TutorialButton { get; private set; }
        [field:SerializeField] public Button RankingButton { get; private set; }
        [field:SerializeField] public Button ExitGameButton { get; private set; }
        
        [field:SerializeField] public TweenAnimator PopAnimation  { get; private set; }
    }

    public class TitleCanvasModel : UIModelBase
    {
        public override UIType UIType => UIType.Panel;
        public override string PrefabPath => "Title.prefab";

        private TitleCanvas _titleCanvas;
        
        
        public override void OnCreate(UIPanel view)
        {
            base.OnCreate(view);
            _titleCanvas = UIView as TitleCanvas;
            
            _titleCanvas.StartButton.onClick.AddListener(GameStart);
            _titleCanvas.TutorialButton.onClick.AddListener(TutorialOpen);
            _titleCanvas.RankingButton.onClick.AddListener(RankingOpen);
            _titleCanvas.ExitGameButton.onClick.AddListener(GameEnd);
        }
        
        public override void SetState(UIState state)
        {
            switch (state)
            {
                case UIState.Title:
                    Show();
                    break;
                default:
                    ReleaseUI();
                    break;
            }
        }
        
        public override void Show()
        {
            base.Show();
            
            SystemManager.Instance.RankingManager.ResetRankingTime();
            SystemManager.Instance.RankingManager.PlayerOn.Invoke(false);
            //SystemManager.Instance.SoundManager.PlayBGM(SystemManager.Instance.SoundManager.SoundData.MainBGM);

            WaitJsonLoad();
            
            SystemManager.Instance.SoundManager.PlayBGM(SystemManager.Instance.SoundManager.SoundData.MainBGM);
        }

        private async void WaitJsonLoad()
        {
            _titleCanvas.PopAnimation.Reset();
            
            _titleCanvas.StartButton.interactable = false;
            _titleCanvas.TutorialButton.interactable = false;
            _titleCanvas.RankingButton.interactable = false;
            
            await UniTask.WaitUntil(() => SystemManager.Instance.LoadingManager.IsJsonLoad());
            
            _titleCanvas.PopAnimation.ReStart();
            
            _titleCanvas.StartButton.interactable = true;
            _titleCanvas.TutorialButton.interactable = true;
            _titleCanvas.RankingButton.interactable = true;
        }
        
        private void GameStart()
        {
            if (SystemManager.Instance.LoadingManager.IsJsonLoad())
            {
                SystemManager.Instance.GetSystem<DungeonMapSystem>().DungenMapGenerate();
                
                SystemManager.Instance.SoundManager.PlayOneShot(SystemManager.Instance.SoundManager.SoundData.UIGameStartSFX);
                SystemManager.Instance.StageLoadManager.StageLoad("1");
                SystemManager.Instance.LoadingManager.LoadScene(1, ReleaseUI);
            }
        }

        private void RankingOpen()
        {
            SystemManager.Instance.UIManager.GetUIPanel<RankingCanvas>().OnOpen();
        }

        private void TutorialOpen()
        {
            SystemManager.Instance.UIManager.GetUIPanel<TutorialCanvas>().OnOpen();
        }
        
        private void GameEnd()
        {
            UnityUtil.ProgramExit();
        }
    }
}
