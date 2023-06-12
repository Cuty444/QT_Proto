using System.Collections;
using System.Collections.Generic;
using QT.Core;
using QT.Ranking;
using QT.Tutorial;
using QT.Util;
using UnityEngine;

namespace QT.UI
{
    public class TitleCanvas : UIPanel
    {
        [SerializeField] private ButtonTrigger _startButton;
        [SerializeField] private ButtonTrigger _tutorialButton;
        [SerializeField] private ButtonTrigger _rankingButton;
        
        [SerializeField] private UITweenAnimator _popAnimation;
        private bool isFirst = false;
        public override void PostSystemInitialize()
        {
            OnOpen();
        }

        public override void OnOpen()
        {
            base.OnOpen();
            SystemManager.Instance.RankingManager.ResetRankingTime();
            SystemManager.Instance.RankingManager.PlayerOn.Invoke(false);
            //SystemManager.Instance.SoundManager.PlayBGM(SystemManager.Instance.SoundManager.SoundData.MainBGM);
            _startButton.InteractableOn();
            _tutorialButton.InteractableOn();
            _rankingButton.InteractableOn();
            if (!SystemManager.Instance.LoadingManager.IsJsonLoad())
            {
                SystemManager.Instance.LoadingManager.DataJsonLoadCompletedEvent.AddListener(() =>
                {
                    _popAnimation.ReStart();
                });
            }
            else
            {
                _popAnimation.ReStart();
            }
            if (!isFirst)
                isFirst = true;
            else
            {
                SystemManager.Instance.SoundManager.PlayBGM(SystemManager.Instance.SoundManager.SoundData.MainBGM);
            }
        }

        public void RestartAnimation()
        {
            _popAnimation.ReStart();
        }
        
        public void GameStart()
        {
            if (SystemManager.Instance.LoadingManager.IsJsonLoad())
            {
                SystemManager.Instance.SoundManager.PlayOneShot(SystemManager.Instance.SoundManager.SoundData.UIGameStartSFX);
                SystemManager.Instance.StageLoadManager.StageLoad(string.Empty);
                SystemManager.Instance.LoadingManager.LoadScene(1, OnClose);
            }
        }

        public void GameEnd()
        {
            QT.Util.UnityUtil.ProgramExit();
        }

        public void RankingOpen()
        {
            SystemManager.Instance.UIManager.GetUIPanel<RankingCanvas>().OnOpen();
        }

        public void RankignClose()
        {
            _rankingButton.InteractableOn();
        }

        public void TutorialOpen()
        {
            SystemManager.Instance.UIManager.GetUIPanel<TutorialCanvas>().OnOpen();
        }

        public void TutorialClose()
        {
            _tutorialButton.InteractableOn();
        }
    }
}
