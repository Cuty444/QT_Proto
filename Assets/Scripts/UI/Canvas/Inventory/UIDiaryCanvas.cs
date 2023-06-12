using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks.Triggers;
using QT.Core;
using QT.Core.Map;
using QT.Util;
using UnityEngine;

namespace QT.UI
{
    public class UIDiaryCanvas : UIPanel
    {
        [SerializeField] private GameObject _backGround;

        [SerializeField] private GameObject _settingGameobject;
        [SerializeField] private UIInventoryPage _inventoryPage;

        [Space] [SerializeField] private UITweenAnimator _popAnimation;
        [SerializeField] private UITweenAnimator _releaseAnimation;
        [SerializeField] private UITweenAnimator _switchAnimation;

        [Space] public Transform MapTransform;

        private bool _isOpen = false;

        private bool _isInventory = true;

        public override void PostSystemInitialize()
        {
            gameObject.SetActive(true);
            _inventoryPage.Initialize();

            _backGround.SetActive(false);
        }

        private void Update()
        {
            CheckInput();
        }

        private void CheckInput()
        {
            if (SystemManager.Instance.PlayerManager.Player == null)
            {
                return;
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (_isOpen && _isInventory)
                {
                    SwitchPage(false);
                }
                else
                {
                    _isInventory = false;
                    CheckOpen();
                }
            }

            if (Input.GetKeyDown(KeyCode.Tab))
            {
                if (_isOpen && !_isInventory)
                {
                    SwitchPage(true);
                }
                else
                {
                    _isInventory = true;
                    CheckOpen();
                }
            }
        }

        private void CheckOpen()
        {
            StopAllCoroutines();

            _isOpen = !_isOpen;
            SystemManager.Instance.UIManager.InventoryInputCheck.Invoke(_isOpen);
            SystemManager.Instance.SoundManager.PlayOneShot(SystemManager.Instance.SoundManager.SoundData.UITabSFX);
            if (_isOpen)
            {
                SetPage();

                _backGround.SetActive(true);
                _popAnimation.ReStart();
            }
            else
            {
                StartCoroutine(CloseCorutine());
            }
        }

        private void SetPage()
        {
            _settingGameobject.SetActive(!_isInventory);
            _inventoryPage.gameObject.SetActive(_isInventory);

            if (_isInventory)
            {
                _inventoryPage.SetInventoryUI();
            }
            else
            {
                // 세팅 페이지 세팅
            }
        }

        public void SwitchPage(bool isInventory)
        {
            _isInventory = isInventory;
            StartCoroutine(SwitchCorutine());
        }

        private IEnumerator SwitchCorutine()
        {
            _switchAnimation.ReStart();

            yield return new WaitForSeconds(0.2f);

            SetPage();
        }


        private IEnumerator CloseCorutine()
        {
            _releaseAnimation.ReStart();

            yield return new WaitForSeconds(_releaseAnimation.SequenceLength);

            _backGround.SetActive(false);
        }

        public void TitleButton()
        {
            var _playerManager = SystemManager.Instance.PlayerManager;
            SystemManager.Instance.UIManager.GetUIPanel<MinimapCanvas>().CellClear();
            SystemManager.Instance.PlayerManager.AddItemEvent.RemoveAllListeners();
            _playerManager._playerIndexInventory.Clear();
            _playerManager.globalGold = 0;
            SystemManager.Instance.GetSystem<DungeonMapSystem>().SetFloor(0);
            SystemManager.Instance.RankingManager.PlayerOn.Invoke(false);
            SystemManager.Instance.RankingManager.ResetRankingTime();
            var uiManager = SystemManager.Instance.UIManager;
            uiManager.GetUIPanel<FadeCanvas>().FadeOut(() =>
            {
                uiManager.GetUIPanel<MinimapCanvas>().OnClose();
                _isOpen = false;
                StartCoroutine(CloseCorutine());
                uiManager.GetUIPanel<FadeCanvas>().FadeIn();
                uiManager.GetUIPanel<LoadingCanvas>().OnOpen();
                SystemManager.Instance.PlayerManager.OnDamageEvent.RemoveAllListeners();
                SystemManager.Instance.UIManager.GetUIPanel<MinimapCanvas>().CellClear();
                SystemManager.Instance.PlayerManager.CurrentRoomEnemyRegister.Invoke(new List<IHitable>());
                SystemManager.Instance.ProjectileManager.ProjectileListClear();
                SystemManager.Instance.ResourceManager.AllReleasedObject();

                SystemManager.Instance.GetSystem<DungeonMapSystem>().StartCoroutine(UnityUtil.WaitForFunc(() =>
                {
                    SystemManager.Instance.LoadingManager.FloorLoadScene(2);
                    SystemManager.Instance.GetSystem<DungeonMapSystem>().StartCoroutine(UnityUtil.WaitForFunc(() =>
                    {
                        SystemManager.Instance.UIManager.GetUIPanel<TitleCanvas>().OnOpen();
                        SystemManager.Instance.GetSystem<DungeonMapSystem>().DungenMapGenerate();
                        SystemManager.Instance.UIManager.GetUIPanel<MinimapCanvas>().MinimapSetting();
                    }, 2f));
                }, 5f));
            });
        }
    }
}