using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QT.UI
{

    public class UIPanel : MonoBehaviour
    {
        [SerializeField] private bool usePooling = false;
        public bool UsePooling => usePooling;
        
        public virtual void Initialize()
        {

        }

        public virtual void PostSystemInitialize()
        {

        }
        
        public virtual void OnOpen()
        {
            gameObject.SetActive(true);
        }

        public virtual void OnClose()
        {
            gameObject.SetActive(false);
        }
    }
}
