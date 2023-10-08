using System.Collections;
using QT.Core;
using QT.InGame;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace QT.UI
{
    public class UIActiveItemSelectCanvas : UIPanel
    {
        [field:SerializeField] public CanvasGroup CanvasGroup { get; private set; }

        [field:SerializeField] public Image[] ActiveItemImage { get; private set; } = new Image[2];
        [field:SerializeField] public UIItemDesc[] ActiveDesc { get; private set; } = new UIItemDesc[2];
        [field:SerializeField] public Button[] Buttons  { get; private set; } = new Button[2];

        [field:Space] 
        [field:SerializeField] public TweenAnimator[] PopAnimation { get; private set; }  = new TweenAnimator[2];
        [field:SerializeField] public TweenAnimator[] ReleaseAnimation { get; private set; }  = new TweenAnimator[2];
        [field:SerializeField] public TweenAnimator[] SelectAnimation { get; private set; }  = new TweenAnimator[2];
    }

    public class UIActiveItemSelectCanvasModel : UIModelBase
    {
        public override UIType UIType => UIType.Panel;
        public override string PrefabPath => "ActiveSelect.prefab";

        private UIActiveItemSelectCanvas _selectCanvas;

        
        private UnityAction<ItemGameData> _onSelect;
        private ItemGameData[] _items = new ItemGameData[2];
        
        private float _timeScale = 0;
        
        public override void OnCreate(UIPanel view)
        {
            base.OnCreate(view);
            _selectCanvas = UIView as UIActiveItemSelectCanvas;
        }

        public override void SetState(UIState state)
        {
            switch (state)
            {
                default:
                    ReleaseUI();
                    break;
            }
        }
        
        
        public async void Initialize(ItemGameData itemGameData1, ItemGameData itemGameData2, UnityAction<ItemGameData> onSelect)
        {
            if (itemGameData1 == itemGameData2)
            {
                return;
            }
            
            _selectCanvas.StopAllCoroutines();

            var resourceManager = SystemManager.Instance.ResourceManager;
            
            _selectCanvas.ActiveItemImage[0].sprite = await resourceManager.LoadAsset<Sprite>(itemGameData1.ItemIconPath, true);
            _selectCanvas.ActiveItemImage[1].sprite = await resourceManager.LoadAsset<Sprite>(itemGameData2.ItemIconPath, true);
            
            _selectCanvas.ActiveDesc[0].SetData(itemGameData1);
            _selectCanvas.ActiveDesc[1].SetData(itemGameData2);
            
            _items[0] = itemGameData1;
            _items[1] = itemGameData2;

            _onSelect = onSelect;
            
            _selectCanvas.PopAnimation[0].ReStart();
            _selectCanvas.PopAnimation[1].ReStart();

            
            _selectCanvas.CanvasGroup.alpha = 1;
            
            _timeScale = Time.timeScale;
            Time.timeScale = 0;
        
            foreach (var button in _selectCanvas.Buttons)
            {
                button.enabled = true;
            }
        }

        public void Select(int index)
        {
            Time.timeScale = _timeScale;
            
            _onSelect?.Invoke(_items[index]);
            
            foreach (var button in _selectCanvas.Buttons)
            {
                button.enabled = false;
            }
            
            _selectCanvas.ReleaseAnimation[index == 0 ? 1 : 0].ReStart();
            _selectCanvas.SelectAnimation[index].ReStart();

            _selectCanvas.StartCoroutine(FadeOut());
        }
        
        IEnumerator FadeOut()
        {
            yield return new WaitForSeconds(1);
            
            while (_selectCanvas.CanvasGroup.alpha > 0)
            {
                _selectCanvas.CanvasGroup.alpha -= Time.deltaTime * 4;
                yield return null;
            }
            
            ReleaseUI();
        }
        
    }


}