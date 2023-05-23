using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace QT
{
    public class UIInventoryItem : MonoBehaviour
    {
        private Button _button;

        public UnityAction OnClick { get; set; }

        private void Awake()
        {
            _button = GetComponent<Button>();
            _button.onClick.AddListener(() => OnClick?.Invoke());
        }
    }
}
