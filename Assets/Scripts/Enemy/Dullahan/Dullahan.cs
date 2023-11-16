using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using QT.Core;
using UnityEngine;


namespace QT.InGame
{
    public partial class Dullahan : FSMPlayer<Dullahan>, IFSMEntity, IEnemy
    {
        public string PrefabPath { get; set; }
        
        public LayerMask HitMask => LayerMask.GetMask("Wall","HardCollider","ProjectileCollider", "Player", "Enemy", "InteractionCollider");

        private readonly int RotationAnimHash = Animator.StringToHash("Rotation");
        
        public enum States : int
        {
            Global,
            
            // 살아있음
            Normal,
            Stun, 
            
            Rush,
            Smash,
            Throw,
            
            Attack,
            Jump,
            
            Summon,
            
            Dead,
        }
        
        public int InstanceId => gameObject.GetInstanceID();
        public Vector2 Position => transform.position;
        public float ColliderRad => RushColliderSize;
        public bool IsClearTarget => true;
        public bool IsDead => CurrentStateIndex == (int) States.Dead;
        public bool IsRigid => false;
        
        
        [SerializeField] private int _enemyId;
        public EnemyGameData Data { get; private set; }
        public BossMapData MapData { get; private set; }
        
        [field: SerializeField] public DullahanData DullahanData{ get; private set; }
        
        
        [field: SerializeField] public Transform[] ShootPoints{ get; private set; }
        [field: SerializeField] public Transform CenterTransform{ get; private set; }
        [field: SerializeField] public Transform HandTransform{ get; private set; }
        [field: SerializeField] public Transform WheelTransform{ get; private set; }
        [field: SerializeField] public float RushColliderSize{ get; private set; }
        [field: SerializeField] public GameObject RushTrailObject{ get; private set; }
        
        [field:SerializeField] public CinemachineImpulseSource RushShockImpulseSource { get; private set; }
        [field:SerializeField] public CinemachineImpulseSource AttackImpulseSource { get; private set; }
        
        [field:SerializeField] public CinemachineImpulseSource JumpReadyImpulseSource { get; private set; }
        [field:SerializeField] public CinemachineImpulseSource JumpImpulseSource { get; private set; }
        [field:SerializeField] public CinemachineImpulseSource LandingImpulseSource { get; private set; }
        
        // 사망 연출에 시네 머신 같은 것을 사용 하자
        [field:SerializeField] public CinemachineImpulseSource DeadImpulseSource { get; private set; }
        [field:SerializeField] public CinemachineImpulseSource ExplosionImpulseSource { get; private set; }
        
        [field: SerializeField] public Transform DullahanObject{ get; private set; }
        [field: SerializeField] public SpriteRenderer Shadow{ get; private set; }

        public Rigidbody2D Rigidbody { get; private set; }
        public Animator Animator { get; private set; }
        public EnemyProjectileShooter Shooter { get; private set; }
        public SkeletalMaterialChanger[] MaterialChanger { get; private set; }
        public Steering Steering { get; private set; }

        private Collider2D[] _colliders;
        
        private void Awake()
        {
            Rigidbody = GetComponent<Rigidbody2D>();
            Shooter = GetComponent<EnemyProjectileShooter>();
            Animator = GetComponentInChildren<Animator>();
            MaterialChanger = GetComponentsInChildren<SkeletalMaterialChanger>();
            Steering = GetComponent<Steering>();
            
            _colliders = new Collider2D[Rigidbody.attachedColliderCount];
            Rigidbody.GetAttachedColliders(_colliders);
        }
        
        public void Initialization(int enemyId)
        {
            _enemyId = enemyId;
            Data = SystemManager.Instance.DataManager.GetDataBase<EnemyGameDataBase>().GetData(_enemyId);
            
#if UNITY_EDITOR
            if (DungeonManager.Instance is DungeonManagerDummy)
            {
                MapData = FindObjectOfType<BossMapData>(true);
            }
#endif
            
            var mapCellData = DungeonManager.Instance.GetCurrentMapCellData();
            if(mapCellData != null && mapCellData.SpecialMapData != null)
            {
                MapData = mapCellData.SpecialMapData as BossMapData;
            }
            
            Shooter.Initialize(null);
            
            SetUpStats();
            
            SetGlobalState(new DullahanGlobalState(this));
            SetUp(States.Normal);
            
            HitAbleManager.Instance.Register(this);
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

            SetDir(side, dir.x > 0);
            
            return side;
        }

        public void SetDir(int side, bool isFlip)
        {
            Animator.SetFloat(RotationAnimHash, side);
            Animator.transform.rotation = Quaternion.Euler(0f, isFlip ? 180 : 0, 0f);
        }

        public int GetSide(Vector2 dir, int sideCount)
        {
            var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg + 90;

            if (angle < 0)
            {
                angle += 360;
            }

            if (angle > 180)
            {
                angle = 360 - angle;
            }

            return Mathf.RoundToInt(angle / 180 * sideCount);
        }
    }
}
