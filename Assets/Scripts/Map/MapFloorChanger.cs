using System.Collections;
using System.Collections.Generic;
using QT.Core.Map;
using QT.Core;
using UnityEngine;

//임시
namespace QT
{
    public class MapFloorChanger : MonoBehaviour
    { 
        public void Spawn()
        {
            if (transform.childCount == 0)
            {
                return;
            }

            var floor = SystemManager.Instance.GetSystem<DungeonMapSystem>().GetFloor();
            if (floor > transform.childCount - 1)
            {
                floor = transform.childCount - 1;
            }

            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).gameObject.SetActive(i == floor);
            }
        }
    }
}
