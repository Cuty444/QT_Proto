using System.Collections;
using QT.Core;
using UnityEngine;

namespace QT.UI
{
    public class UIInventoryCanvas : UIPanel
    {
        [SerializeField] private GameObject _inventoryGameobject;
        [SerializeField] private Transform _itemListParents;

        [SerializeField] private UIInventoryDesc _desc;
        
        private UIInventoryItem[] _itemFrames;

        [SerializeField] private UITweenAnimator _popAnimation;
        [SerializeField] private UITweenAnimator _releaseAnimation;

        private bool _isOpen = false;
        
        public override void PostSystemInitialize()
        {
            _itemFrames = _itemListParents.GetComponentsInChildren<UIInventoryItem>();
            
            gameObject.SetActive(true);
            _inventoryGameobject.SetActive(false);
        }

        private void Update()
        {
            CheckInput();
        }

        private void CheckInput()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                _isOpen = !_isOpen;

                StopAllCoroutines();
                if (_isOpen)
                {
                    SetInventoryUI();
                    _inventoryGameobject.SetActive(true);
                    _popAnimation.ReStart();
                }
                else
                {
                    StartCoroutine(CloseCorutine());
                }
            }
        }

        private IEnumerator CloseCorutine()
        {
            _releaseAnimation.ReStart();
            _desc.Hide();
            
            yield return new WaitForSeconds(_releaseAnimation.SequenceLength);
            
            _inventoryGameobject.SetActive(false);
        }

        private void SetInventoryUI()
        {
            var items = SystemManager.Instance.PlayerManager.Player.Inventory.GetItemList();

            for (int i = 0; i < _itemFrames.Length; i++)
            {
                if (i < items.Length)
                {
                    var itemData = items[i].ItemGameData;
                    
                    _itemFrames[i].SetItem(i, itemData);
                    _itemFrames[i].OnClick = OnClickItem;
                }
                else
                {
                    _itemFrames[i].ClearItem();
                    _itemFrames[i].OnClick = null;
                }
            }
        }

        private void OnClickItem(UIInventoryItem item)
        {
            _desc.Show(item);
        }
    }
}
