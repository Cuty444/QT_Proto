using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QT.UI
{

    public class UIPanel : MonoBehaviour
    {
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
