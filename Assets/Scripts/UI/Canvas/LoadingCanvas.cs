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
        
        private void OnEnable()
        {
            SystemManager.Instance.SoundManager.PlayBGM(SystemManager.Instance.SoundManager.SoundData.LoadingBGM);
        }
    }
}
