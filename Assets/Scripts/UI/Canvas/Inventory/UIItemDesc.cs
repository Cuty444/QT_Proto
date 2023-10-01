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

        [SerializeField] private TweenAnimator _descAnimation;
        [SerializeField] private TweenAnimator _failButtonAnimation;

        public void SetData(ItemGameData itemData)
        {
            _name.text = itemData.Name;
            _desc.text = itemData.Desc;
            
            _plusDesc.text = itemData.PlusDesc;
            _plusDesc.gameObject.SetActive(!string.IsNullOrWhiteSpace(itemData.PlusDesc));
            
            _minusDesc.text = itemData.MinusDesc;
            _minusDesc.gameObject.SetActive(!string.IsNullOrWhiteSpace(itemData.MinusDesc));

            if(_goldCost != null)
                _goldCost.text = itemData.CostGold.ToString();
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