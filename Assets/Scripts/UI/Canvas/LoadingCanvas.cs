using System;
using System.Collections;
using System.Collections.Generic;
using QT.Core;
using QT.UI;
using UnityEngine;

namespace QT.UI
{
    public class LoadingCanvas : UIPanel
    {
        public override void Initialize()
        {
            SystemManager.Instance.PlayerManager.PlayerCreateEvent.AddListener((arg) =>
            {
                GetComponent<Canvas>().worldCamera = Camera.main;
            });
        }
        private void OnEnable()
        {
            SystemManager.Instance.SoundManager.PlayBGM(SystemManager.Instance.SoundManager.SoundData.LoadingBGM);
        }
    }
}
