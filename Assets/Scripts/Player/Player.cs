using System;
using System.Collections.Generic;
using QT.Core;
using UnityEngine;
using Cinemachine;

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
            Dodge,
            Dead,
        }

        [SerializeField] private int _characterID = 100;
        [SerializeField] private int _characterAtkID = 200;
        
        [field:SerializeField] public Transform EyeTransform { get; private set; }
        [SerializeField] private Transform _batTransform;
        [SerializeField] private SpriteRenderer _batSpriteRenderer;
        [SerializeField] private TrailRenderer _trailRenderer;
        
        [field:SerializeField] public CinemachineImpulseSource DamageImpulseSource { get; private set; }
        [field:SerializeField] public CinemachineImpulseSource AttackImpulseSource { get; private set; }
        
        public Animator Animator { get; private set; }
        public Rigidbody2D Rigidbody { get; private set; }
        public CharacterGameData Data { get; private set; }
        public CharacterAtkGameData AtkData { get; private set; }
        
        public PlayerProjectileShooter ProjectileShooter { get; private set; }

        private PlayerManager _playerManager;

        private bool _isEnterDoor;
        
        
        private void Awake()
        {
            Data = SystemManager.Instance.DataManager.GetDataBase<CharacterGameDataBase>().GetData(_characterID);
            AtkData = SystemManager.Instance.DataManager.GetDataBase<CharacterAtkGameDataBase>().GetData(_characterAtkID);
            
            Rigidbody = GetComponent<Rigidbody2D>();
            SwingAreaMeshFilter = GetComponentInChildren<MeshFilter>();
            SwingAreaMeshRenderer = GetComponentInChildren<MeshRenderer>();
            ProjectileShooter = GetComponent<PlayerProjectileShooter>();
            Animator = GetComponentInChildren<Animator>();
            
            InitInputs();
            InitStats();
            EffectSetup();
            
            SetUp(States.Move);
            SetGlobalState(new PlayerGlobalState(this));
            
            _playerManager = SystemManager.Instance.PlayerManager;
            _playerManager.CurrentRoomEnemyRegister.AddListener((enemyList) =>
            {
                _enemyList = enemyList;
            });
            _playerManager.PlayerMapPass.AddListener((isBool) =>
            {
                _isEnterDoor = isBool;
            });
            _isEnterDoor = true;
        }

        private void Update()
        {
            UpdateInputs();
            UpdateCoolTime();
        }
        
        public void Hit(Vector2 dir, float power)
        {
            if (IsInvincible())
            {
                return;
            }
            GetStatus(PlayerStats.MercyInvincibleTime).SetStatus(0);
 
            _playerManager.OnDamageEvent.Invoke(dir, power);
        }
    }
}
