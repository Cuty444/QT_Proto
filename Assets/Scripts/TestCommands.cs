using System.Collections;
using System.Collections.Generic;
using IngameDebugConsole;
using QT.Core;
using QT.Core.Map;
using QT.InGame;
using UnityEngine;

namespace QT
{
    public static class TestCommands
    {
        [ConsoleMethod("AddItem", "아이템 추가")]
        public static void AddItem(int id)
        {
            SystemManager.Instance.PlayerManager.Player.Inventory.AddItem(id);
        }
        
        
        [ConsoleMethod("RemoveItem", "아이템 제거")]
        public static void RemoveItem(int index)
        {
            SystemManager.Instance.PlayerManager.Player.Inventory.RemoveItem(index);
        }
        
        [ConsoleMethod("PowerOver", "무적")]
        public static void PowerOver()
        {
            SystemManager.Instance.PlayerManager.Player.GetStatus(PlayerStats.MercyInvincibleTime).AddModifier(new StatModifier(999999,StatModifier.ModifierType.Addition,null));
            Debug.Log("Power Overwhelming");
        }

        [ConsoleMethod("Boss", "보스방 강제 이동")]
        public static void Boss()
        {
            SystemManager.Instance.PlayerManager.PlayerMapPosition.Invoke(SystemManager.Instance.GetSystem<DungeonMapSystem>().DungeonMapData.BossRoomPosition - Vector2Int.down);
            SystemManager.Instance.PlayerManager.PlayerDoorEnter.Invoke(Vector2Int.up);
        }
        
        [ConsoleMethod("RoomMove", "방 생성순서 강제 이동")]
        public static void RoomMove(int index)
        {
            if (index >= SystemManager.Instance.GetSystem<DungeonMapSystem>().DungeonMapData.MapNodeList.Count)
            {
                Debug.Log("방 최대치를 넘김 Index 숫자를 줄여주세요");
                return;
            }
            SystemManager.Instance.PlayerManager.PlayerMapPosition.Invoke(SystemManager.Instance.GetSystem<DungeonMapSystem>().DungeonMapData.MapNodeList[index] - Vector2Int.down);
            SystemManager.Instance.PlayerManager.PlayerDoorEnter.Invoke(Vector2Int.up);
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
            SystemManager.Instance.PlayerManager.StairNextRoomEvent.Invoke();
        }
    }
}
