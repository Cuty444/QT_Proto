using System;
using System.Collections.Generic;
using QT.Core;
using UnityEngine;
using Cinemachine;
using QT.Core.Data;
using QT.UI;
using UnityEngine.UI;

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
            
            KnockBack,
            
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
        
        public SkeletalMaterialChanger MaterialChanger { get; private set; }
        public SkeletonGhost GhostEffect { get; private set; }
        
        private PlayerManager _playerManager;

        [SerializeField] private Transform _attackSpeedCanvas;
        [SerializeField] private Transform[] _attackSpeedBackground;
        [SerializeField] private Image[] _attackGaugeImages;
        
        private static readonly int IsPause = Animator.StringToHash("IsPause");
        private static readonly int IsWarp = Animator.StringToHash("IsWarp");
        private static readonly int Swing = Animator.StringToHash("Swing");

        [field:SerializeField] public CinemachineImpulseSource DamageImpulseSource { get; private set; }
        [field: SerializeField] public float DamageImpulseForce { get; private set; } = 3;
        [field:SerializeField] public CinemachineImpulseSource AttackImpulseSource { get; private set; }
        [field: SerializeField] public float AttackImpulseForce { get; private set; } = 2;
        [field:SerializeField] public CinemachineImpulseSource TeleportImpulseSource { get; private set; }
        [field: SerializeField] public float TeleportImpulseForce { get; private set; } = 0.2f;
        
        [field: SerializeField] public GameObject PlayerFocusCam { get; private set; }
        
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
            
            MaterialChanger = GetComponentInChildren<SkeletalMaterialChanger>();
            GhostEffect = GetComponentInChildren<SkeletonGhost>();
            
            _attackSpeedColorGradient = globalData.AttackSpeedColorCurve;
            InitInputs();
            
            _playerManager = SystemManager.Instance.PlayerManager;
            
            EffectSetup();
            
            Inventory = new Inventory(this);
            
            SetUp(States.Move);
            SetGlobalState(new PlayerGlobalState(this));
            
            //Inventory.CopyItemList(_playerManager.PlayerIndexInventory, _playerManager.PlayerActiveItemIndex);
            
            _playerManager.PlayerNextFloor.AddListener(NextFloorChangeCamera);
        }

        protected override void Update()
        {
            if (Time.timeScale == 0 )
            {
                return;
            }
            
            base.Update();
            
            UpdateInputs();
            UpdateCoolTime();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            
            _playerManager.PlayerNextFloor.RemoveListener(NextFloorChangeCamera);
            Inventory.ClearInventory();
        }

        public void Hit(Vector2 dir, float power,AttackType attackType)
        {
            if(StatComponent.GetStatus(PlayerStats.MercyInvincibleTime).IsFull() && StatComponent.GetStatus(PlayerStats.DodgeInvincibleTime).IsFull())
            {
                StatComponent.GetStatus(PlayerStats.HP).AddStatus(-power);
                SystemManager.Instance.EventManager.InvokeEvent(TriggerTypes.OnDamage, (dir, power));
            }
        }
        
        public void Heal(float amount)
        {
            SystemManager.Instance.EventManager.InvokeEvent(TriggerTypes.OnHeal, amount);
            SystemManager.Instance.SoundManager.PlayOneShot(SystemManager.Instance.SoundManager.SoundData.Player_Heal);
            HealEffectPlay();
        }
        
        public void PlayerDead()
        {
            ChangeState(Player.States.Dead);
        }

        public void Pause(bool isPause)
        {
            PauseGame(isPause);
            
            PlayerFocusCam.SetActive(isPause);
            Animator.SetBool(IsPause, isPause);
        }

        public void Warp(Vector2Int cellPos)
        {
            SystemManager.Instance.UIManager.SetState(UIState.InGame);
            
            PauseGame(true);
            Animator.SetTrigger(IsWarp);
            
            StartCoroutine(Util.UnityUtil.WaitForFunc(() =>
            {
                _warpEffectParticle.Play();
                Animator.ResetTrigger(IsWarp);
                SystemManager.Instance.SoundManager.PlayOneShot(SystemManager.Instance.SoundManager.SoundData
                    .WarpMapSFX);
                StartCoroutine(Util.UnityUtil.WaitForFunc(() =>
                {
                    Animator.SetTrigger(IsWarp);
                    SystemManager.Instance.PlayerManager.PlayerMapTeleportPosition.Invoke(cellPos);
                    StartCoroutine(Util.UnityUtil.WaitForFunc(() => { PauseGame(false); }, 0.5f));
                }, 0.37f));
            }, 0.3f));
            Animator.ResetTrigger(Swing);
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

        private void NextFloorChangeCamera()
        {
            Pause(false);
            _camera = Camera.main;
        }
        
        public Transform SetProjectileTarget()
        {
            var origin = AimPosition;
            var allHitAble = HitAbleManager.Instance.GetAllHitAble();
            var minDist = float.MaxValue;
            IHitAble minHitable = null;
            
            foreach (var hitable in allHitAble)
            {
                if (hitable.IsClearTarget && !hitable.IsDead)
                {
                    var dist = (hitable.Position - origin).sqrMagnitude;
                    if (dist < minDist)
                    {
                        minDist = dist;
                        minHitable = hitable;
                    }
                }
            }

            Transform target = null;
            if (minHitable != null)
            {
                target = ((MonoBehaviour) minHitable).transform;
            }

            ProjectileShooter.SetTarget(target);

            return target;
        }
    }
}
