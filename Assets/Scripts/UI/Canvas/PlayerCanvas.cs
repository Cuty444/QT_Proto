using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QT.UI
{
    public class PlayerCanvas : UIPanel
    {
        [SerializeField] private RectTransform _chargingBarBackground;
        public RectTransform ChargingBarBackground => _chargingBarBackground;
    }

}