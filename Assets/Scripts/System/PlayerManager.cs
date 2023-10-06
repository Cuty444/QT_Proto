using System.Collections.Generic;
using QT.InGame;
using UnityEngine;
using UnityEngine.Events;
using QT.Map;
using QT.UI;
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

        public UnityEvent<Vector2Int> PlayerMapTeleportPosition { get; } = new();
        public UnityEvent<bool> PlayerMapPass { get; } = new();

        public UnityEvent PlayerItemInteraction { get; } = new();
        //public UnityEvent<Vector2, float> OnDamageEvent { get; } = new();
        public UnityEvent<int> OnGoldValueChanged { get; } = new();

        public UnityEvent<VolumeProfile, float> OnMapCellChanged { get; } = new();

        public UnityEvent StairNextRoomEvent { get; } = new();
        
        public Player Player { get; private set; }
        public int PlayerActiveItemIndex = -1;
        public List<int> PlayerIndexInventory = new();

        public UnityEvent AddItemEvent { get; } = new();
        
        public UnityEvent<Sprite> GainItemSprite { get; } = new(); // TODO : 상점에서 얻거나, 획득시 분리한 이유는 시작할때 이전 방에서 아이템 로드하면서 꼬일수 있음...

        public int Gold { get; private set; } = 0;

        public PlayerManager()
        {
            OnGoldValueChanged.AddListener(AddGold);
        }
        
        public async void CreatePlayer()
        {
            PlayerMapPass.RemoveAllListeners();
            Player = await SystemManager.Instance.ResourceManager.GetFromPool<Player>(Constant.PlayerPrefabPath);
            Player.transform.localPosition = new Vector3(0f, 0f, 0f);
            SystemManager.Instance.SoundManager.PlayBGM(SystemManager.Instance.SoundManager.SoundData.Stage1BGM);
            PlayerCreateEvent.Invoke(Player);
            
        }

        private async void AddGold(int value)
        {
            if (value > 0)
            {
                value = (int)(value * Player.StatComponent.GetStat(PlayerStats.GoldGain).Value);
            }

            Gold = Mathf.Max(0, Gold + value);
            
            (await SystemManager.Instance.UIManager.Get<PlayerHPCanvasModel>()).SetGoldText(Gold);
            
            SystemManager.Instance.EventManager.InvokeEvent(EventType.OnGoldChanged, Gold);
        }
        
        public async void Reset()
        {
            Gold = 0;
            (await SystemManager.Instance.UIManager.Get<PlayerHPCanvasModel>()).SetGoldText(Gold);
        }
    }
}
