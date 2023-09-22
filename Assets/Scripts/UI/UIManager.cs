using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;

namespace QT.UI
{
    public class UIManager : MonoBehaviour
    {
        private readonly Dictionary<Type, UIPanel> _Panels = new Dictionary<Type, UIPanel>();
        private UIPanel _currentPanel;


        public void Initialize()
        {
            for(int i = 0; i < transform.childCount; i++)
            {
                UIPanel panel = transform.GetChild(i).GetComponent<UIPanel>();

                if (panel == null)
                    continue;

                panel.Initialize();
                if(!_Panels.ContainsKey(panel.GetType()))
                    _Panels.Add(panel.GetType(), panel);
            }
            
            foreach(KeyValuePair<Type,UIPanel> panel in _Panels)
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
            if (_Panels.TryGetValue(typeof(T), out var system))
            {
                return (T)system;
            }

            return null;
        }
    }
}
