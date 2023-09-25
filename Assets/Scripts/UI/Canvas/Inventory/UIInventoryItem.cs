using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using QT.Core;

namespace QT
{
    public class UIInventoryItem : MonoBehaviour
    {
        [SerializeField] private GameObject _iconGameObject;
        [SerializeField] private Image _icon;
        [SerializeField] private Image _frame;
        private Button _button;
        
        [SerializeField] private Sprite _frameNormal;
        [SerializeField] private Sprite _frameRare;
        [SerializeField] private Sprite _frameCursed;

        public ItemGameData ItemGameData { get; private set; }
        public UnityAction<UIInventoryItem> OnClick { get; set; }
        
        private void Awake()
        {
            _button = GetComponent<Button>();
            _button.onClick.AddListener(OnClickIcon);
            
            _iconGameObject.SetActive(false);
        }

        public async void SetItem(int index, ItemGameData data)
        {
            ItemGameData = data;

            var sprite = await SystemManager.Instance.ResourceManager.LoadAsset<Sprite>(data.ItemIconPath, true);

            _icon.sprite = sprite;

            switch (data.GradeType)
            {
                case ItemGameData.GradeTypes.Cursed:
                    _frame.sprite = _frameCursed;
                    break;
                case ItemGameData.GradeTypes.Rare:
                    _frame.sprite = _frameRare;
                    break;
                default:
                    _frame.sprite = _frameNormal;
                    break;
            }
            
            _iconGameObject.SetActive(true);
        }
        
        public void ClearItem()
        {
            _iconGameObject.SetActive(false);
        }

        private void OnClickIcon()
        {
            OnClick?.Invoke(this);
        }
    }
}
