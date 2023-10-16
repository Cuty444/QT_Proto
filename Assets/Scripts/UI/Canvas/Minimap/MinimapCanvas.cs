using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using QT.Core;
using QT.Map;
using QT.Core.Map;
using QT.Util;
using UnityEngine;

namespace QT.UI
{
    public class MinimapCanvas : UIPanel
    {
        [field:SerializeField] public MinimapRenderer MinimapRenderer { get; private set; }
        
        [field:Space]
        [field:SerializeField] public TweenAnimator PopAnimator { get; private set; }
        [field:SerializeField] public TweenAnimator ReleaseAnimator { get; private set; }
    }


    public class MinimapCanvasModel : UIModelBase
    {
        public override UIType UIType => UIType.Panel;
        public override string PrefabPath => "MiniMap.prefab";
        
        private MinimapCanvas _minimapCanvas;

        
        public override void SetState(UIState state)
        {
            switch (state)
            {
                case UIState.InGame:
                    Show();
                    break;
                default:
                    ReleaseUI();
                    break;
            }
        }

        public override void OnCreate(UIPanel view)
        {
            base.OnCreate(view);

            _minimapCanvas = UIView as MinimapCanvas;
        }

        public override void Show()
        {
            base.Show();
            _minimapCanvas.StopAllCoroutines();
            _minimapCanvas.PopAnimator.ReStart();
        }
        
        public override void ReleaseUI()
        {
            if (!_minimapCanvas.gameObject.activeInHierarchy)
            {
                return;
            }
            
            _minimapCanvas.ReleaseAnimator.ReStart();
            _minimapCanvas.StopAllCoroutines();
            _minimapCanvas.StartCoroutine(UnityUtil.WaitForFunc(() => base.ReleaseUI(), _minimapCanvas.ReleaseAnimator.SequenceLength));
        }
        
        public void SetMiniMap(MapData mapData)
        {
            _minimapCanvas.MinimapRenderer.SetMiniMap(mapData);
        }
        
        public void ChangeCenter(Vector2Int position)
        {
            _minimapCanvas.MinimapRenderer.ChangeCenter(position);
        }
        
    }
    
}
