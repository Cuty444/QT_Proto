using System.Collections;
using System.Collections.Generic;
using IngameDebugConsole;
using QT.Core;
using QT.Core.Map;
using QT.InGame;
using UnityEngine;
using EventType = QT.Core.EventType;

namespace QT
{
    public static class TestCommands
    {
        [ConsoleMethod("AddItem", "아이템 추가")]
        public static void AddItem(int id)
        {
            SystemManager.Instance.PlayerManager.Player.AddItem(SystemManager.Instance.DataManager.GetDataBase<ItemGameDataBase>().GetData(id));
        }
        
        
        [ConsoleMethod("RemoveItem", "아이템 제거")]
        public static void RemoveItem(int index)
        {
            SystemManager.Instance.PlayerManager.Player.Inventory.RemoveItem(index);
        }
        
        
        [ConsoleMethod("Heal", "플레이어 회복")]
        public static void Heal(float amount)
        {
            SystemManager.Instance.EventManager.InvokeEvent(EventType.OnHeal, amount);
        }
        
        [ConsoleMethod("Hit", "플레이어 공격")]
        public static void Hit(float amount)
        {
            SystemManager.Instance.EventManager.InvokeEvent(EventType.OnDamage, (Vector2.zero, amount));
        }
        
        
        [ConsoleMethod("PowerOver", "무적")]
        public static void PowerOver()
        {
            SystemManager.Instance.PlayerManager.Player.StatComponent.GetStatus(PlayerStats.MercyInvincibleTime).AddModifier(new StatModifier(999999,StatModifier.ModifierType.Hard,null));
            SystemManager.Instance.PlayerManager.Player.StatComponent.GetStat(PlayerStats.AtkDmgPer).AddModifier(new StatModifier(999999,StatModifier.ModifierType.Hard,null));
            Debug.Log("Power Overwhelming");
        }

        [ConsoleMethod("Boss", "보스방 강제 이동")]
        public static void Boss()
        {
            SystemManager.Instance.PlayerManager.Player.Warp(SystemManager.Instance.GetSystem<DungeonMapSystem>().DungeonMapData.BossRoomPosition);
        }
        
        [ConsoleMethod("Shop", "상점방 강제 이동")]
        public static void Shop()
        {
            SystemManager.Instance.PlayerManager.Player.Warp(SystemManager.Instance.GetSystem<DungeonMapSystem>().DungeonMapData.ShopRoomPosition);
        }
        
        [ConsoleMethod("RoomMove", "방 생성순서 강제 이동")]
        public static void RoomMove(int index)
        {
            if (index >= SystemManager.Instance.GetSystem<DungeonMapSystem>().DungeonMapData.MapNodeList.Count)
            {
                Debug.Log("방 최대치를 넘김 Index 숫자를 줄여주세요");
                return;
            }
            
            SystemManager.Instance.PlayerManager.Player.Warp(SystemManager.Instance.GetSystem<DungeonMapSystem>().DungeonMapData.MapNodeList[index]);
        }
        [ConsoleMethod("FloorMove", "1~3 층으로 이동")]
        public static void FloorMove(int index)
        {
            if (index >= 4 && index <= 0)
            {
                Debug.Log("층 최대치를 넘김 Index 숫자를 줄여주세요");
                return;
            }
            SystemManager.Instance.GetSystem<DungeonMapSystem>().SetFloor(index - 2);
            SystemManager.Instance.GetSystem<DungeonMapSystem>().NextFloor();
        }
        
        [ConsoleMethod("Gold", "골드 획득")]
        public static void Gold(int value)
        {
            var _playerManager = SystemManager.Instance.PlayerManager;
            _playerManager.OnGoldValueChanged.Invoke(value);
        }
    }
}
