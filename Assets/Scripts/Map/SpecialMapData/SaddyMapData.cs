using QT.Core;
using QT.Map;
using QT.UI;
using UnityEngine;

namespace QT
{
    public class SaddyMapData : SpecialMapData,IBossClearEvent
    {
        [field:SerializeField] public EnemyWave BossWave { get; private set; }

        [field:SerializeField] public BoxCollider2D PingPongAreaCollider { get; private set; }
        [field: SerializeField] public Transform PingPongReadyPoint { get; private set; }
        [field: SerializeField] public Transform PingPongPlayerReadyPoint { get; private set; }
        
        [field:SerializeField] public GameObject BarrierObject { get; private set; }
        [SerializeField] private Transform _itemHolderTransform;
        [SerializeField] private GameObject _itemObject;
        [SerializeField] private GameObject _caseterNPC;
        
        private ItemHolder _itemHolder;

        private bool _isEntered = false;
        
        private void Start()
        {
            _caseterNPC.gameObject.SetActive(false);
        }
        
        private void Awake()
        {
            SystemManager.Instance.PlayerManager.PlayerMapPosition.AddListener(ShowVidio);
        }

        private void OnDestroy()
        {
            SystemManager.Instance?.PlayerManager.PlayerMapPosition.RemoveListener(ShowVidio);
        }

        private void ShowVidio(Vector2Int position)
        {
            if (!_isEntered && position == MapPosition)
            {
                _isEntered = true;
                SystemManager.Instance.UIManager.Show<SaddyIntroVidioCanvas>();
            }
        }
        
        public void BossClear()
        {
            _caseterNPC.gameObject.SetActive(true);
            ItemCreate();
        }
        
        private void ItemCreate()
        {
            var items = SystemManager.Instance.GetSystem<ItemPoolSystem>().GetItemsWithDropPercentage(1,
                DropGameType.Select);

            _itemHolder = Instantiate(_itemObject, _itemHolderTransform).GetComponent<ItemHolder>();
            _itemHolder.gameObject.SetActive(true);
            _itemHolder.Init(items[0],OnGainItem);
            SystemManager.Instance.GetSystem<ItemPoolSystem>().HolderItemCreatedEvent.Invoke(items);
        }
        
        public void OnGainItem()
        {
            _itemHolder.EndAnimation();
        }
    }
}
