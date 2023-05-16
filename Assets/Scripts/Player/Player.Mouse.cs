using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QT.InGame
{
    public partial class Player
    {
        public Vector2 MousePos { get; private set; }

        public void MouseValueSet()
        {
            MousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
    }
}
