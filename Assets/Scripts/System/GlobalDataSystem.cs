using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QT.Core.Data
{
    public class GlobalDataSystem : SystemBase
    {

        [SerializeField]
        private CharacterAtkTable _characterAtkTable;

        public CharacterAtkTable CharacterAtkTable => _characterAtkTable;
        
        [SerializeField] private GlobalData _globalData;
        public GlobalData GlobalData => _globalData;
        
    }
}