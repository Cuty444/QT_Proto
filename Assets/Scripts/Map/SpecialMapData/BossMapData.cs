using System.Collections;
using System.Collections.Generic;
using QT.Core;
using QT.Map;
using UnityEngine;

namespace QT
{
    public class BossMapData : SpecialMapData
    {
        [field:SerializeField] public EnemyWave BossWave { get; private set; }
    }
}
