using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QT
{
    public class PlayerLineDrawer : MonoBehaviour
    {
        [HideInInspector] public LineRenderer LineRenderer;
        private void Awake()
        {
            LineRenderer = GetComponent<LineRenderer>();
        }
    }
}
