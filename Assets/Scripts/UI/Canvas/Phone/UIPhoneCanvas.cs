using System.Collections;
using QT.Core;
using UnityEngine;
using UnityEngine.UI;

namespace QT.UI
{
    public class UIPhoneCanvas : UIPanel
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

        public void CheckOpen()
        {
            StopAllCoroutines();

            _isOpen = !_isOpen;
            
            SystemManager.Instance.SoundManager.PlayOneShot(SystemManager.Instance.SoundManager.SoundData.UITabSFX);
            
            SystemManager.Instance.PlayerManager.Player.Pause(_isOpen);
            if (_isOpen)
            {
                _inventoryPage.SetInventoryUI();
                
                _backGround.SetActive(true);
                _popAnimation.ReStart();
                //Time.timeScale = 0;
            }
            else
            {
                StartCoroutine(CloseCoroutine());
                //Time.timeScale = _lastTimeScale;
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