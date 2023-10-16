using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QT.UI
{

    public class UIPanel : MonoBehaviour
    {
        [SerializeField] private bool usePooling = false;
        public bool UsePooling => usePooling;
    }
}
