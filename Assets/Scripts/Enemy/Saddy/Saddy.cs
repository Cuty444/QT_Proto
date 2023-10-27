using Cinemachine;
using QT.Core;
using UnityEngine;

namespace QT.InGame
{
    public partial class Saddy : FSMPlayer<Saddy>, IFSMEntity, IEnemy
    {
         public string PrefabPath { get; set; }
        
        public LayerMask HitMask => LayerMask.GetMask("Wall","HardCollider","ProjectileCollider", "Player", "Enemy", "InteractionCollider");

        private static readonly int RotationAnimHash = Animator.StringToHash("Rotation");
        
        public enum States : int
        {
            Global,
            
            // 살아있음
            Normal,
            
            Swing,
            Throw,
            
            Jump,
            
            Summon,
            
            Dead,
        }
        
        public int InstanceId => gameObject.GetInstanceID();
        public Vector2 Position => transform.position;
        public bool IsClearTarget => true;
        public bool IsDead => CurrentStateIndex == (int) States.Dead;
        public bool IsRigid => false;
        
        public EnemyGameData Data { get; private set; }
        
        
        [field: SerializeField] public SaddyData SaddyData{ get; private set; }
        [field: SerializeField] public float ColliderRad { get; private set; }
        
        [field:Space]
        [field: SerializeField] public Transform[] ShootPoints{ get; private set; }
        
        [field:Space]
        [field: SerializeField] public Transform SaddyObject{ get; private set; }
        [field: SerializeField] public SpriteRenderer Shadow{ get; private set; }

        public Rigidbody2D Rigidbody { get; private set; }
        public Animator Animator { get; private set; }
        public EnemyProjectileShooter Shooter { get; private set; }
        public SkeletalMaterialChanger[] MaterialChanger { get; private set; }
        public Steering Steering { get; private set; }

        
        private int _enemyId;
        
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
        
        public void initialization(int enemyId)
        {
            _enemyId = enemyId;
            Data = SystemManager.Instance.DataManager.GetDataBase<EnemyGameDataBase>().GetData(_enemyId);
            
            Shooter.Initialize(null);
            
            SetUpStats();
            
            SetGlobalState(new SaddyGlobalState(this));
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
