using System;
using System.Collections.Generic;
using System.Linq;
using QT.Core;
using UnityEngine;
using Cinemachine;
using QT.Core.Data;
using QT.UI;
using UnityEngine.UI;
using Spine.Unity;
using EventType = QT.Core.EventType;

namespace QT.InGame
{
    public partial class Player : FSMPlayer<Player>, IFSMEntity, IHitAble
    {
        public enum States : int
        {
            Global,
            
            Move,
            Swing,
            Dodge,
            Fall,
            Dead,
            
            Empty,
        }

        public int InstanceId => gameObject.GetInstanceID();
        public Vector2 Position => transform.position;
        public float ColliderRad => StatComponent.GetStat(PlayerStats.PCHitboxRad) * 0.5f;
        public bool IsClearTarget => false;
        public bool IsDead => CurrentStateIndex == (int) States.Dead;
        
        
        [SerializeField] private int _characterID = 100;
        [SerializeField] private int _characterAtkID = 200;
        
        [field:SerializeField] public Transform EyeTransform { get; private set; }
        [field:SerializeField] public Transform CenterTransform { get; private set; }
        
        public PlayerStatComponent StatComponent { get; private set; }
        public BuffComponent BuffComponent { get; private set; }
        public Inventory Inventory { get; private set; }
        
        public Animator Animator;
        public Rigidbody2D Rigidbody { get; private set; }
        
        public MeshFilter SwingAreaMeshFilter { get; private set; }
        public MeshRenderer SwingAreaMeshRenderer { get; private set; }
        
        public PlayerProjectileShooter ProjectileShooter { get; private set; }
        
        public EnemySkeletalMaterialChanger MaterialChanger { get; private set; }
        public SkeletonGhost GhostEffect { get; private set; }
        
        private PlayerManager _playerManager;

        private PlayerHPCanvas _playerHpCanvas;

        [SerializeField] private Transform _attackSpeedCanvas;
        [SerializeField] private Transform[] _attackSpeedBackground;
        [SerializeField] private Image[] _attackGaugeImages;
        
        [field:SerializeField] public CinemachineImpulseSource DamageImpulseSource { get; private set; }
        [field: SerializeField] public float DamageImpulseForce { get; private set; } = 3;
        [field:SerializeField] public CinemachineImpulseSource AttackImpulseSource { get; private set; }
        [field: SerializeField] public float AttackImpulseForce { get; private set; } = 2;
        [field:SerializeField] public CinemachineImpulseSource TeleportImpulseSource { get; private set; }
        [field: SerializeField] public float TeleportImpulseForce { get; private set; } = 0.2f;
        
        [field: SerializeField] public GameObject PlayerFocusCam { get; private set; }
        
        
        [HideInInspector] public bool IsGarden;
        
        public Vector2 LastSafePosition { get; set; }
        
        
        private void Awake()
        {
            var data = SystemManager.Instance.DataManager.GetDataBase<CharacterGameDataBase>().GetData(_characterID);
            var atkData = SystemManager.Instance.DataManager.GetDataBase<CharacterAtkGameDataBase>().GetData(_characterAtkID);
            StatComponent = new PlayerStatComponent(data, atkData);
            
            BuffComponent = GetComponent<BuffComponent>();
            BuffComponent.Init(StatComponent);
            
            ProjectileShooter = GetComponent<PlayerProjectileShooter>();
            ProjectileShooter.Init(StatComponent);
            
            OnAim.RemoveAllListeners();
            Rigidbody = GetComponent<Rigidbody2D>();
            
            
            var globalData = SystemManager.Instance.GetSystem<GlobalDataSystem>().GlobalData;
            
            SwingAreaMeshFilter = GetComponentInChildren<MeshFilter>();
            SwingAreaMeshRenderer = GetComponentInChildren<MeshRenderer>();
            SwingAreaMeshRenderer.material.color = globalData.SwingAreaColor;
            
            MaterialChanger = GetComponentInChildren<EnemySkeletalMaterialChanger>();
            GhostEffect = GetComponentInChildren<SkeletonGhost>();
            
            _attackSpeedColorGradient = globalData.AttackSpeedColorCurve;
            InitInputs();
            
            _playerManager = SystemManager.Instance.PlayerManager;
            
            EffectSetup();
            
            Inventory = new Inventory(this);
            Inventory.CopyItemList(_playerManager.PlayerIndexInventory, _playerManager.PlayerActiveItemIndex);

            SetUp(States.Move);
            SetGlobalState(new PlayerGlobalState(this));

            _playerHpCanvas = SystemManager.Instance.UIManager.GetUIPanel<PlayerHPCanvas>();
            StatComponent.GetStatus(PlayerStats.HP).SetStatus(StatComponent.GetStatus(PlayerStats.HP).Value);
            
            SystemManager.Instance.UIManager.GetUIPanel<MinimapCanvas>()?.OnOpen();
            
            SystemManager.Instance.RankingManager.PlayerOn.Invoke(true);
        }

        protected override void Update()
        {
            base.Update();
            
            UpdateInputs();
            UpdateCoolTime();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            
            Inventory.ClearInventory();
        }

        public void Hit(Vector2 dir, float power,AttackType attackType)
        {
            SystemManager.Instance.EventManager.InvokeEvent(EventType.OnDamage, (dir, power));
        }
        
        public void Heal(float amount)
        {
            SystemManager.Instance.EventManager.InvokeEvent(EventType.OnHeal, amount);
        }
        
        public bool GetHpComparision(int hpCost)
        {
            return StatComponent.GetStatus(PlayerStats.HP) > hpCost;
        }
        
        public void PlayerDead()
        {
            _playerManager.PlayerIndexInventory.Clear();
            _playerManager.PlayerActiveItemIndex = -1;
            
            SystemManager.Instance.PlayerManager.Reset();;
            ChangeState(Player.States.Dead);
        }

        public void Pause(bool isPause)
        {
            PauseGame(isPause);
            
            PlayerFocusCam.SetActive(isPause);
            Animator.SetBool("IsPause", isPause);
        }

        public void Warp(Vector2Int cellPos)
        {
            PauseGame(true);
            Animator.SetTrigger("IsWarp");
            if (Animator.GetBool("IsPause"))
            {
                SystemManager.Instance.UIManager.GetUIPanel<UIPhoneCanvas>().CheckOpen();
            }

            StartCoroutine(Util.UnityUtil.WaitForFunc(() =>
            {
                _warpEffectParticle.Play();
                Animator.ResetTrigger("IsWarp");
                SystemManager.Instance.SoundManager.PlayOneShot(SystemManager.Instance.SoundManager.SoundData
                    .WarpMapSFX);
                StartCoroutine(Util.UnityUtil.WaitForFunc(() =>
                {
                    Animator.SetTrigger("IsWarp");
                    SystemManager.Instance.PlayerManager.PlayerMapTeleportPosition.Invoke(cellPos);
                    StartCoroutine(Util.UnityUtil.WaitForFunc(() => { PauseGame(false); }, 0.5f));
                }, 0.37f));
            }, 0.3f));
            Animator.ResetTrigger("Swing");
        }

        private void PauseGame(bool isPause)
        {
            if(isPause)
            {
                inputActions.Disable();
                Animator.updateMode = AnimatorUpdateMode.UnscaledTime;
            }
            else
            {
                inputActions.Enable();
                Animator.updateMode = AnimatorUpdateMode.Normal;
            }
        }
    }
}
