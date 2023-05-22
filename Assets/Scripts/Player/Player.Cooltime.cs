using System.Collections;
using System.Collections.Generic;
using QT.Core;
using QT.UI;
using UnityEngine;

namespace QT.InGame
{
    public partial class Player
    {
        private void UpdateCoolTime()
        {
            if (CurrentStateIndex == (int) States.Dead)
            {
                return;
            }
            
            GetStatus(PlayerStats.DodgeCooldown).AddStatus(Time.deltaTime);
            GetStatus(PlayerStats.SwingCooldown).AddStatus(Time.deltaTime);
            
            GetStatus(PlayerStats.MercyInvincibleTime).AddStatus(Time.deltaTime);
            GetStatus(PlayerStats.DodgeInvincibleTime).AddStatus(Time.deltaTime);
            
            var dodgeCooldown = GetStatus(PlayerStats.DodgeCooldown);
            dodgeCooldown.AddStatus(Time.deltaTime);
            
            SystemManager.Instance.UIManager.GetUIPanel<PlayerHPCanvas>().SetDodgeCoolTime(dodgeCooldown);
        }
        
        public bool IsInvincible()
        {
            return !GetStatus(PlayerStats.MercyInvincibleTime).IsFull()
                    || !GetStatus(PlayerStats.DodgeInvincibleTime).IsFull();
        }
    }
}
