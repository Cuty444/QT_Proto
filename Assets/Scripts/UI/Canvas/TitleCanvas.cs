using System.Collections;
using System.Collections.Generic;
using QT.Core;
using QT.Core.Map;
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
            //if (!isFirst)
            //    isFirst = true; // TODO : 스플래쉬 씬에서 브금 2번 곂치는 형상을 위한 코드
            //else
            //{
                SystemManager.Instance.SoundManager.PlayBGM(SystemManager.Instance.SoundManager.SoundData.MainBGM);
            //}
            
            
            // 무지성 보스 HP 버그 수정
            SystemManager.Instance.UIManager.GetUIPanel<BossHPCanvas>().OnClose();
        }

        public void RestartAnimation()
        {
            _popAnimation.ReStart();
        }
        
        public void GameStart()
        {
            if (SystemManager.Instance.LoadingManager.IsJsonLoad())
            {
                SystemManager.Instance.GetSystem<DungeonMapSystem>().DungenMapGenerate();
                
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
