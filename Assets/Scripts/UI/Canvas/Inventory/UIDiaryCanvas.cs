using System.Collections;
using QT.Core;
using UnityEngine;
using UnityEngine.UI;

namespace QT.UI
{
    public class UIDiaryCanvas : UIPanel
    {
        [SerializeField] private GameObject _backGround;

        [SerializeField] private UIInventoryPage _inventoryPage;

        [Space] [SerializeField] private TweenAnimator _popAnimation;
        [SerializeField] private TweenAnimator _releaseAnimation;
        [SerializeField] private TweenAnimator _switchAnimation;

        [Space] public Transform MapTransform;

        private bool _isOpen = false;


        public override void PostSystemInitialize()
        {
            gameObject.SetActive(true);
            _inventoryPage.Initialize();

            _backGround.SetActive(false);
        }
        
        private void Update()
        {
            CheckInput();
        }

        private void CheckInput()
        {
            if (SystemManager.Instance.PlayerManager.Player == null)
            {
                return;
            }
            
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                CheckOpen();
            }
            
            SystemManager.Instance.SoundManager.VolumeSave();
        }

        private void CheckOpen()
        {
            StopAllCoroutines();

            _isOpen = !_isOpen;
            
            SystemManager.Instance.UIManager.GetUIPanel<MinimapCanvas>().OnOff(_isOpen);
            
            SystemManager.Instance.SoundManager.PlayOneShot(SystemManager.Instance.SoundManager.SoundData.UITabSFX);
            if (_isOpen)
            {
                _inventoryPage.SetInventoryUI();
                
                _backGround.SetActive(true);
                _popAnimation.ReStart();
            }
            else
            {
                StartCoroutine(CloseCoroutine());
            }
        }


        private IEnumerator CloseCoroutine()
        {
            _releaseAnimation.ReStart();

            yield return new WaitForSeconds(_releaseAnimation.SequenceLength);

            _backGround.SetActive(false);
        }
        
    }
}