using System.Collections;
using System.Collections.Generic;
using QT.Core;
using UnityEngine;

namespace QT
{
    public class ShopMapData : MonoBehaviour
    {
        [SerializeField] private Transform[] _shopItemTransforms;
        [SerializeField] private GameObject _itemObject;
        
        public Vector2Int MapPosition;
        
        private void Awake()
        {
            SystemManager.Instance.PlayerManager.PlayerMapPosition.AddListener(ItemCreate);
        }

        private void ItemCreate(Vector2Int position)
        {
            if (position == MapPosition)
            {
                var percent = SystemManager.Instance.DataManager.GetDataBase<DropGameDataBase>().GetData((int)DropGameType.Shop);
                
                var items = SystemManager.Instance.DataManager.GetDataBase<ItemGameDataBase>()
                    .GetItemsWithDropPercentage(percent, _shopItemTransforms.Length,
                        SystemManager.Instance.PlayerManager.Player.Inventory);
                
                for (int i = 0; i < items.Count; i++)
                {
                    var holder = Instantiate(_itemObject, _shopItemTransforms[i]).GetComponent<ItemHolder>();
                    
                    holder.gameObject.SetActive(true);
                    holder.Init(items[i]);
                }
                SystemManager.Instance.PlayerManager.PlayerMapPosition.RemoveListener(ItemCreate);
            }
        }
    }
}
