using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QT.UI
{
    public class UIManager : MonoSingleton<UIManager>
    {
        Dictionary<string, UIPanel> _Panels = new Dictionary<string, UIPanel>();
        UIPanel _CurrentPanel;

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
        }

        public void PostSystemInitialize()
        {
            foreach (var panel in _Panels)
            {
                panel.Value.PostSystemInitialize();
            }
        }

        public void Open(string panelName)
        {
            _CurrentPanel.OnClose();
            _CurrentPanel = _Panels[panelName];
            _CurrentPanel.OnOpen();
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
