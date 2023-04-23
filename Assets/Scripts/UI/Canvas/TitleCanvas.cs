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
            QT.Util.UnityUtil.ProgramExit();
        }
    }
}
