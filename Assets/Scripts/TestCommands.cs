using System.Collections;
using System.Collections.Generic;
using IngameDebugConsole;
using QT.Core;
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
    }
}
