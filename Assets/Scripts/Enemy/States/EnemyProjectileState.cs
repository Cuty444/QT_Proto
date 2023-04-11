using System.Collections;
using System.Collections.Generic;
using QT.Core;
using QT.Core.Data;
using UnityEngine;

namespace QT.Enemy
{
    [FSMState((int)Enemy.States.Projectile)]
    public class EnemyProjectileState : FSMState<Enemy>
    {
        private LayerMask _bounceMask;
        
        private float _speed;
        private float _speedDecay;
        
        private float _size;

        private int _maxBounce;
        private int _bounceCount;
        
        private Vector2 _direction;
        
        private Transform _transform;
        
        public EnemyProjectileState(IFSMEntity owner) : base(owner)
        {
            _transform = _ownerEntity.transform;
        }

        public void InitializeState(Vector2 dir, float power)
        {
            _direction = dir;
            _speed = power;
        }

        public override void UpdateState()
        {
            var moveLength = _speed * Time.deltaTime;
            var hit = Physics2D.CircleCast(_transform.position, _size, _direction, moveLength, _bounceMask);

            if (hit.collider != null)
            {
                _direction += hit.normal * (-2 * Vector2.Dot(_direction, hit.normal));

                if (--_bounceCount < 0)
                {
                    _ownerEntity.ChangeState(Enemy.States.Dead);
                }
            }
            
            _transform.Translate(_direction * moveLength);
            
            _speed -= _speedDecay * Time.deltaTime;
            if (_speed <= 0)
            {
                _ownerEntity.ChangeState(Enemy.States.Dead);
            }
        }
    }
}
