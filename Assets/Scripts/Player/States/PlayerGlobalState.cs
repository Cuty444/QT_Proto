using UnityEngine;
using System.Collections;
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
        
        private PlayerHPCanvas _playerHpCanvas;


        public PlayerGlobalState(IFSMEntity owner) : base(owner)
        {
            _playerHpCanvas = SystemManager.Instance.UIManager.GetUIPanel<PlayerHPCanvas>();
            _playerHpCanvas.gameObject.SetActive(true);
            
            _playerHpCanvas.SetHp(_ownerEntity.GetStatus(PlayerStats.HP));

            _ownerEntity.SetBatActive(false);
            SystemManager.Instance.PlayerManager.OnDamageEvent.AddListener(OnDamage);
            
            _ownerEntity.OnLook.AddListener(OnLook);
        }


        private void OnLook(Vector2 lookDir)
        {
            var angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg - 90;
            float flip = 180;
            
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
        }
        
        
        private void OnDamage(Vector2 dir, float damage)
        {
            _ownerEntity.Animator.SetTrigger(AnimationRigidHash);
            //_ownerEntity.PlayerHitEffectPlay();
            
            var hp = _ownerEntity.GetStatus(PlayerStats.HP);
            
            hp.AddStatus(-damage);
            _playerHpCanvas.CurrentHpImageChange(hp);
            
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