using System;
using System.Collections.Generic;
using QT.Core;
using UnityEngine;
using UnityEngine.Serialization;

namespace QT.Player
{
    public partial class Player : FSMPlayer<Player>, IFSMEntity
    {
        public enum States : int
        {
            Global,
            Idle,
            Move,
            Swing,
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


        private PlayerManager _playerManager;
        private void Awake()
        {
            Data = SystemManager.Instance.DataManager.GetDataBase<CharacterGameDataBase>().GetData(_characterID);
            AtkData = SystemManager.Instance.DataManager.GetDataBase<CharacterAtkGameDataBase>().GetData(_characterAtkID);
            Rigidbody = GetComponent<Rigidbody2D>();
            SwingAreaMeshFilter = GetComponentInChildren<MeshFilter>();
            SwingAreaMeshRenderer = GetComponentInChildren<MeshRenderer>();
            ProjectileShooter = GetComponent<PlayerProjectileShooter>();
            _animator = GetComponentInChildren<Animator>();
            SetUpStats();
            SetUp(States.Idle);
            SetGlobalState(new PlayerGlobalState(this));

            _playerManager = SystemManager.Instance.PlayerManager;
        }

    }
}
