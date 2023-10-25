using System;
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
        public Action<int> OnStackChanged;
        public int Stack { get; private set; }

        private readonly int _itemDataId;
        public ItemGameData ItemGameData { get; }
        private List<ItemEffectGroup> _itemEffectList;
        
        private readonly Dictionary<TriggerTypes, UnityEvent> _applyPointEvents = new ();
        
        
        public Item(int itemDataId, Player player)
        {
            var dataManager = SystemManager.Instance.DataManager;

            _itemDataId = itemDataId;
            ItemGameData = dataManager.GetDataBase<ItemGameDataBase>().GetData(_itemDataId);
            
            if (ItemGameData != null)
            {
                _itemEffectList = new List<ItemEffectGroup>();
                
                var datas = dataManager.GetDataBase<ItemEffectGameDataBase>()
                    .GetData(ItemGameData.ItemEffectDataId);

                foreach (var data in datas)
                {
                    if (data != null)
                    {
                        var effectGroup = new ItemEffectGroup(this, player, data);
                        
                        if (data.ApplyBuffId != 0)
                        {
                            var effect = ItemEffectFactory.GetEffect(this, ItemEffectTypes.Buff, player, data);
                            effectGroup.Add(effect);
                        }

                        if (data.ApplySpecialEffectId != 0)
                        {
                            var specialDatas = dataManager.GetDataBase<SpecialEffectGameDataBase>().GetData(data.ApplySpecialEffectId);

                            foreach (var specialData in specialDatas)
                            {
                                var effect = ItemEffectFactory.GetEffect(this, specialData.EffectType, player, data, specialData);
                                effectGroup.Add(effect);
                            }
                        }
                        
                        _itemEffectList.Add(effectGroup);
                        SetTrigger(effectGroup);
                    }
                }
            }
            else
            {
                Debug.LogError($"존재하지 않는 아이템 아이디 : {itemDataId}");
            }
        }
        
        private void SetTrigger(ItemEffectGroup effect)
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

        public void AddStack(int amount)
        {
            if (ItemGameData.MaxStack == -1)
            {
                return;
            }
            
            Stack = Mathf.Clamp(Stack + amount, 0, ItemGameData.MaxStack);

            OnStackChanged?.Invoke(Stack);
        }
        
        public void ClearStack()
        {
            Stack = 0;
            OnStackChanged?.Invoke(Stack);
        }

        public float GetCoolTimeProgress()
        {
            float result = 0;
            foreach (var effect in _itemEffectList)
            {
                result = Mathf.Max(result, effect.GetCoolTimeProgress());
            }

            return result;
        }
        
    }
}
