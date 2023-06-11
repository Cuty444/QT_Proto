using System.Collections;
using System.Collections.Generic;
using QT.Core;
using QT.Core.Data;
using QT.UI;
using UnityEngine;

namespace QT.InGame
{
    public partial class Player
    {
        private Gradient _attackSpeedColorGradient;
        private void UpdateCoolTime()
        {
            if (CurrentStateIndex == (int) States.Dead)
            {
                _attackSpeedCanvas.gameObject.SetActive(false);
                return;
            }
            
            GetStatus(PlayerStats.DodgeCooldown).AddStatus(Time.deltaTime);
            GetStatus(PlayerStats.SwingCooldown).AddStatus(Time.deltaTime);
            
            GetStatus(PlayerStats.MercyInvincibleTime).AddStatus(Time.deltaTime);
            GetStatus(PlayerStats.DodgeInvincibleTime).AddStatus(Time.deltaTime);
            
            var dodgeCooldown = GetStatus(PlayerStats.DodgeCooldown);
            dodgeCooldown.AddStatus(Time.deltaTime);
            
            SystemManager.Instance.UIManager.GetUIPanel<PlayerHPCanvas>().SetDodgeCoolTime(dodgeCooldown);
            AttackCanvas();
        }
        
        public bool IsInvincible()
        {
            return !GetStatus(PlayerStats.MercyInvincibleTime).IsFull()
                    || !GetStatus(PlayerStats.DodgeInvincibleTime).IsFull()
                    || CurrentStateIndex == (int)Player.States.Teleport;
        }

        private void AttackCanvas()
        {
            if (GetStatus(PlayerStats.SwingCooldown).IsFull())
            {
                _attackSpeedCanvas.gameObject.SetActive(false);
            }
            else
            {
                _attackSpeedCanvas.gameObject.SetActive(true);
                bool flip = PlayerAimAngle();
                _attackSpeedBackground[flip ? 0 : 1].gameObject.SetActive(true);
                _attackSpeedBackground[flip ? 1 : 0].gameObject.SetActive(false);
                var attackCoolTime = GetStatus(PlayerStats.SwingCooldown);
                Color color = _attackSpeedColorGradient.Evaluate(attackCoolTime.StatusValue / attackCoolTime.Value);
                float remap = Util.Math.Remap(attackCoolTime.StatusValue, attackCoolTime.Value, 0f);
                for (int i = 0; i < _attackGaugeImages.Length; i++)
                {
                    _attackGaugeImages[i].fillAmount = remap;
                    _attackGaugeImages[i].color = color;
                }
            }
        }
        
        private bool PlayerAimAngle()
        {
            float playerRotation = EyeTransform.rotation.z;
            if (playerRotation < -0.7f)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
