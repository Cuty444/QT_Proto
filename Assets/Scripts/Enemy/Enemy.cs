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
        
        [field:SerializeField]public int MonsterId { get; private set; }
        public Rigidbody2D Rigidbody { get; private set; }

        private void Awake()
        {
            Rigidbody = GetComponent<Rigidbody2D>();
            
            SetUp(States.Normal);
        }
    }    
}
