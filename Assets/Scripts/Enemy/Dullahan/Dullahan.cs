using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using QT.Core;
using UnityEngine;


namespace QT.InGame
{
    public partial class Dullahan : FSMPlayer<Dullahan>, IFSMEntity, IHitable
    {
        public LayerMask HitMask => LayerMask.GetMask("Wall","HardCollider","ProjectileCollider", "Player", "Enemy", "InteractionCollider");

        private readonly int RotationAnimHash = Animator.StringToHash("Rotation");
        
        public enum States : int
        {
            Global,
            
            // 살아있음
            Normal,
            Stun, 
            
            Rush,
            Jump,
            Attack,
            Throw,
            
            Dead,
        }
        
        [SerializeField] private int _enemyId;
        public EnemyGameData Data { get; private set; }
        [field: SerializeField] public DullahanData DullahanData{ get; private set; }
        
        
        [field: SerializeField] public Transform[] ShootPoints{ get; private set; }
        [field: SerializeField] public Transform CenterTransform{ get; private set; }
        [field: SerializeField] public float RushColliderSize{ get; private set; }
        [field: SerializeField] public GameObject RushTrailObject{ get; private set; }
        [field:SerializeField] public CinemachineImpulseSource RushShockImpulseSource { get; private set; }
        
        [field: SerializeField] public Transform DullahanObject{ get; private set; }
        [field: SerializeField] public SpriteRenderer Shadow{ get; private set; }

        public Rigidbody2D Rigidbody { get; private set; }
        public Animator Animator { get; private set; }
        public EnemyProjectileShooter Shooter { get; private set; }
        public EnemySkeletalMaterialChanger[] MaterialChanger { get; private set; }
        public Steering Steering { get; private set; }

        private Collider2D[] _colliders;
        
        private void Start()
        {
            Data = SystemManager.Instance.DataManager.GetDataBase<EnemyGameDataBase>().GetData(_enemyId);
            Rigidbody = GetComponent<Rigidbody2D>();
            Shooter = GetComponent<EnemyProjectileShooter>();
            Animator = GetComponentInChildren<Animator>();
            MaterialChanger = GetComponentsInChildren<EnemySkeletalMaterialChanger>();
            Steering = GetComponent<Steering>();
            Shooter.Initialize(null);
            
            _colliders = new Collider2D[Rigidbody.attachedColliderCount];
            Rigidbody.GetAttachedColliders(_colliders);

            SetUpStats();
            
            SetGlobalState(new DullahanGlobalState(this));
            SetUp(States.Normal);
        }
        
        public void SetPhysics(bool enable)
        {
            Rigidbody.simulated = enable;
            
            foreach (var collider in _colliders)
            {
                collider.enabled = enable;
            }
        }

        public int SetDir(Vector2 dir,int sideCount)
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

            int side = (int)Mathf.Round(angle / 180 * sideCount);
            
            Animator.SetFloat(RotationAnimHash, side);
            
            Animator.transform.rotation = Quaternion.Euler(0f, dir.x > 0 ? 180 : 0, 0f);

            return side;
        }
    }
}
