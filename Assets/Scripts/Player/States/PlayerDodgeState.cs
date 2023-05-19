using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QT.Core;

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

            var force = _ownerEntity.GetStat(PlayerStats.DodgeAddForce).Value;

            _ownerEntity.Rigidbody.velocity = _ownerEntity.BefereDodgeDirecton * force;

            _ownerEntity.StartCoroutine(WaitSecond(force));
        }
        
        public override void ClearState()
        {
            _ownerEntity.SetDodgeEndAnimation();
            _ownerEntity.Rigidbody.velocity = Vector2.zero;
        }
        
        IEnumerator WaitSecond(float time)
        {
            yield return new WaitForSeconds(time);
            
            _ownerEntity.RevertToPreviousState();
        }
    }
}
