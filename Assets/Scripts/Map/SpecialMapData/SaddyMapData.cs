using QT.Map;
using UnityEngine;

namespace QT
{
    public class SaddyMapData : SpecialMapData
    {
        [field:SerializeField] public EnemyWave BossWave { get; private set; }

        [field: SerializeField] public Transform PingPongReadyPoint { get; private set; }
        
        [field:SerializeField] public GameObject WallObject { get; private set; }
    }
}
