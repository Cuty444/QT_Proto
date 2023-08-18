using System;
using System.Collections;
using System.Collections.Generic;
using QT.Core;
using QT.InGame;
using UnityEngine;

namespace QT
{
    public class ItemDataManager
    {
        private Dictionary<ItemGameData.GradeTypes, List<int>> _itemGradeDictionary = new();
        private Dictionary<DropGameType, DropPercentage> _dropPercentagesList = new();

        private GameDataManager _gameDataManager;
        private Player _player;
        
        public void Initialize()
        {
            SystemManager.Instance.LoadingManager.DataJsonLoadCompletedEvent.AddListener(() =>
            {
                _gameDataManager = SystemManager.Instance.DataManager;
                DropGameDataLoad();
                ItemDataLoad();
            });
            
            SystemManager.Instance.PlayerManager.PlayerCreateEvent.AddListener((player) =>
            {
                _player = player;
            });
        }

        public List<int> GetDropItemList(DropGameType dropGameType, int max)
        {
            List<int> itemIds = new List<int>();
            Item[] itemArray = _player.Inventory.GetItemList();
            for (int i = 0; itemIds.Count < max; i++)
            {
                var typeData = _dropPercentagesList[dropGameType].RandomGradeType();

                int itemID = _itemGradeDictionary[typeData][UnityEngine.Random.Range(0, _itemGradeDictionary[typeData].Count)];
                if (!itemIds.Contains(itemID))
                {
                    bool bCheck = false;
                    for (int j = 0; j < itemArray.Length; j++)
                    {
                        if (itemArray[j].ItemGameData.Index == itemID)
                        {
                            bCheck = true;
                            break;
                        }
                    }

                    if (!bCheck)
                    {
                        itemIds.Add(itemID);
                    }
                }
            }

            return itemIds;
        }

        private void DropGameDataLoad()
        {
            int index = 9000;
            for (int i = 0; i < 10; i++)
            {
                var data = _gameDataManager.GetDataBase<DropGameDataBase>().GetData(index + i + 1);
                
                if (data == null)
                    break;
                _dropPercentagesList.Add((DropGameType) i, data);
            }
        }

        private void ItemDataLoad()
        {
            _itemGradeDictionary.Add(ItemGameData.GradeTypes.Normal, new List<int>());
            _itemGradeDictionary.Add(ItemGameData.GradeTypes.Rare, new List<int>());
            _itemGradeDictionary.Add(ItemGameData.GradeTypes.Cursed, new List<int>());
            for (int index = 2000; index <= 4000; index += 1000)
            {
                ItemCheck(index, _itemGradeDictionary[(ItemGameData.GradeTypes) (index / 1000 - 2)]);
            }
        }

        private void ItemCheck(int itemID, List<int> itemIDs)
        {
            for (int i = 0; i < 999; i++)
            {
                var data = _gameDataManager.GetDataBase<ItemGameDataBase>().GetData(itemID + i);
                if (data != null)
                {
                    itemIDs.Add(itemID + i);
                }
                else
                {
                    break;
                }
            }
        }
        
    }
}