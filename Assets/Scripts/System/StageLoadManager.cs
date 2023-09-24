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
        public void StageLoad(string stageNumber)
        {
            Debug.Log("맵 로드 시작");
            SystemManager.Instance.LoadingManager.MapReLoad();
            MapLoad(stageNumber);
        }

        private async UniTaskVoid MapLoad(string stageNumber)
        {
            await UniTask.WhenAll(SystemManager.Instance.GetSystem<DungeonMapSystem>().MapLoad(stageNumber),
                SystemManager.Instance.GetSystem<DungeonMapSystem>().ShopLoad(stageNumber),
                SystemManager.Instance.GetSystem<DungeonMapSystem>().StartRoomLoad(stageNumber),
                SystemManager.Instance.GetSystem<DungeonMapSystem>().BossRoomLoad(stageNumber),
                SystemManager.Instance.GetSystem<DungeonMapSystem>().StairsRoomLoad(stageNumber),
                SystemManager.Instance.GetSystem<DungeonMapSystem>().RewardRoomLoad(stageNumber),
                SystemManager.Instance.GetSystem<DungeonMapSystem>().HpHealRoomLoad(stageNumber));

            SystemManager.Instance.LoadingManager.DataMapLoadCompletedEvent.Invoke();
        }
    }
}
