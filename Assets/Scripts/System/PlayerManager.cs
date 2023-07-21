using System.Collections.Generic;
using QT.InGame;
using UnityEngine;
using UnityEngine.Events;
using QT.Map;
using UnityEngine.Rendering;

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
        public UnityEvent<List<IHitable>> CurrentRoomEnemyRegister { get; } = new();
        public UnityEvent<Collider2D> PlayerDoorEnterCameraShapeChange { get; } = new();

        public UnityEvent PlayerItemInteraction { get; } = new();
        public UnityEvent<Vector2, float> OnDamageEvent { get; } = new();
        public UnityEvent PlayerThrowProjectileReleased { get; } = new();
        public UnityEvent<int> OnGoldValueChanged { get; } = new();

        public UnityEvent<VolumeProfile> OnVolumeProfileChange { get; } = new();

        public UnityEvent StairNextRoomEvent { get; } = new();
        
        public Player Player { get; private set; }

        public UnityEvent AddItemEvent { get; } = new();
        
        public UnityEvent<List<IHitable>> FloorAllHitalbeRegister { get; } = new();

        public UnityEvent FadeInCanvasOut { get; } = new();

        public UnityEvent<Sprite> GainItemSprite { get; } = new(); // TODO : 상점에서 얻거나, 획득시 분리한 이유는 시작할때 이전 방에서 아이템 로드하면서 꼬일수 있음...

        public int globalGold = 0;
        public async void CreatePlayer() // 추후 로그라이크맵 절차 생성 SystemManager에서 관리하도록 코드 위치 변경이 필요함
        {
            Player = await SystemManager.Instance.ResourceManager.GetFromPool<Player>(Constant.PlayerPrefabPath);
            Player.transform.localPosition = new Vector3(0f, 6f, 0f);
            SystemManager.Instance.SoundManager.PlayBGM(SystemManager.Instance.SoundManager.SoundData.Stage1BGM);
            PlayerCreateEvent.Invoke(Player);
        }
    }
}
