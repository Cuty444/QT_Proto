using UnityEngine;
using UnityEngine.Events;
using QT.Data;
using QT.Map;

namespace QT.Core
{
    public class PlayerManager
    {
        public ChargeAtkPierce ChargeAtkPierce { get; set; }

        public UnityEvent<float> ChargeAtkShootEvent { get; } = new();
        public UnityEvent<int> BatSwingRigidHitEvent { get; } = new();
        public UnityEvent<int> ChargeBounceValueEvent { get; } = new();
        public UnityEvent BatSwingEndEvent { get; } = new();
        public UnityEvent<Player.Player> PlayerCreateEvent  { get; } = new();
        public UnityEvent<bool> DodgeEvent  { get; } = new();
        public UnityEvent PlayerCollisionEnemyEvent  { get; } = new();
        public UnityEvent<GameObject> PlayerBallAddedEvent  { get; } = new();
        public UnityEvent<float> PlayerCurrentChargingTimeEvent  { get; } = new();
        public UnityEvent<bool> BatSwingTimeScaleEvent { get; } = new();
        public UnityEvent BatSwingBallHitEvent  { get; } = new();

        public UnityEvent<Vector2Int> PlayerMapPosition { get; } = new();

        public UnityEvent<Vector2Int> PlayerMapVisitedPosition { get; } = new();
        
        public UnityEvent<Vector2Int> PlayerMapClearPosition { get; } = new();

        public UnityEvent<Vector2Int> PlayerDoorEnter { get; } = new();

        public UnityEvent PlayerThrowProjectileReleased { get; } = new();
        
        public Player.Player Player { get; private set; }

        public async void OnPlayerCreate() // 추후 로그라이크맵 절차 생성 SystemManager에서 관리하도록 코드 위치 변경이 필요함
        {
            Player = await SystemManager.Instance.ResourceManager.GetFromPool<Player.Player>(Constant.PlayerPrefabPath);
            PlayerCreateEvent.Invoke(Player);
        }
    }
}
