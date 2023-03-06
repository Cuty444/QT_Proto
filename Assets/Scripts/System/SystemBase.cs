using UnityEngine;

namespace QT.Core
{
    public class SystemBase : MonoBehaviour
    {
        public virtual void OnInitialized()
        {
            Debug.Log($"OnInitialized {gameObject.name}");
        }

        public virtual void OnPostInitialized()
        {
            Debug.Log($"OnPostInitialized {gameObject.name}");
        }
    }
}