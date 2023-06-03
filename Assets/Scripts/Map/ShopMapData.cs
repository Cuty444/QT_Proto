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
                DropGameType dropGameType = Random.Range(10, 50) == 0 ? DropGameType.GoldShop : DropGameType.HpShop;
                List<int> itemIDs = SystemManager.Instance.ItemDataManager.GetDropItemList(dropGameType, _shopItemTransforms.Length);
                for (int i = 0; i < itemIDs.Count; i++)
                {
                    var item = Instantiate(_itemObject, _shopItemTransforms[i]).GetComponent<ItemObject>();
                    item.gameObject.SetActive(true);
                    item.DropType = dropGameType;
                    item.ItemID = itemIDs[i];
                }
                SystemManager.Instance.PlayerManager.PlayerMapPosition.RemoveListener(ItemCreate);
            }
        }
    }
}
