using System.Collections.Generic;
using UnityEngine;
using QT.Core;
using QT.Core.Data;
using QT.UI;
using UnityEngine.PlayerLoop;

namespace QT.InGame
{
    [FSMState((int) Player.States.Throw)]
    public class PlayerThrowState : PlayerMoveState
    {
        private readonly int AnimationThrowHash = Animator.StringToHash("PlayerThrow");
        
        public PlayerThrowState(IFSMEntity owner) : base(owner)
        {
            SystemManager.Instance.PlayerManager.PlayerThrowProjectileReleased.AddListener(() =>
            {
                SystemManager.Instance.UIManager.GetUIPanel<PlayerHPCanvas>().ThrowProjectileGauge(true);
                
                _ownerEntity.StatComponent.GetStatus(PlayerStats.BallStack).AddStatus(1);
            });
        }

        public override void InitializeState()
        {
            base.InitializeState();
            
            _ownerEntity.StatComponent.GetStatus(PlayerStats.ThrowCooldown).SetStatus(0);
            
            var stack = _ownerEntity.StatComponent.GetStatus(PlayerStats.BallStack);

            if (stack <= 0)
            {
                _ownerEntity.RevertToPreviousState();
                return;
            }
            
            stack.AddStatus(-1);;
            
            _ownerEntity.Animator.SetTrigger(AnimationThrowHash);
            
            var throwSpd = _ownerEntity.StatComponent.GetStat(PlayerStats.ThrowSpd).Value;
            var throwBounceCount = (int)_ownerEntity.StatComponent.GetStat(PlayerStats.ThrowBounceCount).Value;
            bool isPierce = _ownerEntity.StatComponent.GetStat(PlayerStats.ChargeAtkPierce).Value >= 1;

            _ownerEntity.ProjectileShooter.ShootProjectile(200,
                Util.Math.ZAngleToGetDirection(_ownerEntity.EyeTransform), throwSpd, 0, throwBounceCount,
                ProjectileOwner.Player, isPierce, 2.5f);
            
            _ownerEntity.RevertToPreviousState();
        }
        
        protected override void OnThrow(bool isOn)
        {
        }
    }
}