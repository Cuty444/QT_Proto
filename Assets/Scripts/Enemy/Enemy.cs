using QT.Core;
using UnityEngine;

namespace QT.Enemy
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

        [SerializeField] private int _enemyId;

        public EnemyGameData Data { get; private set; }
        public Rigidbody2D Rigidbody { get; private set; }
        
        public EnemyProjectileShooter Shooter { get; private set; }
        public Animator Animator { get; private set; }

        private void Awake()
        {
            Data = SystemManager.Instance.DataManager.GetDataBase<EnemyGameDataBase>().GetData(_enemyId);
            Rigidbody = GetComponent<Rigidbody2D>();
            Shooter = GetComponent<EnemyProjectileShooter>();
            Animator = GetComponentInChildren<Animator>();

            Shooter.Initialize(this);
            
            SetUpStats();
            SetUp(States.Normal);
        }
    }    
}
