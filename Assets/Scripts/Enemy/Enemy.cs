using QT.Core;
using UnityEngine;
using UnityEngine.UI;

namespace QT.InGame
{
    public partial class Enemy : FSMPlayer<Enemy>, IFSMEntity, IProjectile
    {
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
        
        public int ProjectileId => gameObject.GetInstanceID();
        public  Vector2 Position => transform.position;
        public float ColliderRad { get; private set; }

        [SerializeField] private int _enemyId;
        [field: SerializeField] public Canvas HpCanvas { get; private set; }
        public EnemyGameData Data { get; private set; }
        public Rigidbody2D Rigidbody { get; private set; }
        
        public EnemyProjectileShooter Shooter { get; private set; }
        public Animator Animator { get; private set; }
        public EnemySkeletalMaterialChanger MaterialChanger { get; private set; }

        public CapsuleCollider2D Collider2D { get; private set; }

        [field: SerializeField] public Transform BallObject { get; private set; }
        [field: SerializeField] public float BallHeight { get; private set; }
        [field: SerializeField] public float BallHeightMin { get; private set; }
        
        [HideInInspector] public Image HpImage;

        [HideInInspector] public AttackType HitAttackType;

        private void Start()
        {
            Data = SystemManager.Instance.DataManager.GetDataBase<EnemyGameDataBase>().GetData(_enemyId);
            Rigidbody = GetComponent<Rigidbody2D>();
            Collider2D = GetComponent<CapsuleCollider2D>();
            Shooter = GetComponent<EnemyProjectileShooter>();
            Animator = GetComponentInChildren<Animator>();
            MaterialChanger = GetComponentInChildren<EnemySkeletalMaterialChanger>();

            ColliderRad = SystemManager.Instance.DataManager.GetDataBase<ProjectileGameDataBase>()
                .GetData(Data.ProjectileDataId).ColliderRad * 0.5f;
            
            Shooter.Initialize(this);
            
            SetUpStats();
            SetUp(States.Normal);
            SetGlobalState(new EnemyGlobalState(this));
            HpCanvas.worldCamera = Camera.main;
            HpImage = HpCanvas.transform.GetChild(0).GetChild(0).GetComponent<Image>();
            HpCanvas.gameObject.SetActive(false);
        }
        
        public void SetPhysics(bool enable)
        {
            Rigidbody.simulated = enable;

            var colliders = new Collider2D[Rigidbody.attachedColliderCount];
            Rigidbody.GetAttachedColliders(colliders);
            
            foreach (var collider in colliders)
            {
                collider.enabled = enable;
            }

            Collider2D.enabled = enable; // TODO : 비활성화 되면 배열을 못가져오는 것 같아서 임시 처리 
        }

        public int RandomGoldDrop()
        {
            return UnityEngine.Random.Range(Data.GoldDropMin, Data.GoldDropMax + 1);
        }
    }    
}
