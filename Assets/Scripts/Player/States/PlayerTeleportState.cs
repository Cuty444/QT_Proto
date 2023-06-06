using System.Collections;
using System.Collections.Generic;
using QT.Core;
using QT.InGame;
using UnityEngine;

namespace QT
{
    [FSMState((int)Player.States.Teleport)]
    public class PlayerTeleportState :  FSMState<Player>
    {
        private readonly int AnimationMouseRotateHash = Animator.StringToHash("MouseRotate");
        private readonly int AnimationTeleportHash = Animator.StringToHash("PlayerTeleport");
        private readonly int AnimationTeleportEndHash = Animator.StringToHash("PlayerTeleportEnd");
        LayerMask BounceMask => LayerMask.GetMask("Wall", "Enemy");
        private const string SwingProjectileHitPath = "Effect/Prefabs/FX_Ball_Attack.prefab";
        public PlayerTeleportState(IFSMEntity owner) : base(owner)
        {
        }

        public override void InitializeState()
        {
            _ownerEntity.GetStatus(PlayerStats.MercyInvincibleTime).SetStatus(0.5f);
            _ownerEntity.transform.position = _ownerEntity._rigidTeleportEnemy.Position;
            _ownerEntity.Animator.SetTrigger(AnimationTeleportHash);
            _ownerEntity.StartCoroutine(Util.UnityUtil.WaitForFunc(() =>
            {
                _ownerEntity.Animator.ResetTrigger(AnimationTeleportHash);
            }, 0.2f));
            var aimDir = ((Vector2) _ownerEntity.transform.position - _ownerEntity._rigidTargetEnemy.Position).normalized;
            float flip = 180;
            var angle = Mathf.Atan2(aimDir.y, aimDir.x) * Mathf.Rad2Deg - 90;
            if (angle < 0)
            {
                angle += 360;
            }
            _ownerEntity.EyeTransform.rotation = Quaternion.Euler(0, 0, angle + 270);
            if (angle > 180)
            {
                angle = 360 - angle;
                flip = 0;
            }
            _ownerEntity.Animator.SetFloat(AnimationMouseRotateHash, angle / 180 * 4);
            _ownerEntity.Animator.transform.rotation = Quaternion.Euler(0f, flip, 0f);

            _ownerEntity._rigidTeleportEnemy.ProjectileHit((_ownerEntity._rigidTargetEnemy.Position - _ownerEntity._rigidTeleportEnemy.Position).normalized
                , _ownerEntity.GetStat(PlayerStats.ChargeShootSpd2), BounceMask
                , ProjectileOwner.PlayerTeleport
                , _ownerEntity.GetStat(PlayerStats.ReflectCorrection));
            SystemManager.Instance.ResourceManager.EmitParticle(SwingProjectileHitPath, _ownerEntity._rigidTargetEnemy.Position);
            //var angle = Util.Math.GetDegree(_ownerEntity.transform.position, _ownerEntity._rigidTargetEnemy.Position);
            //_ownerEntity.EyeTransform.rotation = Quaternion.Euler(0, 0, angle);
            _ownerEntity.swingSlashEffectPlay();
            _ownerEntity.PlayBatAnimation();
            _ownerEntity.StartCoroutine(Util.UnityUtil.WaitForFunc(() =>
            {
                _ownerEntity.RevertToPreviousState();
            },0.2f));
        }

        public override void ClearState()
        {
            _ownerEntity.Animator.SetTrigger(AnimationTeleportEndHash);
            _ownerEntity.StartCoroutine(Util.UnityUtil.WaitForFunc(() =>
            {
                _ownerEntity.Animator.ResetTrigger(AnimationTeleportEndHash);
            }, 0.2f));
        }
    }
}
