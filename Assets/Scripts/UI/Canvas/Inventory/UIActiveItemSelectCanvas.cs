using System.Collections;
using QT.Core;
using QT.InGame;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace QT.UI
{
    public class UIActiveItemSelectCanvas : UIPanel
    {
        [SerializeField] private CanvasGroup _canvasGroup;
        
        [SerializeField] private Image[] _activeItemImage = new Image[2];
        [SerializeField] private UIItemDesc[] _activeDesc = new UIItemDesc[2];
        [SerializeField] private Button[] _buttons = new Button[2];

        [Space] 
        [SerializeField] private UITweenAnimator[] _popAnimation = new UITweenAnimator[2];
        [SerializeField] private UITweenAnimator[] _releaseAnimation = new UITweenAnimator[2];
        [SerializeField] private UITweenAnimator[] _selectAnimation = new UITweenAnimator[2];

        private UnityAction<ItemGameData> _onSelect;
        private ItemGameData[] _items = new ItemGameData[2];
        
        private float _timeScale = 0;
        
        public override void PostSystemInitialize()
        {
            gameObject.SetActive(false);
        }

        public async void Show(ItemGameData itemGameData1, ItemGameData itemGameData2, UnityAction<ItemGameData> onSelect)
        {
            StopAllCoroutines();

            var resourceManager = SystemManager.Instance.ResourceManager;
            
            _activeItemImage[0].sprite = await resourceManager.LoadAsset<Sprite>(itemGameData1.ItemIconPath, true);
            _activeItemImage[1].sprite = await resourceManager.LoadAsset<Sprite>(itemGameData2.ItemIconPath, true);
            
            _activeDesc[0].SetData(itemGameData1);
            _activeDesc[1].SetData(itemGameData2);
            
            _items[0] = itemGameData1;
            _items[1] = itemGameData2;

            _onSelect = onSelect;
            
            _popAnimation[0].ReStart();
            _popAnimation[1].ReStart();

            
            _canvasGroup.alpha = 1;
            
            _timeScale = Time.timeScale;
            Time.timeScale = 0;
        
            foreach (var button in _buttons)
            {
                button.gameObject.SetActive(true);
            }
            
            gameObject.SetActive(true);
        }

        public void Select(int index)
        {
            Time.timeScale = _timeScale;
            
            _onSelect?.Invoke(_items[index]);
            
            foreach (var button in _buttons)
            {
                button.gameObject.SetActive(false);
            }
            
            _releaseAnimation[index == 0 ? 1 : 0].ReStart();
            _selectAnimation[index].ReStart();

            StartCoroutine(FadeOut());
        }
        
        IEnumerator FadeOut()
        {
            yield return new WaitForSeconds(1);
            
            while (_canvasGroup.alpha > 0)
            {
                _canvasGroup.alpha -= Time.deltaTime * 4;
                yield return null;
            }
            
            gameObject.SetActive(false);
        }
        
    }
}