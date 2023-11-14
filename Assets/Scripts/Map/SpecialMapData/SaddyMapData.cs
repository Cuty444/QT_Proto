using QT.Map;
using UnityEngine;

namespace QT
{
    public class SaddyMapData : SpecialMapData,IBossClearEvent
    {
        [field:SerializeField] public EnemyWave BossWave { get; private set; }

        [field:SerializeField] public BoxCollider2D PingPongAreaCollider { get; private set; }
        [field: SerializeField] public Transform PingPongReadyPoint { get; private set; }
        [field: SerializeField] public Transform PingPongPlayerReadyPoint { get; private set; }
        
        [field:SerializeField] public GameObject BarrierObject { get; private set; }
        [SerializeField] private GameObject _caseterNPC;
        private void Start()
        {
            _caseterNPC.gameObject.SetActive(false);
        }
        public void BossClear()
        {
            _caseterNPC.gameObject.SetActive(true);
        }
    }
}
