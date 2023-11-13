using System;
using System.Collections;
using System.Collections.Generic;
using QT.Core;
using QT.Core.Data;
using UnityEngine;
using UnityEngine.UI;

namespace QT.InGame
{
    public partial class Enemy : FSMPlayer<Enemy>, IFSMEntity, IEnemy, IProjectile
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
        public bool IsRigid => CurrentStateIndex == (int)States.Rigid;
        public LayerMask BounceMask { get; set; }
        
        [field: SerializeField] public EnemyHPIndicator HpIndicator { get; private set; }
        
        public EnemyGameData Data { get; private set; }
        public Rigidbody2D Rigidbody { get; private set; }
        
        public EnemyProjectileShooter Shooter { get; private set; }
        public Animator Animator { get; private set; }
        public SkeletalMaterialChanger MaterialChanger { get; private set; }
        public Steering Steering { get; private set; }
        
        [field: SerializeField] public Transform BallObject { get; private set; }
        [field: SerializeField] public float BallHeight { get; private set; }
        [field: SerializeField] public float BallHeightMin { get; private set; }
        [field: SerializeField] public SpriteRenderer ShadowSprite { get; private set; }

        [field: SerializeField] public ProjectileOwner Owner { get; private set; }

        private Collider2D[] _colliders;

        public int ProjectileDamage { get; private set; }
        
        
        private void Awake()
        {
            Rigidbody = GetComponent<Rigidbody2D>();
            
            _colliders = new Collider2D[Rigidbody.attachedColliderCount];
            Rigidbody.GetAttachedColliders(_colliders);
            
            Shooter = GetComponent<EnemyProjectileShooter>();
            Animator = GetComponentInChildren<Animator>();
            MaterialChanger = GetComponentInChildren<SkeletalMaterialChanger>();
            Steering = GetComponent<Steering>();

            LoadSound();
            
        }
        
        public void Initialization(int enemyId)
        {
            var dataManager = SystemManager.Instance.DataManager;
            
            Data = dataManager.GetDataBase<EnemyGameDataBase>().GetData(enemyId);
            ColliderRad = dataManager.GetDataBase<ProjectileGameDataBase>().GetData(Data.ProjectileDataId).ColliderRad;
            
            Shooter.Initialize(Animator);
            BounceMask = Shooter.BounceMask;
            
            SetUpStats();
            SetUp(States.Normal);
            SetGlobalState(new EnemyGlobalState(this));
            
            HitAbleManager.Instance.Register(this);
            ProjectileManager.Instance.Register(this);
            
            BallObject.localPosition = Vector2.up * BallHeightMin;
            ShadowSprite.color = new Color(0, 0, 0, 0.5f);
            
            HpIndicator.gameObject.SetActive(false);
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
        
        
        public void ReleaseObject()
        {
            if (string.IsNullOrWhiteSpace(PrefabPath))
            {
                return;
            }
            
            OnDestroy();
            
            HitAbleManager.Instance.UnRegister(this);
            ProjectileManager.Instance.UnRegister(this);
            
            SystemManager.Instance.ResourceManager.ReleaseObject(PrefabPath, transform);
        }
    }    
}
