using System.Collections;
using System.Collections.Generic;
using QT.Core;
using QT.Core.Map;
using UnityEngine;

namespace QT
{
    public class RewardMapData : SpecialMapData
    {
        [SerializeField] private Transform _rewardItemTransform;
        [SerializeField] private GameObject _itemObject;
        
        private void Awake()
        {
            SystemManager.Instance.PlayerManager.PlayerMapPosition.AddListener(ItemCreate);
        }

        private void ItemCreate(Vector2Int position)
        {
            if (position == MapPosition)
            {
                var percent = SystemManager.Instance.DataManager.GetDataBase<DropGameDataBase>().GetData((int)DropGameType.Shop);
                
                var item = SystemManager.Instance.DataManager.GetDataBase<ItemGameDataBase>().PickRandom(percent.RandomGradeType());
                
                var holder = Instantiate(_itemObject, _rewardItemTransform).GetComponent<ItemHolder>();
                    
                holder.gameObject.SetActive(true);
                holder.Init(item);
                SystemManager.Instance.PlayerManager.PlayerMapPosition.RemoveListener(ItemCreate);
            }
        }
    }
}