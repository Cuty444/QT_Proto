using System.Collections;
using System.Collections.Generic;
using QT.Core;
using UnityEngine;

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
            if (SystemManager.Instance.LoadingManager.IsJsonLoad())
            {
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
