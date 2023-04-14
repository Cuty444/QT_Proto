using System;
using System.Collections;
using System.Collections.Generic;
using QT.Core.Map;
using UnityEngine;
using UnityEngine.UI.Extensions;

namespace QT.Map
{
    [Flags]
    public enum MapDirection
    {
        None = 0,
        Up = 1,
        Down = 2,
        Left = 4,
        Right = 8,
    }
    
    public class CellRouteLineDrawer : MonoBehaviour
    {
        [SerializeField] private GameObject _lineRenders;
        [SerializeField] private UILineRenderer[] _uiLineRenderers;
        private void Awake()
        {
            //_lineRenders.SetActive(false);
        }

        public void SetRouteDirection(MapDirection mapDirection)
        {
            _uiLineRenderers[0].enabled = (mapDirection & MapDirection.Up) != 0;
            _uiLineRenderers[1].enabled = (mapDirection & MapDirection.Down) != 0;
            _uiLineRenderers[2].enabled = (mapDirection & MapDirection.Left) != 0;
            _uiLineRenderers[3].enabled = (mapDirection & MapDirection.Right) != 0;
        }
    }
}
