using System;
using System.Collections.Generic;
using QT.Core;
using QT.UI;
using UnityEngine;
using UnityEngine.Serialization;

namespace QT.Player
{
    public partial class Player : FSMPlayer<Player>, IFSMEntity, IHitable
    {
        public enum States : int
        {
            Global,
            Idle,
            Move,
            Swing,
            Dodge,
            Rigid,
            Dead,
        }

        [SerializeField] private int _characterID = 100;
        [SerializeField] private int _characterAtkID = 200;
        
        [field:SerializeField] public Transform EyeTransform { get; private set; }
        [SerializeField] private Transform _batTransform;
        [SerializeField] private Transform _lineRendersTransform;
        [SerializeField] private SpriteRenderer _batSpriteRenderer;
        [SerializeField] private TrailRenderer _trailRenderer;
        
        public Rigidbody2D Rigidbody { get; private set; }
        public CharacterGameData Data { get; private set; }
        public CharacterAtkGameData AtkData { get; private set; }
        
        public PlayerProjectileShooter ProjectileShooter { get; private set; }

        public Vector2 MoveDirection { get; private set; }

        private PlayerManager _playerManager;

        private bool _isEnterDoor;
        private void Awake()
        {
            Data = SystemManager.Instance.DataManager.GetDataBase<CharacterGameDataBase>().GetData(_characterID);
            AtkData = SystemManager.Instance.DataManager.GetDataBase<CharacterAtkGameDataBase>().GetData(_characterAtkID);
            Rigidbody = GetComponent<Rigidbody2D>();
            SwingAreaMeshFilter = GetComponentInChildren<MeshFilter>();
            SwingAreaMeshRenderer = GetComponentInChildren<MeshRenderer>();
            SwingAreaMeshRenderer.material.color = new Color(0.345098f, 1f, 0.8823529f, 0.2f);
            ProjectileShooter = GetComponent<PlayerProjectileShooter>();
            _animator = GetComponentInChildren<Animator>();
            SetUpStats();
            SetUp(States.Idle);
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

        public void SetMoveDirection(Vector2 direction)
        {
            MoveDirection = direction;
        }
    }
}
