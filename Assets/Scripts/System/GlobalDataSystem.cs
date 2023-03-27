using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QT.Core.Data
{
    public class GlobalDataSystem : SystemBase
    {
        [SerializeField]
        // PlayerTable은 추후 PlayerSystem이 각CharacterTable 값을 저장하도록 관리
        private CharacterTable _characterTable;

        public CharacterTable CharacterTable => _characterTable;
        [SerializeField] private GlobalData _globalData;
        public GlobalData GlobalData => _globalData;

        [SerializeField] private BatTable _batTable;
        public BatTable BatTable => _batTable;

        [SerializeField]
        //Bat와 Ball은 추후 아이템형식이 나오면 수정
        private BallTable _ballTable;

        public BallTable BallTable => _ballTable;

        [SerializeField]
        // EnemyTable은 추후 EnemyController 컴포넌트가 개체Table 값을 저장하도록 관리
        private EnemyTable _enemyTable;

        public EnemyTable EnemyTable => _enemyTable;
    }
}