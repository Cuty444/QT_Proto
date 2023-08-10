using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QT.Core;

namespace QT.InGame
{
    // 그냥 아무것도 안하는 상태 (아이템 사용 중 같은 상황에 사용)
    [FSMState((int)Player.States.Empty)]
    public class PlayerEmptyState : FSMState<Player>
    {
        public PlayerEmptyState(IFSMEntity owner) : base(owner)
        {
            
        }
    }
}
