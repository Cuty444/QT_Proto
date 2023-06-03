using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Serialization;

namespace QT
{
    public class UIInventoryDesc : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _name;
        [SerializeField] private TextMeshProUGUI _desc;

        [SerializeField] private UITweenAnimator _descAnimation;

        public void Show(UIInventoryItem item)
        {
            transform.position = item.transform.position;
            _name.text = item.ItemGameData.Name;
            _desc.text = item.ItemGameData.Desc;

            _descAnimation.ReStart();
        }

        public void Hide()
        {
            _descAnimation.PlayBackwards();
        }
    }
}