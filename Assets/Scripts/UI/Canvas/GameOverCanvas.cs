using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;
using QT.Core;
using QT.Util;
using QT.Core.Map;
using QT.InGame;

namespace QT.UI
{
    public class GameOverCanvas : UIPanel
    {
        [SerializeField] private GameObject _gameOverUI;
        [SerializeField] private Transform _panelTransform;
        [SerializeField] private SkeletonGraphic _skeletonGraphic;
        [SerializeField] private ButtonTrigger _retryButtonTrigger;
        [SerializeField] private ButtonTrigger _titleButtonTrigger;
        [SerializeField] private CanvasGroup _canvasGroup;

        private GameObject _uiObject = null;
        public override void OnOpen()
        {
            base.OnOpen();
            if (_uiObject != null)
            {
                Destroy(_uiObject);
            }
            _uiObject = Instantiate(_gameOverUI, _panelTransform);
            _uiObject.transform.localPosition = Vector3.zero;
            _skeletonGraphic = _uiObject.GetComponentInChildren<SkeletonGraphic>();
            _skeletonGraphic.AnimationState.SetAnimation(1, "S_GameOver",false);
            _canvasGroup.alpha = 0f;
            _canvasGroup.interactable = false;
            _retryButtonTrigger.InteractableOff();
            _titleButtonTrigger.InteractableOff();
            SystemManager.Instance.PlayerManager._playerIndexInventory.Clear();
            SystemManager.Instance.PlayerManager.globalGold = 0;
            SystemManager.Instance.GetSystem<DungeonMapSystem>().SetFloor(0);
            SystemManager.Instance.PlayerManager.OnDamageEvent.RemoveAllListeners();
            var buttonTrigger = _canvasGroup.GetComponentsInChildren<ButtonTrigger>()[1];
            buttonTrigger.InteractableOff();
            SystemManager.Instance.UIManager.GetUIPanel<MinimapCanvas>().CellClear();
            StartCoroutine(UnityUtil.WaitForFunc(() =>
            {
                StartCoroutine(UnityUtil.FadeCanvasGroup(_canvasGroup, 0f, 1f, 1.0f,()=>
                {
                    _canvasGroup.interactable = true;
                    _retryButtonTrigger.InteractableOn();
                    _titleButtonTrigger.InteractableOn();
                    buttonTrigger.InteractableOn();
                }));
                SystemManager.Instance.PlayerManager.CurrentRoomEnemyRegister.Invoke(new List<IHitable>());
                SystemManager.Instance.ProjectileManager.ProjectileListClear();
                SystemManager.Instance.ResourceManager.AllReleasedObject();
            }, 3.0f));
        }

        public void Retry()
        {
            SystemManager.Instance.PlayerManager.AddItemEvent.RemoveAllListeners();
            _skeletonGraphic.AnimationState.SetAnimation(1, "S_GameOver_Replay",false);
            StartCoroutine(UnityUtil.WaitForFunc(() =>
            {
                SystemManager.Instance.LoadingManager.LoadScene(1, OnClose);
                SystemManager.Instance.GetSystem<DungeonMapSystem>().DungenMapGenerate();
                SystemManager.Instance.UIManager.GetUIPanel<MinimapCanvas>().MinimapSetting();
                _retryButtonTrigger.Clear();
                _titleButtonTrigger.Clear();
                _uiObject.SetActive(false);
            }, 1f));
        }

        public void Exit()
        {
            SystemManager.Instance.PlayerManager.AddItemEvent.RemoveAllListeners();
            StartCoroutine(UnityUtil.WaitForFunc(() =>
            {
                SystemManager.Instance.LoadingManager.LoadScene(2,()=>
                {
                    OnClose();
                    SystemManager.Instance.GetSystem<DungeonMapSystem>().StartCoroutine(UnityUtil.WaitForFunc(() =>
                    {
                        SystemManager.Instance.UIManager.GetUIPanel<TitleCanvas>().OnOpen();
                    }, 1f));
                });
                SystemManager.Instance.GetSystem<DungeonMapSystem>().DungenMapGenerate();
                SystemManager.Instance.UIManager.GetUIPanel<MinimapCanvas>().MinimapSetting();
                _retryButtonTrigger.Clear();
                _titleButtonTrigger.Clear();
                _uiObject.SetActive(false);
            }, 1f));
        }
    }
}
