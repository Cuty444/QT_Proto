using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using QT.Core;
using UnityEngine.Events;

namespace QT.UI
{
    public enum UIType
    {
        Panel,
        Popup,
    }

    public enum UIState
    {
        None,
        Loading,
        Title,
        InGame,
        Inventory,
        Shop,
        Setting,
        Pause,
        GameOver,
        GameClear,
    }
    
    public class UIManager : MonoBehaviour
    {
        private const string PrefabPath = "UI/Prefab/";

        [SerializeField] private Transform panelParent;
        [SerializeField] private Transform popupParent;
        [SerializeField] private Transform deActiveParent;
        
        private Dictionary<Type, UIModelBase> _allUI = new ();
        private Stack<UIModelBase> _popupStack = new ();

        public UIState State { get; private set; } = UIState.None;
        
         #region Get

        public T Get<T>() where T : UIModelBase
        {
            if (_allUI.TryGetValue(typeof(T), out var model))
            {
                switch (model.UIType)
                {
                    case UIType.Panel:
                        model.UIView.transform.SetParent(panelParent);
                        break;
                    case UIType.Popup:

                        if(model.UIView.transform.parent == popupParent)
                        {
                            ReleasePopup(model);
                        }

                        model.UIView.transform.SetParent(popupParent);
                         model.UIView.transform.SetAsLastSibling();

                        _popupStack.Push(model);
                        break;
                }

                return (T) model;
            }

            model = (T) Activator.CreateInstance(typeof(T));

            SetView<T>(model);

            return (T) model;
        }

        private async void SetView<T>(UIModelBase model) where T : UIModelBase
        {
            var view = await SystemManager.Instance.ResourceManager.LoadAsset<UIPanel>(PrefabPath + model.PrefabPath, false);
            
            switch (model.UIType)
            {
                case UIType.Panel:
                    view.transform.SetParent(panelParent, false);
                    break;
                case UIType.Popup:
                    view.transform.SetParent(popupParent, false);
                    view.transform.SetAsLastSibling();

                    _popupStack.Push(model);
                    break;
            }
            
            model.OnCreate(view);

            if (view.UsePooling)
            {
                _allUI.Add(typeof(T), model);
            }
        }

        #endregion

        #region Release

        public void Release(UIModelBase model)
        {
            switch (model.UIType)
            {
                case UIType.Panel:
                    model.UIView.transform.SetParent(deActiveParent);
                    break;
                case UIType.Popup:
                    ReleasePopup(model);
                    if (model.UIView && !model.UIView.UsePooling)
                    {
                        Destroy(model.UIView);
                    }
                    break;
            }
        }

        private void ReleasePopup(UIModelBase model)
        {
            if (_popupStack.Count != 0 && _popupStack.Peek().Equals(model))
            {
                _popupStack.Pop();
                model.UIView.transform.SetParent(deActiveParent);
            }
            else if (_popupStack.Contains(model))
            {
                while (_popupStack.Count > 0)
                {
                    var last = _popupStack.Pop();
                    last.UIView.transform.SetParent(deActiveParent);

                    if (last.Equals(model))
                    {
                        return;
                    }
                }
            }
        }

        public void ReleaseAllPopups()
        {
            while (_popupStack.Count > 0)
            {
                var last = _popupStack.Pop();
                last.UIView.transform.SetParent(deActiveParent);
            }
        }

        #endregion
        
        public void SetState(UIState state)
        {
            State = state;

            foreach (var ui in _allUI)
            {
                ui.Value.SetState(state);
            }
        }

        public bool IsUIActive<T>() where T : UIModelBase
        {
            if (_allUI.TryGetValue(typeof(T), out var model))
            {
                return model.UIView.gameObject.activeInHierarchy;
            }

            return false;
        }
        
        
        
        
        public T GetUIPanel<T>() where T : UIPanel
        {
            return null;
        }
    }
}
