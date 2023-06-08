using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using QT.Core;
using QT.Core.Data;
using QT.UI;
using Unity.VisualScripting;
using UnityEngine.UI;

namespace QT.InGame
{
    [FSMState((int)Player.States.Global, false)]
    public class PlayerGlobalState : FSMState<Player>
    {
        private readonly int AnimationMouseRotateHash = Animator.StringToHash("MouseRotate");
        private readonly int AnimationRigidHash = Animator.StringToHash("PlayerRigid");
        private const string TeleportLinePath = "Prefabs/TeleportLine.prefab";

        private PlayerHPCanvas _playerHpCanvas;
        
        private GlobalDataSystem _globalDataSystem;

        public PlayerGlobalState(IFSMEntity owner) : base(owner)
        {
            _playerHpCanvas = SystemManager.Instance.UIManager.GetUIPanel<PlayerHPCanvas>();
            _playerHpCanvas.gameObject.SetActive(true);
            
            _playerHpCanvas.SetHp(_ownerEntity.GetStatus(PlayerStats.HP));
            SystemManager.Instance.ResourceManager.CacheAsset(TeleportLinePath);

            _ownerEntity.SetBatActive(false);
            SystemManager.Instance.PlayerManager.OnDamageEvent.AddListener(OnDamage);
            //SystemManager.Instance.PlayerManager.CurrentRoomEnemyRegister.AddListener(arg0 => TeleportLineClear());
            //SystemManager.Instance.PlayerManager.PlayerMapPosition.AddListener(arg0 => TeleportLineClear());

            _globalDataSystem = SystemManager.Instance.GetSystem<GlobalDataSystem>();
            _ownerEntity.OnAim.AddListener(OnAim);
        }


        private void OnAim(Vector2 aimPos)
        {
            if (_ownerEntity.CurrentStateIndex == (int)Player.States.Teleport)
            {
                return;
            }
            var aimDir = ((Vector2) _ownerEntity.transform.position - aimPos).normalized;

            if (_ownerEntity.IsReverseLookDir)
            {
                aimDir *= -1;
            }
            
            float flip = 180;
            if (_ownerEntity.IsDodge)
            {
                flip = _ownerEntity.IsFlip ? 180f : 0f;
                _ownerEntity.Animator.transform.rotation = Quaternion.Euler(0f, flip, 0f);
                return;
            }
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

            var aimValue = (angle / 180 * 4) + (_ownerEntity.CurrentStateIndex == (int)Player.States.Swing ? 5 : 0);
            _ownerEntity.Animator.SetFloat(AnimationMouseRotateHash, aimValue);
            _ownerEntity.Animator.transform.rotation = Quaternion.Euler(0f, flip, 0f);
        }
        
        private void OnDamage(Vector2 dir, float damage)
        {
            _ownerEntity.Animator.SetTrigger(AnimationRigidHash);
            //_ownerEntity.PlayerHitEffectPlay();
            
            var hp = _ownerEntity.GetStatus(PlayerStats.HP);
            _ownerEntity.MaterialChanger.SetHitMaterial();
            hp.AddStatus(-damage);
            _playerHpCanvas.CurrentHpImageChange(hp);

            _ownerEntity.DamageImpulseSource.GenerateImpulse(dir);
            
            if (hp <= 0)
            {
                PlayerDead();
            }
        }

        private void PlayerDead()
        {
            SystemManager.Instance.PlayerManager.PlayerThrowProjectileReleased.RemoveAllListeners();
            SystemManager.Instance.PlayerManager.OnDamageEvent.RemoveAllListeners();
            _ownerEntity.ChangeState(Player.States.Dead);
        }

    }

}