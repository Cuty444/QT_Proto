using System.Collections;
using System.Collections.Generic;
using QT.UI;
using UnityEngine;

public class PlayerCanvas : UIPanel
{
    [SerializeField] private RectTransform _chargingBarBackground;
    public RectTransform ChargingBarBackground => _chargingBarBackground;
}
