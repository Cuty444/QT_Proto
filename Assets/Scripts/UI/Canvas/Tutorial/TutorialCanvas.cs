using QT.Core;
using QT.UI;
using UnityEngine;
using UnityEngine.UI.Extensions;
using UnityEngine.Video;

namespace QT.Tutorial
{
    public class TutorialCanvas : UIPanel
    {
        [SerializeField] private VideoClip[] _videoClips;
        
        [SerializeField] private RectTransform _videoParent;
        [SerializeField] private VideoPlayer _videoPlayer;
        
        [SerializeField] private HorizontalScrollSnap _scrollSnap;
        
        [SerializeField] private TweenAnimator _popAnimation;
        
        public override void OnOpen()
        {
            base.OnOpen();
            OnPageChanged(0);
            
            _popAnimation.ReStart();
        }

        public override void OnClose()
        {
            base.OnClose();
            
            //SystemManager.Instance.UIManager.GetUIPanel<TitleCanvas>();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape)) OnClose();
            
            if(Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
                _scrollSnap.PreviousScreen();
            else if(Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
                _scrollSnap.NextScreen();
        }

        public void OnPageChangeStart()
        {
            _videoParent.gameObject.SetActive(false);
        }

        public void OnPageChanged(int index)
        {  
            if (index < 0 || index >= _videoClips.Length || _videoClips[index] == null)
            {
                _videoParent.gameObject.SetActive(false);
                return;
            }
            
            _scrollSnap.CurrentPageObject(out var page);
            
            _videoParent.SetParent(page.transform);
            _videoParent.anchoredPosition = Vector3.zero;
            _videoParent.gameObject.SetActive(true);
            
            _videoPlayer.clip = _videoClips[index];
            _videoPlayer.Play();
        }
    }
}