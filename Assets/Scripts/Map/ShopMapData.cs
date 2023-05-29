using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QT
{
    public class ShopMapData : MonoBehaviour
    {
        [SerializeField] private Transform[] _shopItemTransforms;
        [SerializeField] private GameObject _itemObject;

        [SerializeField] private int[] _itemCodes;
        private List<int> _itemIDs = new List<int>();
        private void Awake()
        {
            _itemIDs.Add(1000);
            _itemIDs.Add(1001); // TODO : 추후 스테이지에 나오는 아이템 리스트 테이블 필요
            if(_itemCodes != null)
                _itemIDs.AddRange(_itemCodes);
            for (int i = 0; i < _shopItemTransforms.Length; i++)
            {
                if (_itemIDs.Count == 0)
                    break;
                var item = Instantiate(_itemObject, _shopItemTransforms[i]).GetComponent<ItemObject>();
                item.gameObject.SetActive(true);
                int randomRange = Random.Range(0, _itemIDs.Count);
                item.ItemID = _itemIDs[randomRange];
                _itemIDs.RemoveAt(randomRange);
            }
        }
    }
}
