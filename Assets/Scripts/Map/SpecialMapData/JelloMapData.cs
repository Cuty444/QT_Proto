using System;
using QT.Core;
using QT.Map;
using UnityEngine;

namespace QT
{
    public class JelloMapData : SpecialMapData,IBossClearEvent
    {
        [SerializeField] private Transform _itemHolderTransform;
        [SerializeField] private GameObject _itemObject;
        [field: SerializeField] public Transform MapCenter { get; private set; }

        // #if UNITY_EDITOR
        // private void OnDrawGizmos()
        // {
        //     if (MapCenter != null)
        //     {
        //         Gizmos.DrawWireSphere(MapCenter.position, 8);
        //     }
        // }
        // #endif
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
