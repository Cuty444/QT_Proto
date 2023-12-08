using System.Collections;
using System.Collections.Generic;
using QT.Core;
using QT.InGame;
using QT.UI;
using UnityEngine;

namespace QT
{
    public class DungeonManagerLobby : DungeonManager
    {
        public override bool IsBattle => true;
        
        private new void Start()
        {
            SystemManager.Instance.PlayerManager.PlayerCreateEvent.AddListener(OnPlayerCreated);
            
            SystemManager.Instance.PlayerManager.CreatePlayer();
        }
        
        private new void OnDestroy()
        {
        }
        
        private void OnPlayerCreated(Player player)
        {
            //SystemManager.Instance.PlayerManager.OnMapCellChanged.Invoke(Target.VolumeProfile, Target.CameraSize);

            SystemManager.Instance.UIManager.SetState(UIState.None);
            
            SystemManager.Instance.PlayerManager.PlayerCreateEvent.RemoveListener(OnPlayerCreated);
        }

    }
}
