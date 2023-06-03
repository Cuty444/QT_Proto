using System.Collections.Generic;
using QT.InGame;
using UnityEngine;
using UnityEngine.Events;
using QT.Map;

namespace QT.Core
{
    public class PlayerManager
    {
        public UnityEvent<Player> PlayerCreateEvent  { get; } = new();
        
        public UnityEvent<Vector2Int> PlayerMapPosition { get; } = new();
        public UnityEvent<Vector2Int> PlayerMapVisitedPosition { get; } = new();
        public UnityEvent<Vector2Int> PlayerMapClearPosition { get; } = new();
        public UnityEvent<Vector2Int> PlayerDoorEnter { get; } = new();
        public UnityEvent<bool> PlayerMapPass { get; } = new();
        public UnityEvent<List<Enemy>> CurrentRoomEnemyRegister { get; } = new();
        public UnityEvent<Collider2D> PlayerDoorEnterCameraShapeChange { get; } = new();
        
        public UnityEvent PlayerThrowProjectileReleased { get; } = new();
        public UnityEvent<Vector2, float> OnDamageEvent { get; } = new();
        
        public UnityEvent<int> OnGoldValueChanged { get; } = new();

        public Player Player { get; private set; }
        
        public async void CreatePlayer() // 추후 로그라이크맵 절차 생성 SystemManager에서 관리하도록 코드 위치 변경이 필요함
        {
            Player = await SystemManager.Instance.ResourceManager.GetFromPool<Player>(Constant.PlayerPrefabPath);
            Player.transform.localPosition = new Vector3(0f, 6f, 0f);
            
            PlayerCreateEvent.Invoke(Player);
        }
    }
}
