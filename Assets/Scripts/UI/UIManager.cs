using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QT.UI
{
    public class UIManager : MonoBehaviour
    {
        private Dictionary<string, UIPanel> _Panels = new Dictionary<string, UIPanel>();
        private UIPanel _currentPanel;

        public void Initialize()
        {
            for(int i = 0; i < transform.childCount; i++)
            {
                UIPanel panel = transform.GetChild(i).GetComponent<UIPanel>();

                if (panel == null)
                    continue;

                panel.Initialize();
                if(!_Panels.ContainsKey(panel.GetType().ToString()))
                    _Panels.Add(panel.GetType().ToString(), panel);
            }
            
            foreach(KeyValuePair<string,UIPanel> panel in _Panels)
            {
                panel.Value.OnClose();
            }
        }

        public void PostSystemInitialize()
        {
            foreach (var panel in _Panels)
            {
                panel.Value.PostSystemInitialize();
            }
        }

        public void Open(string panelName) // TODO : 이 함수는 리팩토링 필요
        {
            //_currentPanel.OnClose();
            //_currentPanel = _Panels[panelName];
            //_currentPanel.OnOpen();
        }

        public T GetUIPanel<T>() where T : UIPanel
        {
            T uiPanel = null;
            string childUIPanelTypeNmae = typeof(T).ToString();
            if (_Panels.ContainsKey(childUIPanelTypeNmae))
            {
                uiPanel = _Panels[childUIPanelTypeNmae] as T;
            }

            return uiPanel;
        }
    }
}
