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

        private List<ItemObject> _itemObjects = new List<ItemObject>();
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
                //DropGameType dropGameType = Random.Range(0, 1) == 0 ? DropGameType.GoldShop : DropGameType.HpShop;
                List<int> itemIDs =
                    SystemManager.Instance.ItemDataManager.GetDropItemList(_dropGameType, _shopItemTransforms.Length);
                for (int i = 0; i < itemIDs.Count; i++)
                {
                    var item = Instantiate(_itemObject, _shopItemTransforms[i]).GetComponent<ItemObject>();
                    item.gameObject.SetActive(true);
                    item.DropType = _dropGameType;
                    item.ItemID = itemIDs[i];
                    item._itemSelectMapData = this;
                    _itemObjects.Add(item);
                }

                SystemManager.Instance.PlayerManager.FadeInCanvasOut.RemoveListener(ItemCreate);
            }
        }
        
        private void ItemCreate()
        {
            //if (position == MapPosition)
            //{
                //DropGameType dropGameType = Random.Range(0, 1) == 0 ? DropGameType.GoldShop : DropGameType.HpShop;
                List<int> itemIDs = SystemManager.Instance.ItemDataManager.GetDropItemList(_dropGameType, _shopItemTransforms.Length);
                for (int i = 0; i < itemIDs.Count; i++)
                {
                    var item = Instantiate(_itemObject, _shopItemTransforms[i]).GetComponent<ItemObject>();
                    item.gameObject.SetActive(true);
                    item.DropType = _dropGameType;
                    item.ItemID = itemIDs[i];
                    item._itemSelectMapData = this;
                    _itemObjects.Add(item);
                }
                SystemManager.Instance.PlayerManager.FadeInCanvasOut.RemoveListener(ItemCreate);
            //}
        }

        public void ItemSelectGainEnd()
        {
            for (int i = 0; i < _itemObjects.Count; i++)
            {
                _itemObjects[i].EndAnimation();
            }
        }
    }
}
