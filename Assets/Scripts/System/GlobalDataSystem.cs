using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QT.Core.Data
{
    public class GlobalDataSystem : SystemBase
    {
        [SerializeField]
        // PlayerTable�� ���� PlayerSystem�� ��CharacterTable ���� �����ϵ��� ����
        private CharacterTable _characterTable;

        public CharacterTable CharacterTable => _characterTable;
        [SerializeField] private GlobalData _globalData;
        public GlobalData GlobalData => _globalData;

        [SerializeField] private BatTable _batTable;
        public BatTable BatTable => _batTable;

        [SerializeField]
        //Bat�� Ball�� ���� ������������ ������ ����
        private BallTable _ballTable;

        public BallTable BallTable => _ballTable;

        [SerializeField]
        // EnemyTable�� ���� EnemyController ������Ʈ�� ��üTable ���� �����ϵ��� ����
        private EnemyTable _enemyTable;

        public EnemyTable EnemyTable => _enemyTable;
    }
}