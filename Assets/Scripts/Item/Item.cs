using System.Collections;
using System.Collections.Generic;
using QT.Core;
using UnityEngine;
using UnityEngine.Events;
using TriggerTypes = QT.ItemEffectGameData.TriggerTypes;

namespace QT.InGame
{
    public class Item
    {
        private readonly int _itemDataId;
        public ItemGameData ItemGameData { get; }
        private List<ItemEffect> _itemEffectList;
        
        private readonly Dictionary<TriggerTypes, UnityEvent> _applyPointEvents = new ();

        public Item(int itemDataId, Player player)
        {
            var dataManager = SystemManager.Instance.DataManager;

            _itemDataId = itemDataId;
            ItemGameData = dataManager.GetDataBase<ItemGameDataBase>().GetData(_itemDataId);
            
            if (ItemGameData != null)
            {
                _itemEffectList = new List<ItemEffect>();
                
                var datas = dataManager.GetDataBase<ItemEffectGameDataBase>()
                    .GetData(ItemGameData.ItemEffectDataId);

                foreach (var data in datas)
                {
                    if (data != null)
                    {
                        var effect = new ItemEffect(data, player);
                        _itemEffectList.Add(effect);
                        
                        SetTrigger(effect);
                    }
                }
            }
            else
            {
                Debug.LogError($"존재하지 않는 아이템 아이디 : {itemDataId}");
            }
        }
        
        private void SetTrigger(ItemEffect effect)
        {
            if (effect.Data.TriggerType == TriggerTypes.Equip)
            {
                return;
            }
            
            if (!_applyPointEvents.TryGetValue(effect.Data.TriggerType, out var events))
            {
                events = new UnityEvent();
                _applyPointEvents.Add(effect.Data.TriggerType, events);
            }
        
            events.AddListener(effect.OnTrigger);
        }

        
        public void InvokeTrigger(TriggerTypes triggerTypes)
        {
            if (triggerTypes == TriggerTypes.Equip)
            {
                return;
            }
            
            if (_applyPointEvents.TryGetValue(triggerTypes, out var events))
            {
                events.Invoke();
            }
        }

        public void OnEquip()
        {
            foreach (var effect in _itemEffectList)
            {
                effect.OnEquip();
            }
        }

        public void OnRemoved()
        {
            foreach (var effect in _itemEffectList)
            {
                effect.OnRemoved();
            }
        }
        
    }
}
