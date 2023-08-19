using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using QT.Core;
using QT.Sound;
using QT.Util;
using UnityEngine;

namespace QT.InGame
{
    //Param1 : 속도
    //Param2 : 조향 각도 (초당)
    //Param3 : 범위
    public class ChargeItemEffect : ItemEffect
    {
        private LayerMask WallLayer => LayerMask.GetMask("Default", "Wall", "HardCollider", "CharacterCollider", "Fall");
      
        private const string ChargeEffectPath = "Effect/Prefabs/FX_Active_Rush.prefab";
        private const string ShockEffectPath = "Effect/Prefabs/FX_Active_Rush_crash.prefab";
        private const string SwingBatHitPath = "Effect/Prefabs/FX_Bat_Hit.prefab";
        
        private readonly int AnimationMouseRotateHash = Animator.StringToHash("MouseRotate");
        private readonly int MoveSpeedAnimHash = Animator.StringToHash("MoveSpeed");
        private readonly int PlayerIdleAnimHash = Animator.StringToHash("PlayerIdle");
        private readonly int PlayerSwingAnimHash = Animator.StringToHash("PlayerSwing");

        private readonly Player _player;
        private readonly SoundManager _soundManager;

        private CancellationTokenSource _cancellationTokenSource;

        private float _speed;
        private float _maxSteerAngle;
        private float _size;
        

        private Vector2 _dir;
        private Vector2 _aimPosition;
        
        private bool _isCharging;
        private float _currentSpeed;

        public ChargeItemEffect(Player player, ItemEffectGameData effectData, SpecialEffectGameData specialEffectData) : base(player, effectData, specialEffectData)
        {
            _player = player;
            _soundManager = SystemManager.Instance.SoundManager;
            
            _speed = specialEffectData.Param1;
            _maxSteerAngle = specialEffectData.Param2;
            _size = specialEffectData.Param3;
        }

        public override void OnEquip()
        {
            _player.OnAim.AddListener(OnAim);
        }

        protected override void OnTriggerAction()
        {
            if (_isCharging)
            {
                return;
            }

            Charging();
        }

        public override void OnRemoved()
        {
            _cancellationTokenSource?.Cancel();
            
            _player.Animator.SetBool(PlayerIdleAnimHash, true);
            _player.OnAim.RemoveListener(OnAim);
            
            _player.ChangeState(Player.States.Move);
            _player.SetGlobalState(new PlayerGlobalState(_player));
        }

        private void OnAim(Vector2 aimPos)
        {
            _aimPosition = aimPos;
        }
        

        private async UniTaskVoid Charging()
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource = new CancellationTokenSource();
            
            _isCharging = true;
            _currentSpeed = 0;
            
            _player.ChangeState(Player.States.Empty);
            _player.SetGlobalState(null);
            
            _player.Animator.SetBool(PlayerIdleAnimHash, false);
            _dir = (_aimPosition - _player.Position ).normalized;
            
            var chargeEffect = await SystemManager.Instance.ResourceManager.GetFromPool<ParticleSystem>(ChargeEffectPath, _player.CenterTransform);
            chargeEffect.transform.ResetLocalTransform();
            
            chargeEffect.Play();
            
            while (_isCharging)
            {
                await UniTask.NextFrame(PlayerLoopTiming.FixedUpdate, _cancellationTokenSource.Token);
                
                _currentSpeed += Time.deltaTime * _speed;
                _currentSpeed = Mathf.Min(_currentSpeed, _speed);

                _player.Animator.SetFloat(MoveSpeedAnimHash, _currentSpeed * 0.5f);
                
                var targetDir = (_aimPosition - _player.Position ).normalized;
                _dir = Vector3.RotateTowards(_dir, targetDir, _maxSteerAngle * Mathf.Deg2Rad * Time.deltaTime, 0);
                
                _player.Rigidbody.velocity = _dir * _currentSpeed;
                
                var angle = Mathf.Atan2(_dir.y, _dir.x) * Mathf.Rad2Deg;
                chargeEffect.transform.rotation = Quaternion.Euler(0, 0, angle);
                
                
                Aim(-_dir);
                CheckHit();
                
                if (CheckWall())
                {
                    _isCharging = false;
                }

            }

            SystemManager.Instance.ResourceManager.ReleaseObject(ChargeEffectPath, chargeEffect);
            
            _player.Animator.SetTrigger(PlayerSwingAnimHash);
            _player.Animator.SetBool(PlayerIdleAnimHash, true);
            
            _player.ChangeState(Player.States.Move);
            _player.SetGlobalState(new PlayerGlobalState(_player));
        }

        private void CheckHit()
        {
            var damage = _player.StatComponent.GetDmg(PlayerStats.ChargeRigidDmg2);
            var shootSpeed = _player.StatComponent.GetDmg(PlayerStats.ChargeShootSpd2);
            
            Vector2 position = _player.EyeTransform.position;
    
            var hits = Physics2D.CircleCastAll(position, _size, _dir, _speed * Time.deltaTime,
                _player.ProjectileShooter.BounceMask);
    
            Debug.DrawRay(position, _dir * (_size + _speed * Time.deltaTime), Color.magenta, 1);
            Debug.DrawRay(position, new Vector3(-_dir.y, _dir.x) * (_size), Color.magenta, 1);
            Debug.DrawRay(position, new Vector3(_dir.y, -_dir.x) * (_size), Color.magenta, 1);

            bool isHit = false;
            
            foreach (var hit in hits)
            {
                if (hit.collider != null)
                {
                    if (hit.collider.TryGetComponent(out IHitAble hitAble))
                    {
                        hitAble.Hit(_dir, damage);
                        SystemManager.Instance.ResourceManager.EmitParticle(SwingBatHitPath, hitAble.Position);
                        isHit = true;
                    }
                    
                    if (hit.collider.TryGetComponent(out IProjectile projectile))
                    {
                        projectile.ProjectileHit(_dir, shootSpeed, _player.ProjectileShooter.BounceMask,
                            ProjectileOwner.Player, 0, false);
                        isHit = true;
                    }
                }
            }

            if (isHit)
            {
                var speed = _currentSpeed / _speed;
                _player.AttackImpulseSource.GenerateImpulse(_dir * speed);
            }
            
        }

        private bool CheckWall()
        {
            Vector2 position = _player.Position;

            var hit = Physics2D.CircleCast(position, 0.5f, _dir, 0.5f, WallLayer);

            if (hit)
            {
                var normal = hit.normal;
                var angle = Mathf.Atan2(normal.y, normal.x) * Mathf.Rad2Deg - 90;

                SystemManager.Instance.ResourceManager.EmitParticle(ShockEffectPath, hit.point, angle);
                _player.AttackImpulseSource.GenerateImpulse(normal * 3);

                _soundManager.PlayOneShot(_soundManager.SoundData.Boss_Rush_Crash, position);
                
                return true;
            }

            return false;
        }
    
        private void Aim(Vector2 aimDir)
        {
            var angle = Mathf.Atan2(aimDir.y, aimDir.x) * Mathf.Rad2Deg - 90;
            
            if (angle < 0)
            {
                angle += 360;
            }

            float flip = 180;
            if (angle > 180)
            {
                angle = 360 - angle;
                flip = 0;
            }

            var aimValue = (angle / 180 * 4);
            
            _player.Animator.SetFloat(AnimationMouseRotateHash, aimValue);
            _player.Animator.transform.rotation = Quaternion.Euler(0f, flip, 0f);
        }
        
    }
    
}
