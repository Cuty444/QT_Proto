using System.Collections;
using System.Collections.Generic;
using QT.Core;
using QT.Core.Map;
using UnityEngine;

namespace QT
{
    public class Exit : MonoBehaviour
    {
        
        private void OnTriggerEnter2D(Collider2D col)
        {
            //SystemManager.Instance.PlayerManager.Player.Pause(true);
            
            SystemManager.Instance.GetSystem<DungeonMapSystem>().DungenMapGenerate();
            
            SystemManager.Instance.PlayerManager.Reset();;
            SystemManager.Instance.LoadingManager.LoadScene(SceneNumber.InGame);
        }
    }
}
