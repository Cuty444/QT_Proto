using System.Collections;
using System.Timers;
using QT.Core;
using UnityEngine;

namespace QT.InGame
{
    [FSMState((int) Dullahan.States.Rush)]
    public class DullahanRushState : FSMState<Dullahan>
    {
        public readonly LayerMask BounceMask = LayerMask.GetMask("Wall","HardCollider","ProjectileCollider", "Player","InteractionCollider");
        
        private readonly int IsRushingAnimHash = Animator.StringToHash("IsRushing");
        private readonly int RushReadyAnimHash = Animator.StringToHash("RushReady");
        
        
        private bool _isReady;
        private Vector2 _dir;

        private float _time;
        
        private Transform _transform;
        private Transform _rushCenter;
        
        private float _speed;
        private float _size;
        private float _damage;

        public DullahanRushState(IFSMEntity owner) : base(owner)
        {
            _transform = _ownerEntity.transform;
            _rushCenter = _ownerEntity.RushCenter;
            
            _speed = _ownerEntity.DullahanData.RushSpeed;
            _size = _ownerEntity.RushColliderSize;
            _damage = _ownerEntity.DullahanData.RushHitDamage;
        }

        public override void InitializeState()
        {
            _dir = (SystemManager.Instance.PlayerManager.Player.transform.position - _ownerEntity.transform.position);
            _ownerEntity.SetDir(_dir,2);

            _isReady = false;
            _time = 0;
            
            _transform = _ownerEntity.transform;
            _ownerEntity.SetPhysics(false);

            if (Mathf.Abs(_dir.x) > Mathf.Abs(_dir.y))
            {
                _dir = new Vector2(_dir.x > 0 ? 1 : -1, 0);
            }
            else
            {
                _dir = new Vector2(0, _dir.y > 0 ? 1 : -1);
            }

            _ownerEntity.Animator.SetTrigger(RushReadyAnimHash);
            _ownerEntity.RushTrailObject.SetActive(true);
        }

        public override void UpdateState()
        {
            _time += Time.deltaTime;
            if (!_isReady)
            {
                if (_time > _ownerEntity.DullahanData.RushReadyTime)
                {
                    _isReady = true;
                    _ownerEntity.RushTrailObject.SetActive(true);
                    _time = 0;
                }
                _ownerEntity.Animator.SetBool(IsRushingAnimHash, true);
            }
            
            _transform.Translate(_dir * (_speed * Time.deltaTime));
        }

        public override void FixedUpdateState()
        {
            if (CheckHit() || _time > _ownerEntity.DullahanData.RushLengthTime)
            {
                _ownerEntity.RushTrailObject.SetActive(false);
                _ownerEntity.ChangeState(Dullahan.States.Normal);
            }
        }

        private bool CheckHit()
        {
            var hits = Physics2D.CircleCastAll(_rushCenter.position, _size, _dir, _speed * Time.deltaTime, BounceMask);

            Debug.DrawRay(_rushCenter.position, _dir * (_size +_speed * Time.deltaTime), Color.magenta, 1);
            Debug.DrawRay(_rushCenter.position, new Vector3(-_dir.y, _dir.x) * (_size), Color.magenta, 1);
            Debug.DrawRay(_rushCenter.position, new Vector3(_dir.y, -_dir.x) * (_size), Color.magenta, 1);
            
            foreach (var hit in hits)  
            {
                if (hit.collider != null)
                {
                    if (hit.collider.TryGetComponent(out IHitable hitable))
                    {
                        hitable.Hit(_dir, _damage);
                    }
                }
            }
            
            return hits.Length > 0;
        }

        public override void ClearState()
        {
            _ownerEntity.Animator.SetBool(IsRushingAnimHash, false);
            _ownerEntity.SetPhysics(true);
            
            _ownerEntity.Animator.ResetTrigger(RushReadyAnimHash);
        }
        
    }
}