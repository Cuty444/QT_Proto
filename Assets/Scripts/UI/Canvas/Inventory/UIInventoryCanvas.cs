using System.Collections;
using Cysharp.Threading.Tasks.Triggers;
using QT.Core;
using UnityEngine;

namespace QT.UI
{
    public class UIInventoryCanvas : UIPanel
    {
        [SerializeField] private GameObject _backGround;
        
        [SerializeField] private GameObject _settingGameobject;
        [SerializeField] private UIInventoryPage _inventoryPage;
        
        [Space]
        [SerializeField] private UITweenAnimator _popAnimation;
        [SerializeField] private UITweenAnimator _releaseAnimation;
        [SerializeField] private UITweenAnimator _switchAnimation;

        [Space]
        public Transform MapTransform;
        
        private bool _isOpen = false;

        private bool _isInventory = true;
        
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

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (_isOpen && _isInventory)
                {
                    SwitchPage(false);
                }
                else
                {
                    _isInventory = false;
                    CheckOpen();
                }
            }
            
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                if (_isOpen && !_isInventory)
                {
                    SwitchPage(true);
                }
                else
                {
                    _isInventory = true;
                    CheckOpen();
                }
            }
        }

        private void CheckOpen()
        {
            StopAllCoroutines();
            
            _isOpen = !_isOpen;
            if (_isOpen)
            {
                SystemManager.Instance.UIManager.InventoryInputCheck.Invoke(true);

                SetPage();

                _backGround.SetActive(true);
                _popAnimation.ReStart();
            }
            else
            {
                StartCoroutine(CloseCorutine());
            }
        }

        private void SetPage()
        {
            _settingGameobject.SetActive(!_isInventory);
            _inventoryPage.gameObject.SetActive(_isInventory);
                
            if (_isInventory)
            {
                _inventoryPage.SetInventoryUI();
            }
            else
            {
                // 세팅 페이지 세팅
            }
        }

        public void SwitchPage(bool isInventory)
        {
            _isInventory = isInventory;
            StartCoroutine(SwitchCorutine());
        }
        
        private IEnumerator SwitchCorutine()
        {
            _switchAnimation.ReStart();

            yield return new WaitForSeconds(0.2f);
            
            SetPage();
        }
        
        
        
        private IEnumerator CloseCorutine()
        {
            _releaseAnimation.ReStart();
            
            yield return new WaitForSeconds(_releaseAnimation.SequenceLength);
            
            _backGround.SetActive(false);
        }
    }
}
