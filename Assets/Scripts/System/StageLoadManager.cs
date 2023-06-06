using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using QT.Core;
using QT.Core.Map;
using UnityEngine;

namespace QT
{
    public class StageLoadManager
    {
        public void StageLoad(string stagePath)
        {
            Debug.Log("맵 로드 시작");
            MapLoad(stagePath);
        }

        private async UniTaskVoid MapLoad(string stagePath)
        {
            await UniTask.WhenAll(SystemManager.Instance.GetSystem<DungeonMapSystem>().MapLoad(),
                SystemManager.Instance.GetSystem<DungeonMapSystem>().ShopLoad(),
                SystemManager.Instance.GetSystem<DungeonMapSystem>().StartRoomLoad(),
                SystemManager.Instance.GetSystem<DungeonMapSystem>().BossRoomLoad());

            SystemManager.Instance.LoadingManager.DataMapLoadCompletedEvent.Invoke();
        }
    }
}
