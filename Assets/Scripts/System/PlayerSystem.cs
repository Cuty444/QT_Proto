using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace QT.Core.Player
{
    public enum PlayerMoveState
    {
        Idle,
        Move,
        Dash,
    }
    public enum PlayerAttackState
    {
        Idle,
        Shot,
        Swing,
    }
    public class PlayerSystem : SystemBase
    {
        [SerializeField]
        private CharacterTable _characterTable;
        public CharacterTable CharacterTable { get => _characterTable; }
        [SerializeField]
        private GlobalData _globalData;
        public GlobalData GlobalData { get => _globalData; }
        [SerializeField]
        private BatTable _batTable;
        public BatTable BatTable { get => _batTable; }
        [SerializeField]
        private BallTable _ballTable;
        public BallTable BallTable { get => _ballTable; }
    }
}