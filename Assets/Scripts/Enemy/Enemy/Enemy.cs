using System;
using System.Collections;
using System.Collections.Generic;
using QT.Core;
using QT.Core.Data;
using UnityEngine;
using UnityEngine.UI;

namespace QT.InGame
{
    public partial class Enemy : FSMPlayer<Enemy>, IFSMEntity, IHitAble, IProjectile
    {
        public string PrefabPath { get; set; }

        public enum States : int
        {
            Global,
            
            // 살아있음
            Normal,
            Rigid, 
            
            // 사실상 죽어있음
            Projectile,
            Dead,
        }
        
        public int InstanceId => gameObject.GetInstanceID();
        public Vector2 Position => transform.position;
        public float ColliderRad { get; private set; }
        public bool IsClearTarget => true;
        public bool IsDead => HP <= 0;
        public LayerMask BounceMask { get; set; }


        [SerializeField] private int _enemyId;
        [field: SerializeField] public Canvas HpCanvas { get; private set; }
        
        public EnemyGameData Data { get; private set; }
        public Rigidbody2D Rigidbody { get; private set; }
        
        public EnemyProjectileShooter Shooter { get; private set; }
        public Animator Animator { get; private set; }
        public EnemySkeletalMaterialChanger MaterialChanger { get; private set; }
        public Steering Steering { get; private set; }
        
        [field: SerializeField] public Transform BallObject { get; private set; }
        [field: SerializeField] public float BallHeight { get; private set; }
        [field: SerializeField] public float BallHeightMin { get; private set; }
        [field: SerializeField] public SpriteRenderer ShadowSprite { get; private set; }

        [field: SerializeField] public ProjectileOwner Owner { get; private set; }

        [HideInInspector] public Image HpImage;

        //[HideInInspector] public LineRenderer TeleportLineRenderer;

        private Collider2D[] _colliders;

        public int _damage { get; private set; }
        
        
        private void Awake()
        {
            Rigidbody = GetComponent<Rigidbody2D>();
            
            _colliders = new Collider2D[Rigidbody.attachedColliderCount];
            Rigidbody.GetAttachedColliders(_colliders);
            
            Shooter = GetComponent<EnemyProjectileShooter>();
            Animator = GetComponentInChildren<Animator>();
            MaterialChanger = GetComponentInChildren<EnemySkeletalMaterialChanger>();
            Steering = GetComponent<Steering>();
            
            HpCanvas.worldCamera = Camera.main;
            HpImage = HpCanvas.transform.GetChild(0).GetChild(0).GetComponent<Image>();
            HpCanvas.gameObject.SetActive(false);


            // SystemManager.Instance.PlayerManager.PlayerMapClearPosition.AddListener((arg) =>
            //     SetTeleportLine(Vector2.zero, false));
            
            LoadSound();
            
        }

        // 구 버전 맵 데이터를 쓰지 않을 때 제거
        private void Start()
        {
            initialization(_enemyId);
        }

        public void initialization(int enemyId)
        {
            _enemyId = enemyId;
            
            Data = SystemManager.Instance.DataManager.GetDataBase<EnemyGameDataBase>().GetData(_enemyId);
            
            ColliderRad = SystemManager.Instance.DataManager.GetDataBase<ProjectileGameDataBase>()
                .GetData(Data.ProjectileDataId).ColliderRad * 0.5f;
            
            Shooter.Initialize(Animator);
            BounceMask = Shooter.BounceMask;
            
            SetUpStats();
            SetUp(States.Normal);
            SetGlobalState(new EnemyGlobalState(this));
            
            HitAbleManager.Instance.Register(this);
            ProjectileManager.Instance.Register(this);

            BallObject.localPosition = Vector2.up * BallHeightMin;
            ShadowSprite.color = new Color(0, 0, 0, 0.5f);
        }


        public void SetPhysics(bool enable)
        {
            Rigidbody.simulated = enable;
            
            foreach (var collider in _colliders)
            {
                collider.enabled = enable;
            }
        }

        public int RandomGoldDrop()
        {
            return UnityEngine.Random.Range(Data.GoldDropMin, Data.GoldDropMax + 1);
        }
       
        
        // private const string TeleportLinePath = "Prefabs/TeleportLine.prefab";
        // public async void SetTeleportLine(Vector2 target, bool isActive)
        // {
        //     if (isActive)
        //     {
        //         if (TeleportLineRenderer == null)
        //         {
        //             TeleportLineRenderer =
        //                 await SystemManager.Instance.ResourceManager.GetFromPool<LineRenderer>(TeleportLinePath);
        //         }
        //         
        //         if (CurrentStateIndex >= (int)Player.States.Fall)
        //         {
        //             TeleportLineRenderer.positionCount = 0;
        //             return;
        //         }
        //         TeleportLineRenderer.positionCount = 2;
        //         TeleportLineRenderer.SetPosition(0, target);
        //         TeleportLineRenderer.SetPosition(1, transform.position);
        //     }
        //     else if(TeleportLineRenderer != null)
        //     {
        //         TeleportLineRenderer.positionCount = 0;
        //         SystemManager.Instance.ResourceManager.ReleaseObject(TeleportLinePath, TeleportLineRenderer);
        //         TeleportLineRenderer = null;
        //     }
        // }
        
        public void ReleaseObject()
        {
            if (string.IsNullOrWhiteSpace(PrefabPath))
            {
                return;
            }
            
            OnDestroy();
            
            HitAbleManager.Instance.UnRegister(this);
            ProjectileManager.Instance.UnRegister(this);
            
            SystemManager.Instance.ResourceManager.ReleaseObject(PrefabPath, this);
        }
    }    
}
