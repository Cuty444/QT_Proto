using System.Collections;
using QT.Core;
using QT.Core.Data;
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
        public Steering Steering { get; private set; }
        
        [field: SerializeField] public Transform BallObject { get; private set; }
        [field: SerializeField] public float BallHeight { get; private set; }
        [field: SerializeField] public float BallHeightMin { get; private set; }
        [field: SerializeField] public SpriteRenderer ShadowSprite { get; private set; }

        [field: SerializeField] public ProjectileOwner Owner { get; private set; }

        [HideInInspector] public Image HpImage;

        [HideInInspector] public AttackType HitAttackType;

        [HideInInspector] public LineRenderer TeleportLineRenderer;

        [HideInInspector] public bool IsTeleportProjectile = false;

        private AnimationCurve enemyFallScaleCurve;

        private Collider2D[] _colliders;

        public int _damage { get; private set; }
        private void Start()
        {
            Data = SystemManager.Instance.DataManager.GetDataBase<EnemyGameDataBase>().GetData(_enemyId);
            Rigidbody = GetComponent<Rigidbody2D>();
            
            _colliders = new Collider2D[Rigidbody.attachedColliderCount];
            Rigidbody.GetAttachedColliders(_colliders);
            
            Shooter = GetComponent<EnemyProjectileShooter>();
            Animator = GetComponentInChildren<Animator>();
            MaterialChanger = GetComponentInChildren<EnemySkeletalMaterialChanger>();
            Steering = GetComponent<Steering>();

            ColliderRad = SystemManager.Instance.DataManager.GetDataBase<ProjectileGameDataBase>()
                .GetData(Data.ProjectileDataId).ColliderRad * 0.5f;
            
            Shooter.Initialize(Animator);
            
            SetUpStats();
            SetUp(States.Normal);
            SetGlobalState(new EnemyGlobalState(this));
            HpCanvas.worldCamera = Camera.main;
            HpImage = HpCanvas.transform.GetChild(0).GetChild(0).GetComponent<Image>();
            HpCanvas.gameObject.SetActive(false);

            enemyFallScaleCurve = SystemManager.Instance.GetSystem<GlobalDataSystem>().GlobalData.EnemyFallScaleCurve;
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

        public void FallScale()
        {
            StartCoroutine(ScaleReduce());
        }

        private IEnumerator ScaleReduce()
        {
            float time = 0f;
            while (time < 1f)
            {
                float scale = Mathf.Lerp(0, 1, enemyFallScaleCurve.Evaluate(time / 1f));
                transform.localScale = new Vector3(scale, scale, scale);
                yield return null;
                time += Time.deltaTime;
            }
        }
    }    
}
