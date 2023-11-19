using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QT
{
    public class RouletteMapData : SpecialMapData
    {
        [SerializeField] private SlotMachine _slotMachine;

        private void Awake()
        {
            _slotMachine = GetComponentInChildren<SlotMachine>();
        }

        public void MapClearRouletteOn()
        {
            _slotMachine.MapClear();
        }
    }
}
