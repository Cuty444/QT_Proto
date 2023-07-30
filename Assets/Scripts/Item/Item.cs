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
                        if (data.ApplyBuffId != 0)
                        {
                            var effect = ItemEffectFactory.GetEffect(ItemEffectTypes.Buff, player, data);
                            _itemEffectList.Add(effect);
                            SetTrigger(effect);
                        }

                        if (data.ApplySpecialEffectId != 0)
                        {
                            var specialData = dataManager.GetDataBase<SpecialEffectGameDataBase>().GetData(data.ApplySpecialEffectId);

                            var effect = ItemEffectFactory.GetEffect(specialData.ActiveEffectType, player, data, specialData);
                            _itemEffectList.Add(effect);
                            SetTrigger(effect);
                        }
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
            foreach (TriggerTypes value in TriggerTypes.GetValues(typeof(TriggerTypes)))
            {
                if (value == TriggerTypes.Equip)
                {
                    continue;
                }
                
                if (effect.Data.TriggerType.HasFlag(value))
                {  
                    if (!_applyPointEvents.TryGetValue(value, out var events))
                    {
                        events = new UnityEvent();
                        _applyPointEvents.Add(value, events);
                    }
        
                    events.AddListener(effect.OnTrigger);
                }
            }
           
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
                
                if (effect.Data.TriggerType.HasFlag(TriggerTypes.Equip))
                {
                    effect.OnTrigger();
                }
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
