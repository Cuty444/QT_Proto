using DG.Tweening;
using QT.Core;
using QT.Util;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace QT.UI
{
    public class LoadingCanvas : UIPanel
    {
        [field:SerializeField] public CanvasGroup CanvasGroup { get; private set; }
        [field:SerializeField] public float FadeInOutTime { get; private set; }
        
        [field:SerializeField] public string[] LoadingTexts { get; private set; }
        
        [field:SerializeField] public TextMeshProUGUI Text { get; private set; }
        [field:SerializeField] public TweenAnimator Animation { get; private set; }
    }


    public class LoadingCanvasModel : UIModelBase
    {
        public override UIType UIType => UIType.Panel;
        public override string PrefabPath => "Loading.prefab";

        private LoadingCanvas _loadingCanvas;
        
        
        public override void OnCreate(UIPanel view)
        {
            base.OnCreate(view);
            _loadingCanvas = UIView as LoadingCanvas;
        }
        
        public override void SetState(UIState state)
        {
            switch (state)
            {
                case UIState.Loading:
                    Show();
                    break;
                default:
                    ReleaseUI();
                    break;
            }
        }

        public override void Show()
        {
            base.Show();
            SystemManager.Instance.SoundManager.PlayBGM(SystemManager.Instance.SoundManager.SoundData.LoadingBGM);
            
            _loadingCanvas.StopAllCoroutines();
            _loadingCanvas.CanvasGroup.DOFade(1, _loadingCanvas.FadeInOutTime).SetEase(Ease.InQuad);
            
            _loadingCanvas.Animation.GoToPlay(Random.value * _loadingCanvas.Animation.SequenceLength);
            
            if(SystemManager.IsAlive && SystemManager.Instance.IsInitialized)
            {
                var text = _loadingCanvas.LoadingTexts[Random.Range(0, _loadingCanvas.LoadingTexts.Length)];
                _loadingCanvas.Text.text = SystemManager.Instance.DataManager.GetDataBase<LocaleGameDataBase>().GetString(text);
            }
            else
            {
                _loadingCanvas.Text.text = "";
            }
        }

        public override void ReleaseUI()
        {
            if (!_loadingCanvas.gameObject.activeInHierarchy)
            {
                return;
            }
            
            _loadingCanvas.CanvasGroup.DOFade(0, _loadingCanvas.FadeInOutTime).SetEase(Ease.InQuad);;
            
            _loadingCanvas.StopAllCoroutines();
            _loadingCanvas.StartCoroutine(UnityUtil.WaitForFunc(() => base.ReleaseUI(), _loadingCanvas.FadeInOutTime));
        }
        
    }
}
