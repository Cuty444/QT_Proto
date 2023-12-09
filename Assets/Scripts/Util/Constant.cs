using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QT.Core
{
    public enum SceneNumber
    {
        Load = 0,
        Title = 1,
        Lobby = 2,
        Tutorial = 4,
        InGame = 3
    }
    
    
    public enum Progress
    {
        None,
        TutorialClear,
        
        JelloClear,
        SaddyClear,
        
        Clear
    }
    
    public static class Constant
    {
        public const string PlayerPrefabPath = "Prefabs/Player.prefab";
        public const string CoinPrefabPath = "Prefabs/Coin.prefab";

        public const string ProgressDataKey = "Progress";
    }
}
