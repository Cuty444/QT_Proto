using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace QT
{
    public class TriggerToActive : MonoBehaviour
    {
        public GameObject Target;

        public bool CanDeactive;
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            Target.SetActive(true);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (!CanDeactive)
            {
                return;
            }
            
            Target.SetActive(false);
        }
        
    }
}
