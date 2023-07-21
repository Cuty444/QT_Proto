using System;
using System.Collections.Generic;
using System.Linq;
using QT.Core;
using UnityEngine;
using Cinemachine;
using QT.Core.Data;
using QT.UI;
using UnityEngine.UI;

namespace QT.InGame
{
    public partial class Player : FSMPlayer<Player>, IFSMEntity, IHitable
    {
        public enum States : int
        {
            Global,
            Move,
            Swing,
            Throw,
            Gain,
            Teleport,
            Dodge,
            Fall,
            Dead,
        }

        [SerializeField] private int _characterID = 100;
        [SerializeField] private int _characterAtkID = 200;
        
        [field:SerializeField] public Transform EyeTransform { get; private set; }
        [SerializeField] private Transform _batTransform;
        [SerializeField] private SpriteRenderer _batSpriteRenderer;
        [field:SerializeField] public Transform TeleportLineTransform { get; private set; }
        
        public PlayerStatComponent StatComponent { get; private set; }
        public BuffComponent BuffComponent { get; private set; }
        
        public Inventory Inventory { get; private set; }
        public Animator Animator;
        public Rigidbody2D Rigidbody { get; private set; }
        
        public PlayerProjectileShooter ProjectileShooter { get; private set; }
        
        public EnemySkeletalMaterialChanger MaterialChanger { get; private set; }

        private CharacterGameData _data;
        private CharacterAtkGameData _atkData;
        private PlayerManager _playerManager;

        private bool _isEnterDoor;

        private int _goldCost = 0;
        private PlayerHPCanvas _playerHpCanvas;

        [HideInInspector]public List<IHitable> _floorAllHit = new List<IHitable>();

        [SerializeField] private Transform _attackSpeedCanvas;
        [SerializeField] private Transform[] _attackSpeedBackground;
        [SerializeField] private Image[] _attackGaugeImages;
        
        
        [field:SerializeField] public CinemachineImpulseSource DamageImpulseSource { get; private set; }
        [field: SerializeField] public float DamageImpulseForce { get; private set; } = 3;
        [field:SerializeField] public CinemachineImpulseSource AttackImpulseSource { get; private set; }
        [field: SerializeField] public float AttackImpulseForce { get; private set; } = 2;
        [field:SerializeField] public CinemachineImpulseSource TeleportImpulseSource { get; private set; }
        [field: SerializeField] public float TeleportImpulseForce { get; private set; } = 0.2f;
        
        
        
        [HideInInspector] public bool IsFall;
        [HideInInspector] public FallObject EnterFallObject;
        [HideInInspector] public Vector2 DodgePreviousPosition;
        [HideInInspector] public int FallPreviousState;
        [HideInInspector] public bool IsGarden;
        
        
        private void Awake()
        {
            _data = SystemManager.Instance.DataManager.GetDataBase<CharacterGameDataBase>().GetData(_characterID);
            _atkData = SystemManager.Instance.DataManager.GetDataBase<CharacterAtkGameDataBase>().GetData(_characterAtkID);
            StatComponent = new PlayerStatComponent(_data, _atkData);

            BuffComponent = GetComponent<BuffComponent>();
            BuffComponent.Init(StatComponent);
            
            OnAim.RemoveAllListeners();
            Rigidbody = GetComponent<Rigidbody2D>();
            SwingAreaMeshFilter = GetComponentInChildren<MeshFilter>();
            SwingAreaMeshRenderer = GetComponentInChildren<MeshRenderer>();
            SwingAreaMeshRenderer.material.color = new Color(0.345098f, 1f, 0.8823529f, 0.6f);
            ProjectileShooter = GetComponent<PlayerProjectileShooter>();
            //Animator = GetComponentInChildren<Animator>();
            MaterialChanger = GetComponentInChildren<EnemySkeletalMaterialChanger>();
            _attackSpeedColorGradient = SystemManager.Instance.GetSystem<GlobalDataSystem>().GlobalData.AttackSpeedColorCurve;
            InitInputs();
            
            EffectSetup();

            Inventory = new Inventory(this);
            _playerManager = SystemManager.Instance.PlayerManager;
            var items = _playerManager._playerIndexInventory;
            for (int i = 0; i < items.Count; i++)
            {
                Inventory.NextCopyItem(items[i]);
            }
            SetUp(States.Move);
            SetGlobalState(new PlayerGlobalState(this));

            _goldCost = _playerManager.globalGold;
            _playerHpCanvas = SystemManager.Instance.UIManager.GetUIPanel<PlayerHPCanvas>();
            StatComponent.GetStatus(PlayerStats.HP).SetStatus(StatComponent.GetStatus(PlayerStats.HP).Value);
            _playerHpCanvas.SetHp(StatComponent.GetStatus(PlayerStats.HP));
            
            _playerManager.CurrentRoomEnemyRegister.AddListener((hitables) =>
            {
                _hitableList.Clear();
                _hitableList.AddRange(hitables);
            });
            
            _playerManager.FloorAllHitalbeRegister.AddListener((hitalbes) =>
            {
                _floorAllHit.Clear();
                _floorAllHit.AddRange(hitalbes);
            });
            
            _playerManager.PlayerMapClearPosition.AddListener((arg) =>
            {
                _hitableList.Clear();
            });
            _playerManager.PlayerMapPass.AddListener((isBool) =>
            {
                _isEnterDoor = isBool;
            });
            _isEnterDoor = true;
            
            _playerManager.OnGoldValueChanged.Invoke(_goldCost);
            _playerManager.OnGoldValueChanged.AddListener((value) =>
            {
                _goldCost = value;
            });
            
            _playerManager.GainItemSprite.AddListener(GainItem);
            
            SystemManager.Instance.UIManager.GetUIPanel<MinimapCanvas>().OnOpen();
            
            SystemManager.Instance.RankingManager.PlayerOn.Invoke(true);
        }

        private void Start()
        {
            MaterialChanger.SetHitDuration(_data.MercyInvincibleTime);
        }

        protected override void Update()
        {
            base.Update();
            
            UpdateInputs();
            UpdateCoolTime();
        }
        
        public void Hit(Vector2 dir, float power,AttackType attackType)
        {
            if (IsInvincible())
            {
                return;
            }
            StatComponent.GetStatus(PlayerStats.MercyInvincibleTime).SetStatus(0);
 
            _playerManager.OnDamageEvent.Invoke(dir, power);
        }

        public float GetHp()
        {
            return StatComponent.GetStatus(PlayerStats.HP).StatusValue;
        }

        public Vector2 GetPosition()
        {
            return transform.position;
        }
        
        public int GetGoldCost()
        {
            return _goldCost;
        }

        public bool GetGoldComparison(int cost)
        {
            return _goldCost >= cost;
        }

        public bool GetHpComparision(int hpCost)
        {
            return StatComponent.GetStatus(PlayerStats.HP) > hpCost;
        }
        
        public void PlayerDead()
        {
            SystemManager.Instance.PlayerManager._playerIndexInventory.Clear();
            SystemManager.Instance.PlayerManager.globalGold = 0;
            SystemManager.Instance.PlayerManager.PlayerThrowProjectileReleased.RemoveAllListeners();
            SystemManager.Instance.PlayerManager.OnDamageEvent.RemoveAllListeners();
            ChangeState(Player.States.Dead);
        }
    }
}
