using System.Collections;
using System.Collections.Generic;
using QT.Core;
using UnityEngine;
using Application = UnityEngine.Device.Application;

namespace QT.UI
{
    public class TitleCanvas : UIPanel
    {
        public override void PostSystemInitialize()
        {
            OnOpen();
        }

        public void GameStart()
        {
            SystemManager.Instance.LoadingManager.LoadScene(1,OnClose);
        }

        public void GameEnd()
        {
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #else
                Application.Quit();
            #endif
        }
    }
}
