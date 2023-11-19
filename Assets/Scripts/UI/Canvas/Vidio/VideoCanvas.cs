using QT.Core;
using QT.Util;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;
using UnityEngine.Video;

namespace QT.UI
{
    public class VideoCanvas : UIPanel
    {
        [field:SerializeField] public VideoPlayer VideoPlayer;
    }


    public class VideoCanvasModel : UIModelBase
    {
        public override bool UseStack => true;
        public override UIType UIType => UIType.Popup;
        public override string PrefabPath => "Video.prefab";
        
        private VideoCanvas _videoCanvas;
        
        private float _lastTimeScale = 1;
        
        public UnityAction OnFinish;
        
        public override void OnCreate(UIPanel view)
        {
            base.OnCreate(view);
            _videoCanvas = UIView as VideoCanvas;
        }
        
        public override void Show()
        {
            base.Show();
            
            if (Time.timeScale != 0)
            {
                _lastTimeScale = Time.timeScale;
                Time.timeScale = 0;
            }

            _videoCanvas.StartCoroutine(UnityUtil.WaitForRealTimeFunc(
                () => ReleaseUI(),
                (float) _videoCanvas.VideoPlayer.clip.length));
        }

        public override void ReleaseUI()
        {
            Time.timeScale = _lastTimeScale;
            
            OnFinish?.Invoke();
            OnFinish = null;
            base.ReleaseUI();
        }
        
    }

    public class JelloIntroVideoCanvas : VideoCanvasModel
    {
        public override string PrefabPath => "JelloIntro.prefab";
    }
    
    public class SaddyIntroVideoCanvas : VideoCanvasModel
    {
        public override string PrefabPath => "SaddyIntro.prefab";
    }
    
    public class DullahanIntroVideoCanvas : VideoCanvasModel
    {
        public override string PrefabPath => "DullahanIntro.prefab";
    }
    
    public class EndingVideoCanvas : VideoCanvasModel
    {
        public override string PrefabPath => "EndingVideo.prefab";
    }
}