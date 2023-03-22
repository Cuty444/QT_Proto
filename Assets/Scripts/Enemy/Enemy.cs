using QT.Core;
using UnityEngine;

namespace QT.Enemy
{
    public class Enemy : FSMPlayer<Enemy>, IFSMEntity
    {
        public enum States : int
        {
            Global,
            Normal,
            Rigid,
            Projectile,
            Dead,
        }

        [SerializeField] private int _enemyId;

        public EnemyGameData Data { get; private set; }
        public Rigidbody2D Rigidbody { get; private set; }

        private void Awake()
        {
            Rigidbody = GetComponent<Rigidbody2D>();
            Data = SystemManager.Instance.DataManager.GetDataBase<EnemyGameDataBase>().GetData(_enemyId);
            
            SetUp(States.Normal);
        }
    }    
}
