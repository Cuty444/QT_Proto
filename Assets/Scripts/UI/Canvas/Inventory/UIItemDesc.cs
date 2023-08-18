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
        
        [SerializeField] private TextMeshProUGUI _goldCost;

        [SerializeField] private UITweenAnimator _descAnimation;
        [SerializeField] private UITweenAnimator _failButtonAnimation;

        public void SetData(ItemGameData itemData)
        {
            _name.text = itemData.Name;
            _desc.text = itemData.Desc;
            _goldCost.text = itemData.CostGold.ToString();

            if (!string.IsNullOrWhiteSpace(itemData.PlusDesc))
            {
                _plusDesc.text = itemData.PlusDesc;
            }
            else
            {
                _plusDesc.gameObject.SetActive(false);
            }

            if (!string.IsNullOrWhiteSpace(itemData.MinusDesc))
            {
                _minusDesc.text = itemData.MinusDesc;
            }
            else
            {
                _minusDesc.gameObject.SetActive(false);
            }
        }
        
        public void Show()
        {
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