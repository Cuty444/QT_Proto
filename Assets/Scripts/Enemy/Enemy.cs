using QT.Core;
using UnityEngine;

namespace QT.Enemy
{
    public partial class Enemy : FSMPlayer<Enemy>, IFSMEntity
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

        [SerializeField] private int _enemyId;

        public EnemyGameData Data { get; private set; }
        public Rigidbody2D Rigidbody { get; private set; }
        
        public EnemyProjectileShooter Shooter { get; private set; }

        private void Awake()
        {
            Data = SystemManager.Instance.DataManager.GetDataBase<EnemyGameDataBase>().GetData(_enemyId);
            Rigidbody = GetComponent<Rigidbody2D>();
            Shooter = GetComponent<EnemyProjectileShooter>();

            SetUpStats();
            SetUp(States.Normal);
        }
    }    
}
