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
        public async UniTask StageLoad(int stageNumber)
        {
            var stageNumberString = stageNumber.ToString();
            
            SystemManager.Instance.LoadingManager.MapReLoad();
            
            await UniTask.WhenAll(SystemManager.Instance.GetSystem<DungeonMapSystem>().MapLoad(stageNumberString),
                SystemManager.Instance.GetSystem<DungeonMapSystem>().ShopLoad(stageNumberString),
                SystemManager.Instance.GetSystem<DungeonMapSystem>().StartRoomLoad(stageNumberString),
                SystemManager.Instance.GetSystem<DungeonMapSystem>().BossRoomLoad(stageNumberString),
                SystemManager.Instance.GetSystem<DungeonMapSystem>().StairsRoomLoad(stageNumberString),
                SystemManager.Instance.GetSystem<DungeonMapSystem>().RewardRoomLoad(stageNumberString),
                SystemManager.Instance.GetSystem<DungeonMapSystem>().HpHealRoomLoad(stageNumberString));

            SystemManager.Instance.GetSystem<DungeonMapSystem>().SetFloor(stageNumber - 1);
            
            SystemManager.Instance.LoadingManager.IsMapLoaded();
        }
    }
}
