using System.Collections;
using System.Collections.Generic;
using QT.Core;
using QT.InGame;
using UnityEngine;
using UnityEngine.Events;

namespace QT
{
    public class Item
    {
        private readonly int _itemDataId;
        public ItemGameData ItemGameData { get; private set; }
        public List<ItemEffectOld> ItemEffectData { get; private set; } = new ();
        
        private readonly Dictionary<ItemEffectGameData.ApplyPoints, UnityEvent> _applyPointEvents = new ();

        public Item(int itemDataId)
        {
            var dataManager = SystemManager.Instance.DataManager;

            _itemDataId = itemDataId;
            ItemGameData = dataManager.GetDataBase<ItemGameDataBase>().GetData(_itemDataId);
            if (ItemGameData != null)
            {
                ItemEffectData = dataManager.GetDataBase<ItemEffectGameDataBase>()
                    .GetData(ItemGameData.ItemEffectDataId);
            }
            else
            {
                Debug.LogError($"존재하지 않는 아이템 아이디 : {itemDataId}");
            }
        }

        public void ApplyItemEffect(Player player)
        {
            foreach (var effect in ItemEffectData)
            {
                effect.ApplyEffect(player, this);

                SetApplyPoint(effect, player);
            }
        }

        private void SetApplyPoint(ItemEffectOld effect, Player player)
        {
            if (effect.ApplyPoints == ItemEffectGameData.ApplyPoints.Equip)
            {
                return;
            }
            
            if (!_applyPointEvents.TryGetValue(effect.ApplyPoints, out var events))
            {
                events = new UnityEvent();
                _applyPointEvents.Add(effect.ApplyPoints, events);
            }

            events.AddListener(() => ReapplyItemEffect(player, effect));
        }
        
        private void ReapplyItemEffect(Player player, ItemEffectOld effect)
        {
            effect.RemoveEffect(player, this);
            effect.ApplyEffect(player, this);
        }
        
        public void InvokeApplyPoint(ItemEffectGameData.ApplyPoints applyPoints)
        {
            if (applyPoints == ItemEffectGameData.ApplyPoints.Equip)
            {
                return;
            }
            
            if (_applyPointEvents.TryGetValue(applyPoints, out var events))
            {
                events.Invoke();
            }
        }
        
        public void RemoveItemEffect(Player player)
        {
            foreach (var effect in ItemEffectData)
            {
                effect.RemoveEffect(player, this);
            }
        }

        public int GetItemID()
        {
            return _itemDataId;
        }
    }
}
