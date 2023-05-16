using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QT.Core;
using QT.Core.Input;

namespace QT.InGame
{
    [FSMState((int)Player.States.Dodge)]
    public class PlayerDodgeState : FSMState<Player>
    {
        public PlayerDodgeState(IFSMEntity owner) : base(owner)
        {
        }
        
        public override void InitializeState()
        {
            _ownerEntity.SetDodgeAnimation();
            _ownerEntity.DodgeEffectPlay();
            
            _ownerEntity.Rigidbody.velocity = Vector2.zero;
            _ownerEntity.Rigidbody.AddForce(_ownerEntity.BefereDodgeDirecton * _ownerEntity.GetStat(PlayerStats.DodgeAddForce).Value,ForceMode2D.Impulse);
            
            _ownerEntity.StartCoroutine(WaitSecond(_ownerEntity.GetStat(PlayerStats.DodgeDurationTime).Value));
        }
        
        public override void ClearState()
        {
            _ownerEntity.SetDodgeEndAnimation();
            _ownerEntity.Rigidbody.velocity = Vector2.zero;
        }
        
        IEnumerator WaitSecond(float time)
        {
            yield return new WaitForSeconds(time);
            _ownerEntity.ChangeState(Player.States.Idle);
        }
    }
}
