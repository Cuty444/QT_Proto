using QT.Core;
using QT.UI;
using UnityEngine;

namespace QT
{
    public class TitleScene : MonoBehaviour
    {
        private void Start()
        {
            SystemManager.Instance.UIManager.SetState(UIState.Title);
        }
    }
}
