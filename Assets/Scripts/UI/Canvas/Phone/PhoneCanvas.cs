using System.Collections;
using QT.Core;
using QT.Core.Map;
using QT.Util;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

namespace QT.UI
{
    public class PhoneCanvas : UIPanel
    {
        [field:SerializeField] public HorizontalScrollSnap ScrollSnap{ get; private set; }
        [field:SerializeField] public UIInventoryPage InventoryPage{ get; private set; }

        [field:Space]
        [field:SerializeField] public TweenAnimator PopAnimator { get; private set; }
        [field:SerializeField] public TweenAnimator ReleaseAnimator { get; private set; }
        [field:SerializeField] public TweenAnimator SwitchLeftAnimation { get; private set; }
        [field:SerializeField] public TweenAnimator SwitchRightAnimation { get; private set; }

        [field:SerializeField] public MinimapRenderer MinimapRenderer { get; private set; }
    }

    public class PhoneCanvasModel : UIModelBase
    {
        public override UIType UIType => UIType.Panel;
        public override string PrefabPath => "Phone.prefab";

        private UIInputActions _inputActions;
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


            _inputActions = new UIInputActions();

            _inputActions.UI.LeftMenu.started += OnClickLeft;
            _inputActions.UI.RightMenu.started += OnClickRight;
        }

        public override void Show()
        {
            _inputActions.Enable();

            _phoneCanvas.StopAllCoroutines();

            base.Show();

            SystemManager.Instance.SoundManager.PlayOneShot(SystemManager.Instance.SoundManager.SoundData.UITabSFX);
            SystemManager.Instance.PlayerManager.Player.Pause(true);

            _phoneCanvas.InventoryPage.SetInventoryUI();

            _phoneCanvas.ReleaseAnimator.Pause();
            _phoneCanvas.PopAnimator.ReStart();
            _phoneCanvas.MinimapRenderer.ResizeSize();
            _phoneCanvas.MinimapRenderer.ChangeCenter(DungeonManager.Instance.PlayerPosition);
        }

        public override void ReleaseUI()
        {
            _inputActions.Disable();

            if (!_phoneCanvas.gameObject.activeInHierarchy)
            {
                return;
            }

            _phoneCanvas.StopAllCoroutines();

            SystemManager.Instance.PlayerManager.Player.Pause(false);
            
            _phoneCanvas.InventoryPage.OnDisable();

            _phoneCanvas.PopAnimator.Pause();
            _phoneCanvas.ReleaseAnimator.ReStart();
            _phoneCanvas.StartCoroutine(UnityUtil.WaitForFunc(() => base.ReleaseUI(),
                _phoneCanvas.ReleaseAnimator.SequenceLength));
        }

        public void SetMiniMap(MapData mapData)
        {
            _phoneCanvas.MinimapRenderer.SetMiniMap(mapData);
        }

        private void OnClickLeft(InputAction.CallbackContext context)
        {
            _phoneCanvas.ScrollSnap.PreviousScreen();
            _phoneCanvas.SwitchLeftAnimation.ReStart();
            
            if (_phoneCanvas.ScrollSnap.CurrentPage == 1)
            {
                _phoneCanvas.InventoryPage.OnDisable();
            }
        }

        private void OnClickRight(InputAction.CallbackContext context)
        {
            _phoneCanvas.ScrollSnap.NextScreen();
            _phoneCanvas.SwitchRightAnimation.ReStart();
            
            if (_phoneCanvas.ScrollSnap.CurrentPage == 1)
            {
                _phoneCanvas.InventoryPage.OnDisable();
            }
        }


    }

}