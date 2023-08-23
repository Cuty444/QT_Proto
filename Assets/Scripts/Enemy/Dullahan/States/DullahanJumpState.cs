using System.Timers;
using QT.Core;
using QT.Sound;
using UnityEngine;

namespace QT.InGame
{
    [FSMState((int) Dullahan.States.Jump)]
    public class DullahanJumpState : FSMState<Dullahan>
    {
        private readonly int IsJumpingAnimHash = Animator.StringToHash("IsJumping");
        private readonly int JumpReadyAnimHash = Animator.StringToHash("JumpReady");
        
        private const string ChargingEffectPath = "Effect/Prefabs/FX_Boss_Jmup_Charging.prefab";
        private const string SmashEffectPath = "Effect/Prefabs/FX_Boss_Jump.prefab";
        
        private int _state;

        private float _time;
        
        private Transform _transform;
        private Transform _playerTransform;
        
        private float _speed;
        private float _hitRange;
        private float _damage;
        
        private Color _shadowColor;
        private Vector2 _shadowScale;

        private SoundManager _soundManager;

        public DullahanJumpState(IFSMEntity owner) : base(owner)
        {
            _transform = _ownerEntity.transform;
            
            _speed = _ownerEntity.DullahanData.JumpMoveSpeed;
            _hitRange = _ownerEntity.DullahanData.JumpMoveSpeed;
            _damage = _ownerEntity.DullahanData.LandingHitRange;
        }

        public override void InitializeState()
        {
            _state = 0;
            _time = 0;
            _soundManager = SystemManager.Instance.SoundManager;

            _ownerEntity.Rigidbody.velocity = Vector2.zero;
            _playerTransform = SystemManager.Instance.PlayerManager.Player.transform;
            
            _ownerEntity.Animator.SetTrigger(JumpReadyAnimHash);
            _ownerEntity.Animator.SetBool(IsJumpingAnimHash, false);
            
            SystemManager.Instance.ResourceManager.EmitParticle(ChargingEffectPath, _transform.position);
            _soundManager.PlayOneShot(_soundManager.SoundData.Boss_JumpReady, _ownerEntity.transform.position);
            _ownerEntity.JumpReadyImpulseSource.GenerateImpulse(1);
            
            _shadowColor = _ownerEntity.Shadow.color;
            _shadowScale = _ownerEntity.Shadow.transform.localScale;
            
            HitAbleManager.Instance.UnRegister(_ownerEntity);
        }

        public override void UpdateState()
        {
            _time += Time.deltaTime;

            switch (_state)
            {
                case 0:
                    Ready();
                    break;
                case 1:
                    Jumping();
                    break;
                case 2:
                    OnAir();
                    break;
                case 3:
                    Ending();
                    break;
            }
        }


        private void Ready()
        {
            if (_time > _ownerEntity.DullahanData.JumpReadyTime)
            {
                _state++;
                _ownerEntity.Animator.SetBool(IsJumpingAnimHash, true);
                _soundManager.PlayOneShot(_soundManager.SoundData.Boss_Jump, _ownerEntity.transform.position);
                _ownerEntity.JumpImpulseSource.GenerateImpulse(1);
                
                _ownerEntity.SetPhysics(false);
                _time = 0;
            }
        }

        private void Jumping()
        {
            var height = (_time / _ownerEntity.DullahanData.JumpingTime);
            
            // easeInCubic
            height *= height * height;
            
            SetHeight(1 - height);
            
            if (_time > _ownerEntity.DullahanData.JumpingTime)
            {
                _state++;
                _time = 0;
                _ownerEntity.DullahanObject.gameObject.SetActive(false);
            }
        }

        private void OnAir()
        {
            var time = (_time / _ownerEntity.DullahanData.JumpLengthTime);
            
            // easeOutQuad
            var height = 1 - (1 - time) * (1 - time);
            
            SetHeight(height);
            
            var dir = (_playerTransform.position - _transform.position).normalized;
            _transform.Translate(_speed * (1 - time) * Time.deltaTime * dir);
            
            if (_time > _ownerEntity.DullahanData.JumpLengthTime)
            {
                _state++;
                _time = 0;
                
                _ownerEntity.DullahanObject.gameObject.SetActive(true);
                _ownerEntity.Animator.SetBool(IsJumpingAnimHash, false);
                
                SystemManager.Instance.ResourceManager.EmitParticle(SmashEffectPath, _transform.position);
                _ownerEntity.LandingImpulseSource.GenerateImpulse(2);
                
                _ownerEntity.Shadow.color = _shadowColor;
                _ownerEntity.Shadow.transform.localScale = _shadowScale;

                _ownerEntity.Shooter.ShootPoint = _transform;
                _ownerEntity.Shooter.PlayEnemyAtkSequence(_ownerEntity.DullahanData.LandingAtkId, ProjectileOwner.Boss);
            }
        }
        
        private void Ending()
        {
            if (_time > _ownerEntity.DullahanData.JumpEndTime)
            {
                _state++;
                _ownerEntity.SetPhysics(true);

                _ownerEntity.ChangeState(Dullahan.States.Normal);
                _soundManager.PlayOneShot(_soundManager.SoundData.Boss_Landing, _ownerEntity.transform.position);
            }
        }

        private void SetHeight(float height)
        {
            var shadowColor = _shadowColor;
            shadowColor.a *= height;
            _ownerEntity.Shadow.color = shadowColor;
            
            _ownerEntity.Shadow.transform.localScale = _shadowScale * height;
        }
        

        public override void ClearState()
        {
            _ownerEntity.Animator.ResetTrigger(JumpReadyAnimHash);
            _ownerEntity.Animator.SetBool(IsJumpingAnimHash, false);
            _ownerEntity.SetPhysics(true);
            
            HitAbleManager.Instance.Register(_ownerEntity);
        }
    }
}