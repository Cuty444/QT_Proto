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
            
            await UniTask.WhenAll(SystemManager.Instance.GetSystem<DungeonMapSystem>().MapLoad(stageNumberString,RoomType.Normal),
                SystemManager.Instance.GetSystem<DungeonMapSystem>().MapLoad(stageNumberString,RoomType.Start),
                SystemManager.Instance.GetSystem<DungeonMapSystem>().MapLoad(stageNumberString,RoomType.GoldShop),
                SystemManager.Instance.GetSystem<DungeonMapSystem>().MapLoad(stageNumberString,RoomType.Boss),
                SystemManager.Instance.GetSystem<DungeonMapSystem>().MapLoad(stageNumberString,RoomType.Stairs),
                SystemManager.Instance.GetSystem<DungeonMapSystem>().MapLoad(stageNumberString,RoomType.Reward),
                SystemManager.Instance.GetSystem<DungeonMapSystem>().MapLoad(stageNumberString,RoomType.HpHeal),
                SystemManager.Instance.GetSystem<DungeonMapSystem>().MapLoad(stageNumberString,RoomType.Wait));

            SystemManager.Instance.GetSystem<DungeonMapSystem>().SetFloor(stageNumber - 1);
            
            SystemManager.Instance.LoadingManager.IsMapLoaded();
        }
        
        public async UniTask TutorialStage()
        {
            await UniTask.WhenAll(SystemManager.Instance.GetSystem<DungeonMapSystem>().MapLoad("Tuto",RoomType.Tutorial),
                SystemManager.Instance.GetSystem<DungeonMapSystem>().MapLoad("Tuto",RoomType.Start));
            
            SystemManager.Instance.GetSystem<DungeonMapSystem>().SetFloor(0);
            SystemManager.Instance.LoadingManager.IsMapLoaded();
        }
    }
}
