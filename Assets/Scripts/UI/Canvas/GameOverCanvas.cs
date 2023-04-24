using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;
using QT.Core;
using QT.Util;
using QT.Core.Map;
using UnityEditor.Experimental.GraphView;

namespace QT.UI
{
    public class GameOverCanvas : UIPanel
    {
        [SerializeField] private SkeletonGraphic _skeletonGraphic;
        [SerializeField] private ButtonTrigger _retryButtonTrigger;
        [SerializeField] private CanvasGroup _canvasGroup;
        public override void OnOpen()
        {
            base.OnOpen();
            _skeletonGraphic.AnimationState.SetAnimation(1, "S_GameOver",false);
            _canvasGroup.alpha = 0f;
            _canvasGroup.interactable = false;
            SystemManager.Instance.UIManager.GetUIPanel<MinimapCanvas>().CellClear();
            StartCoroutine(UnityUtil.WaitForFunc(() =>
            {
                StartCoroutine(UnityUtil.FadeCanvasGroup(_canvasGroup, 0f, 1f, 0.5f,()=>
                {
                    _canvasGroup.interactable = true;
                }));
            }, 3.0f));
        }

        public void Retry()
        {
            _skeletonGraphic.AnimationState.SetAnimation(1, "S_GameOver_Replay",false);
            StartCoroutine(UnityUtil.WaitForFunc(() =>
            {
                SystemManager.Instance.LoadingManager.LoadScene(1, OnClose);
                SystemManager.Instance.GetSystem<DungeonMapSystem>().DungenMapGenerate();
                SystemManager.Instance.UIManager.GetUIPanel<MinimapCanvas>().MinimapSetting();
                _retryButtonTrigger.Clear();
            }, 1f));
        }

        public void Exit()
        {
            QT.Util.UnityUtil.ProgramExit();
        }
    }
}
