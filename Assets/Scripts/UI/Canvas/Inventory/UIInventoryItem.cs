using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using QT.Core;

namespace QT
{
    public class UIInventoryItem : MonoBehaviour
    {
        [SerializeField] private Image _icon;
        private Button _button;
        
        public ItemGameData ItemGameData { get; private set; }
        public UnityAction<UIInventoryItem> OnClick { get; set; }
        
        private void Awake()
        {
            _button = GetComponent<Button>();
            _button.onClick.AddListener(OnClickIcon);
            
            _icon.gameObject.SetActive(false);
        }

        public async void SetItem(int index, ItemGameData data)
        {
            ItemGameData = data;

            var sprite = await SystemManager.Instance.ResourceManager.LoadAsset<Sprite>(data.ItemIconPath, true);

            _icon.sprite = sprite;
            _icon.gameObject.SetActive(true);
        }
        
        public void ClearItem()
        {
            _icon.gameObject.SetActive(false);
        }

        private void OnClickIcon()
        {
            OnClick?.Invoke(this);
        }
    }
}
