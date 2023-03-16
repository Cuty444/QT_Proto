using System.Collections;
using System.Collections.Generic;
using QT.Core;
using UnityEngine;
namespace QT.Data
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
    public class GlobalDataSystem : SystemBase
    {
        [SerializeField]
        private CharacterTable _characterTable;
        public CharacterTable CharacterTable => _characterTable;

        [SerializeField]
        private GlobalData _globalData;
        public GlobalData GlobalData => _globalData;

        [SerializeField]
        private BatTable _batTable;
        public BatTable BatTable => _batTable;

        [SerializeField]
        private BallTable _ballTable;
        public BallTable BallTable => _ballTable;

        [SerializeField]
        private EnemyTable _enemyTable;
        public EnemyTable EnemyTable => _enemyTable;
    }
}