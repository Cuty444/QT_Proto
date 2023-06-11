using System.Collections;
using System.Collections.Generic;
using QT.Core;
using QT.Util;
using UnityEngine;

namespace QT.UI
{
    public class TitleCanvas : UIPanel
    {
        [SerializeField] private ButtonTrigger _startButton;
        [SerializeField] private ButtonTrigger _tutorialButton;
        public override void PostSystemInitialize()
        {
            OnOpen();
        }

        public override void OnOpen()
        {
            base.OnOpen();
            GetComponent<Canvas>().worldCamera = Camera.main;
            SystemManager.Instance.SoundManager.PlayBGM(SystemManager.Instance.SoundManager.SoundData.MainBGM);
            _startButton.InteractableOn();
            _tutorialButton.InteractableOn();
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
    }
}
