using System;
using System.Collections;
using System.Collections.Generic;
using QT.Core;
using QT.InGame;
using UnityEngine;

namespace QT
{
    [Serializable]
    public enum DropGameType
    {
        ItemReward,
        HpShop,
        GoldShop,
        Select,
        Boss,
    }

    public class DropPercentage
    {
        public float Normal;
        public float Rare;
        public float Cursed;
        public float Hp;
        public float Gold;

        public DropPercentage(float normal, float rare, float cursed, float hp, float gold)
        {
            Normal = normal;
            Rare = Normal + rare;
            Cursed = Rare + cursed;
            Hp = Cursed + hp;
            Gold = Hp + gold;
        }

        public ItemGameData.GradeTypes RandomGradeType()
        {
            int key = UnityEngine.Random.Range(0, 10000);
            if (key < Normal)
            {
                return ItemGameData.GradeTypes.Normal;
            }
            else if (key < Rare)
            {
                return ItemGameData.GradeTypes.Rare;
            }
            else if (key < Cursed)
            {
                return ItemGameData.GradeTypes.Cursed;
            }
            else if (key < Hp)
            {
                return ItemGameData.GradeTypes.Normal;
            }
            else if (key < Gold)
            {
                return ItemGameData.GradeTypes.Normal;
            }

            return ItemGameData.GradeTypes.Normal;
        }
    }

    public class ItemDataManager
    {
        private Dictionary<ItemGameData.GradeTypes, List<int>> _itemGradeDictionary =
            new Dictionary<ItemGameData.GradeTypes, List<int>>();

        private Dictionary<DropGameType, DropPercentage> _dropPercentagesList =
            new Dictionary<DropGameType, DropPercentage>();

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
                        if (itemArray[j].GetItemID() == itemID)
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
                var data = GetDropData(index + i);
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

        private DropPercentage GetDropData(int dropID)
        {
            var data = _gameDataManager.GetDataBase<DropGameDataBase>().GetData(dropID);
            if (data == null)
                return null;
            return new DropPercentage(data.Normal, data.Rare, data.Cursed, data.Hp, data.Gold);
        }
    }
}