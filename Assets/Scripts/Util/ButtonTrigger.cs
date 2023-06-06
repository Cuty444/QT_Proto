using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace QT.Util
{

    public class ButtonTrigger : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler,IPointerClickHandler,IPointerExitHandler,IPointerUpHandler
    {
        [Serializable]
        public class MyEventType : UnityEvent { } //함수 받아오기

        [SerializeField]private Sprite _targetGraphic;
        [SerializeField]private Sprite _highligtedSprite;
        [SerializeField]private Sprite _pressedSprite;
        [SerializeField]private Sprite _disabledSprite;
        [SerializeField]private Sprite _selectedSprite;
        [SerializeField]private bool _isInteractable = true;
        [SerializeField]private bool _isSelected;
        public MyEventType OnEvent;
        private Image targetImage;
        private Sprite DefualtSprite;

        private void Awake()
        {
            targetImage = GetComponent<Image>();
            if (targetImage == null)
                return;
            _targetGraphic = targetImage.sprite;
            DefualtSprite = targetImage.sprite;
        }

        public void InteractableOn()
        {
            _isInteractable = true;
            ChangeSprite(DefualtSprite);
        }

        public void InteractableOff()
        {
            _isInteractable = false;
            ChangeSprite(_disabledSprite);
        }

        public void ChangeSprite(Sprite sprite)
        {
            if (sprite == null || _targetGraphic == sprite)
                return;
            _targetGraphic = sprite;
            targetImage.sprite = _targetGraphic;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!_isInteractable)
                return;
            ChangeSprite(_highligtedSprite);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (!_isInteractable)
                return;
            ChangeSprite(_pressedSprite);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!_isInteractable)
                return;
            
            OnEvent.Invoke();
            if (_isSelected)
            {
                ChangeSprite(_selectedSprite);
                _isInteractable = false;
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (!_isInteractable)
                return;
            ChangeSprite(DefualtSprite);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (!_isInteractable)
                return;
            ChangeSprite(DefualtSprite);
        }

        public void Clear()
        {
            _isInteractable = true;
            targetImage.sprite = DefualtSprite;
        }
    }
}