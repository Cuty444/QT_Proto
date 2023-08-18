using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace QT
{
    public class UIItemDesc : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _name;
        [SerializeField] private TextMeshProUGUI _desc;
        [SerializeField] private TextMeshProUGUI _plusDesc;
        [SerializeField] private TextMeshProUGUI _minusDesc;

        [SerializeField] private UITweenAnimator _descAnimation;
        [SerializeField] private UITweenAnimator _failButtonAnimation;

        public void Show(UIInventoryItem item)
        {
            transform.position = item.transform.position;
            _name.text = item.ItemGameData.Name;
            _desc.text = item.ItemGameData.Desc;

            if (!string.IsNullOrWhiteSpace(item.ItemGameData.PlusDesc))
            {
                _plusDesc.text = item.ItemGameData.PlusDesc;
            }
            else
            {
                _plusDesc.gameObject.SetActive(false);
            }

            
            if (!string.IsNullOrWhiteSpace(item.ItemGameData.MinusDesc))
            {
                _minusDesc.text = item.ItemGameData.MinusDesc;
            }
            else
            {
                _minusDesc.gameObject.SetActive(false);
            }
            
            _descAnimation.ReStart();
        }

        public void Hide()
        {
            _descAnimation.PlayBackwards();
        }

        public void PlayFailButtonAnimation()
        {
            _failButtonAnimation?.ReStart();
        }
    }
}