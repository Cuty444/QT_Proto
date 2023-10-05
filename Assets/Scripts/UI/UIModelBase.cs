using System.Collections;
using System.Collections.Generic;
using QT.Core;
using UnityEngine;

namespace QT.UI
{
    public abstract class UIModelBase
    {
        public UIPanel UIView {get; private set;}

        public abstract UIType UIType {get;}
        public abstract string PrefabPath {get;}

        public virtual void OnCreate(UIPanel view)
        {
            UIView = view;
        }

        public virtual void SetState(UIState state)
        {
            
        }
        
        public virtual void Show()
        {
            UIView.gameObject.SetActive(true);
        }

        public virtual void Hide()
        {
            UIView.gameObject.SetActive(false);
        }

        public virtual void ReleaseUI()
        {
            SystemManager.Instance.UIManager.Release(this);
        }
    }
}
