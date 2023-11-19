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
                //_attackSpeedCanvas.gameObject.SetActive(false);
                return;
            }
            
            StatComponent.GetStatus(PlayerStats.DodgeCooldown).AddStatus(Time.deltaTime);
            StatComponent.GetStatus(PlayerStats.SwingCooldown).AddStatus(Time.deltaTime);
            
            StatComponent.GetStatus(PlayerStats.MercyInvincibleTime).AddStatus(Time.deltaTime);
            StatComponent.GetStatus(PlayerStats.DodgeInvincibleTime).AddStatus(Time.deltaTime);
            
            var dodgeCooldown = StatComponent.GetStatus(PlayerStats.DodgeCooldown);
            dodgeCooldown.AddStatus(Time.deltaTime);
            
            AttackCanvas();
        }

        private void AttackCanvas()
        {
            if (StatComponent.GetStatus(PlayerStats.SwingCooldown).IsFull())
            {
                //_attackSpeedCanvas.gameObject.SetActive(false);
            }
            else
            {
                //_attackSpeedCanvas.gameObject.SetActive(true);
                bool flip = PlayerAimAngle();
                //_attackSpeedBackground[flip ? 0 : 1].gameObject.SetActive(true);
                //_attackSpeedBackground[flip ? 1 : 0].gameObject.SetActive(false);
                var attackCoolTime = StatComponent.GetStatus(PlayerStats.SwingCooldown);
                Color color = _attackSpeedColorGradient.Evaluate(attackCoolTime.StatusValue / attackCoolTime.Value);
                float remap = Util.Math.Remap(attackCoolTime.StatusValue, attackCoolTime.Value, 0f);
                // for (int i = 0; i < _attackGaugeImages.Length; i++)
                // {
                //     _attackGaugeImages[i].fillAmount = remap;
                //     _attackGaugeImages[i].color = color;
                // }
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
