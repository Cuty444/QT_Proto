using QT.Core;
using QT.Map;
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

        private void Start()
        {
            _caseterNPC.gameObject.SetActive(false);
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
