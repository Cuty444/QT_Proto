using System.Collections;
using System.Collections.Generic;
using QT.Core;
using UnityEngine;

namespace QT
{
    public class ItemSelectMapData : MonoBehaviour
    {
        [SerializeField] private Transform[] _shopItemTransforms;
        [SerializeField] private GameObject _itemObject;
        [SerializeField] private DropGameType _dropGameType;

        public Vector2Int MapPosition;

        private List<ItemHolder> _itemObjects = new ();
        
        private void Awake()
        {
            ItemCreate();
        }
        
        private void ItemCreate()
        {
            var items = SystemManager.Instance.GetSystem<ItemPoolSystem>().GetItemsWithDropPercentage(_shopItemTransforms.Length,
                DropGameType.Start);
            for (int i = 0; i < items.Count; i++)
            {
                var holder = Instantiate(_itemObject, _shopItemTransforms[i]).GetComponent<ItemHolder>();
                holder.gameObject.SetActive(true);
                holder.Init(items[i], OnGainItem);
                
                _itemObjects.Add(holder);
            }
        }

        public void OnGainItem()
        {
            for (int i = 0; i < _itemObjects.Count; i++)
            {
                _itemObjects[i].EndAnimation();
            }
        }
    }
}
