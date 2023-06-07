using System.Collections;
using System.Collections.Generic;
using QT.Core;
using UnityEngine;


namespace QT.InGame
{
    public partial class Dullahan : FSMPlayer<Dullahan>, IFSMEntity, IHitable
    {
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
        public Rigidbody2D Rigidbody { get; private set; }
        
        
        public EnemyProjectileShooter Shooter { get; private set; }
        public Animator Animator { get; private set; }
        public EnemySkeletalMaterialChanger MaterialChanger { get; private set; }
        public Steering Steering { get; private set; }

        private Collider2D[] _colliders;


        private void Start()
        {
            Data = SystemManager.Instance.DataManager.GetDataBase<EnemyGameDataBase>().GetData(_enemyId);
            Rigidbody = GetComponent<Rigidbody2D>();
            Shooter = GetComponent<EnemyProjectileShooter>();
            Animator = GetComponentInChildren<Animator>();
            MaterialChanger = GetComponentInChildren<EnemySkeletalMaterialChanger>();
            Steering = GetComponent<Steering>();
            
            _colliders = new Collider2D[Rigidbody.attachedColliderCount];
            Rigidbody.GetAttachedColliders(_colliders);

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

        public void SetFlip(bool enable)
        {
            Animator.transform.rotation = Quaternion.Euler(0f, enable ? 180 : 0, 0f);
        }
    }
}
