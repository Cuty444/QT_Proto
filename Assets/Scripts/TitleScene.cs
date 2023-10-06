using Cysharp.Threading.Tasks;
using QT.Core;
using QT.Core.Map;
using QT.UI;
using UnityEngine;

namespace QT
{
    public class TitleScene : MonoBehaviour
    {
        private async void Start()
        {
            await SystemManager.Instance.StageLoadManager.StageLoad("1");
            
            SystemManager.Instance.UIManager.SetState(UIState.Title);
        }
    }
}
