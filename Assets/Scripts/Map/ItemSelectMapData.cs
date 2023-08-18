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
            if (_dropGameType == DropGameType.Select)
            {
                SystemManager.Instance.PlayerManager.PlayerMapPosition.AddListener(ItemCreate);
            }
            else if (_dropGameType == DropGameType.Start)
            {
                SystemManager.Instance.PlayerManager.FadeInCanvasOut.AddListener(ItemCreate);
            }
        }

        private void ItemCreate(Vector2Int position)
        {
            if (position == MapPosition)
            {
                ItemCreate();
            }
        }
        
        private void ItemCreate()
        {
            var percent = SystemManager.Instance.DataManager.GetDataBase<DropGameDataBase>().GetData((int)_dropGameType);
                
            var items = SystemManager.Instance.DataManager.GetDataBase<ItemGameDataBase>()
                .GetItemsWithDropPercentage(percent, _shopItemTransforms.Length,
                    SystemManager.Instance.PlayerManager.Player.Inventory);

            for (int i = 0; i < items.Count; i++)
            {
                var holder = Instantiate(_itemObject, _shopItemTransforms[i]).GetComponent<ItemHolder>();
                holder.gameObject.SetActive(true);
                holder.Init(items[i], OnGainItem);
                
                _itemObjects.Add(holder);
            }

            SystemManager.Instance.PlayerManager.FadeInCanvasOut.RemoveListener(ItemCreate);
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
