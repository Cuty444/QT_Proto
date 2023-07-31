using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks.Triggers;
using QT.Core;
using QT.Core.Map;
using QT.Sound;
using QT.Tutorial;
using QT.Util;
using UnityEngine;
using UnityEngine.UI;

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

        [SerializeField] private ButtonTrigger _tutorialButtonTrigger;
        [SerializeField] private ButtonTrigger _titleButtonTrigger;

        [SerializeField] private Slider[] _soundSliders;
        private bool _isOpen = false;

        private bool _isInventory = true;

        public bool _isTutorial { get; private set; } = false;

        private SoundManager _soundManager;
        public override void PostSystemInitialize()
        {
            gameObject.SetActive(true);
            _inventoryPage.Initialize();

            _backGround.SetActive(false);
            _soundManager = SystemManager.Instance.SoundManager;
            float volume = 0;
            _soundManager.GetMasterVolume(out volume);
            _soundSliders[0].value = volume;
            _soundManager.GetBGMVolume(out volume);
            _soundSliders[1].value = volume;
            _soundManager.GetSFXVolume(out volume);
            _soundSliders[2].value = volume;
        }

        public void SetMasterVolume(float volume)
        {
            _soundManager.SetMasterVolume(volume);
        }
        
        public void SetBGMVolume(float volume)
        {
            _soundManager.SetBGMVolume(volume);
        }
        
        public void SetSFXVolume(float volume)
        {
            _soundManager.SetSFXVolume(volume);
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
            
            SystemManager.Instance.SoundManager.VolumeSave();
        }

        private void CheckOpen()
        {
            if (_isTutorial)
                return;
            StopAllCoroutines();

            _isOpen = !_isOpen;
            SystemManager.Instance.UIManager.InventoryInputCheck.Invoke(_isOpen);
            SystemManager.Instance.SoundManager.PlayOneShot(SystemManager.Instance.SoundManager.SoundData.UITabSFX);
            if (_isOpen)
            {
                SetPage();
                _tutorialButtonTrigger.InteractableOn();
                _titleButtonTrigger.InteractableOn();
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

        public void TutorialButton()
        {
            SystemManager.Instance.UIManager.GetUIPanel<TutorialCanvas>().OnOpen();
            CheckOpen();
            _isTutorial = true;
        }

        public void TutorialClose()
        {
            _isTutorial = false;
            _tutorialButtonTrigger.InteractableOn();
            CheckOpen();
        }

        public void TitleButton()
        {
            var _playerManager = SystemManager.Instance.PlayerManager;
            SystemManager.Instance.UIManager.GetUIPanel<MinimapCanvas>().CellClear();
            SystemManager.Instance.PlayerManager.AddItemEvent.RemoveAllListeners();
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
                ProjectileManager.Instance.Clear();
                HitAbleManager.Instance.Clear();
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