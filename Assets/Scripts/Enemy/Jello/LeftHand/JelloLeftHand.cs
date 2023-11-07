using System;
using System.Collections;
using System.Collections.Generic;
using QT.Core;
using QT.Core.Data;
using UnityEngine;
using UnityEngine.UI;

namespace QT.InGame
{
    public partial class JelloLeftHand : FSMPlayer<JelloLeftHand>, IFSMEntity, IEnemy, IProjectile
    {
        public string PrefabPath { get; set; }
        private static readonly int RotationAnimHash = Animator.StringToHash("Rotation");
        
        public enum States : int
        {
            Global,
            
            Spawn,
            
            // 살아있음
            Normal,
            Rigid, 
            
            Shoot,
            
            Return,
            
            // 사실상 죽어있음
            Projectile,
            Dead,
        }

        public int InstanceId => gameObject.GetInstanceID();
        public Vector2 Position => transform.position;
        public bool IsClearTarget => true;
        public bool IsDead => HP <= 0;
        public bool IsRigid => CurrentStateIndex == (int)States.Rigid;
        
        public LayerMask BounceMask { get; set; }
        
        [field: SerializeField] public JelloHandData JelloData{ get; private set; }
        [field: SerializeField] public float ColliderRad { get; private set; }
        [field: SerializeField] public EnemyHPIndicator HpIndicator { get; private set; }
        
        [field:Space]
        [field: SerializeField] public Transform ShootPointPivot{ get; private set; }
        [field: SerializeField] public Transform ShootPointTransform{ get; private set; }
        
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
        }
        
        public void initialization(int enemyId)
        {
            Data = SystemManager.Instance.DataManager.GetDataBase<EnemyGameDataBase>().GetData(enemyId);
            
            Shooter.Initialize(Animator);
            BounceMask = Shooter.BounceMask;
            
            SetUpStats();
            SetUp(States.Spawn);
            SetGlobalState(new JelloLeftHandGlobalState(this));
            
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


        public int SetDir(Vector2 dir, int sideCount)
        {
            var side = GetSide(dir, sideCount);

            SetDir(side);
            
            var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            ShootPointPivot.rotation = Quaternion.Euler(0, 0, angle);
            
            return side;
        }

        private void SetDir(int side)
        {
            Animator.SetFloat(RotationAnimHash, side);
        }

        public int GetSide(Vector2 dir, int sideCount)
        {
            var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

            if (angle < 0)
            {
                angle += 360;
            }
            
            return Mathf.RoundToInt(angle / 360 * sideCount);
        }
    }    
}
