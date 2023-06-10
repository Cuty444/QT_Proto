using System.Collections;
using System.Collections.Generic;
using QT.Core;
using UnityEngine;

namespace QT
{
    public class ProductialMapGameData  : IGameData
    {
        public int Index { get; set; }

        public int MaxRoomValue { get; set; }
        public float GoldShopProb { get; set; }
        public float HpShopProb { get; set; }
        public float EnemyHpIncreasePer { get; set; }
        public int ItemDropId { get; set; }
        public int HpShopDropId { get; set; }
        public int GoldShopDropId { get; set; }
        public int BossDropId { get; set; }
        public int ChoiceDropId { get; set; }
        public int WeaponDropId { get; set; }
    }
    
    [GameDataBase(typeof(ProductialMapGameData), "ProductialMapGameData")]
    public class ProductialMapGameDataBase : IGameDataBase
    {
        private readonly Dictionary<int, ProductialMapGameData> _datas = new();

        public void RegisterData(IGameData data)
        {
            _datas.Add(data.Index, (ProductialMapGameData)data);
        }

        public ProductialMapGameData GetData(int id)
        {
            if (_datas.TryGetValue(id, out var value))
            {
                return value;
            }

            return null;
        }
    }  
}
