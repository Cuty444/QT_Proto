using System.Collections;
using System.Collections.Generic;
using QT.Core;
using QT.Core.Map;
using UnityEngine;

namespace QT
{
    public class Gate : MonoBehaviour
    {
        public bool IsTutorialGate;
        
        private void OnTriggerEnter2D(Collider2D col)
        {
            //SystemManager.Instance.PlayerManager.Player.Pause(true);

            if (IsTutorialGate)
            {
                SystemManager.Instance.GetSystem<DungeonMapSystem>().TutorialMapGenerate();
            }
            else
            {
                SystemManager.Instance.GetSystem<DungeonMapSystem>().DungenMapGenerate();
            }

            SystemManager.Instance.PlayerManager.Reset();
            SystemManager.Instance.LoadingManager.LoadScene(SceneNumber.InGame);
        }
    }
}
