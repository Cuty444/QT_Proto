using System.Collections;
using QT.Core;
using QT.Util;
using UnityEngine;
using UnityEngine.UI;

namespace QT.UI
{
    public class PhoneCanvas : UIPanel
    {
        [field:SerializeField] public UIInventoryPage InventoryPage{ get; private set; }

        [field:Space]
        [field:SerializeField] public TweenAnimator PopAnimator { get; private set; }
        [field:SerializeField] public TweenAnimator ReleaseAnimator { get; private set; }
        [field:SerializeField] public TweenAnimator SwitchAnimation { get; private set; }

        [Space] public Transform MapTransform;
    }

    public class PhoneCanvasModel : UIModelBase
    {
        public override UIType UIType => UIType.Panel;
        public override string PrefabPath => "Phone.prefab";

        private PhoneCanvas _phoneCanvas;

        public override void SetState(UIState state)
        {
            switch (state)
            {
                case UIState.Phone:
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
            _phoneCanvas = UIView as PhoneCanvas;
            
            _phoneCanvas.InventoryPage.Initialize();
        }
        
        public override void Show()
        {
            _phoneCanvas.StopAllCoroutines();
            
            base.Show();
            
            SystemManager.Instance.SoundManager.PlayOneShot(SystemManager.Instance.SoundManager.SoundData.UITabSFX);
            SystemManager.Instance.PlayerManager.Player.Pause(true);
            
            _phoneCanvas.InventoryPage.SetInventoryUI();
            
            _phoneCanvas.PopAnimator.ReStart();
        }
        
        public override void ReleaseUI()
        {
            if (!_phoneCanvas.gameObject.activeInHierarchy)
            {
                return;
            }
            _phoneCanvas.StopAllCoroutines();
            
            SystemManager.Instance.PlayerManager.Player.Pause(false);
            
            _phoneCanvas.ReleaseAnimator.ReStart();
            _phoneCanvas.StartCoroutine(UnityUtil.WaitForFunc(() => base.ReleaseUI(), _phoneCanvas.ReleaseAnimator.SequenceLength));
        }
        
    }

}