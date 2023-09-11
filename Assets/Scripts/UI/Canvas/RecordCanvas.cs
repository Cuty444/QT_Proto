using System;
using System.Collections;
using System.Collections.Generic;
using QT.Core;
using QT.Core.Map;
using QT.UI;
using QT.Util;
using TMPro;
using UnityEngine;

namespace QT
{
    public class RecordCanvas : UIPanel
    {
        [SerializeField] private TextMeshProUGUI _time;
        [SerializeField] private TextMeshProUGUI _clearTimeText;

        private bool isSelected = false;

        private string _name = string.Empty;

        private DungeonMapSystem _dungeonMapSystem;

        [Space] [SerializeField] private UITweenAnimator _popAnimation;

        public override void Initialize()
        {
            _dungeonMapSystem = SystemManager.Instance.GetSystem<DungeonMapSystem>();
        }
        
        public override void OnOpen()
        {
            base.OnOpen();
            TimeSpan time = TimeSpan.FromSeconds(SystemManager.Instance.RankingManager.GetRankingTime());
            _clearTimeText.text = "#클리어_시간_" + time.ToString(@"hh\:mm\:ss");
            
            var _playerManager = SystemManager.Instance.PlayerManager;
            SystemManager.Instance.UIManager.GetUIPanel<MinimapCanvas>().CellClear();
            SystemManager.Instance.PlayerManager.AddItemEvent.RemoveAllListeners();
            _playerManager.Reset();
            
            _playerManager.PlayerIndexInventory.Clear();
            _playerManager.PlayerActiveItemIndex = -1;
            
            _dungeonMapSystem.SetFloor(0);
            
            _popAnimation.ReStart();
        }

        private void Update()
        {
            _time.text = DateTime.Now.ToString(@"hh:mm");
            
            if (Input.GetKeyDown(KeyCode.Return) && !isSelected)
            {
                if (_name == string.Empty)
                {
                    return;
                }
                SystemManager.Instance.RankingManager.AddRankingData(_name);
                Title();
            }
        }

        private void Title()
        {
            var uiManager = SystemManager.Instance.UIManager;
            uiManager.GetUIPanel<FadeCanvas>().FadeOut(() =>
            {
                OnClose();
                uiManager.GetUIPanel<MinimapCanvas>().OnClose();
                uiManager.GetUIPanel<FadeCanvas>().FadeIn();
                uiManager.GetUIPanel<LoadingCanvas>().OnOpen();
                SystemManager.Instance.UIManager.GetUIPanel<MinimapCanvas>().CellClear();
                ProjectileManager.Instance.Clear();
                HitAbleManager.Instance.Clear();
                SystemManager.Instance.ResourceManager.AllReleasedObject();

                _dungeonMapSystem.StartCoroutine(UnityUtil.WaitForFunc(() =>
                {
                    SystemManager.Instance.LoadingManager.FloorLoadScene(2);
                    _dungeonMapSystem.StartCoroutine(UnityUtil.WaitForFunc(() =>
                    {
                        SystemManager.Instance.UIManager.GetUIPanel<TitleCanvas>().OnOpen();
                        //_dungeonMapSystem.DungenMapGenerate();
                        //SystemManager.Instance.UIManager.GetUIPanel<MinimapCanvas>().MinimapSetting(); TODO : 이 부분 로딩 정리하기
                    }, 2f));
                },5f));
                //SystemManager.Instance.StageLoadManager.StageLoad((_dungeonMapSystem.GetFloor() + 1).ToString());
            });
        }
        

        public void ChangedName(string text)
        {
            _name = text;
        }
        
        public void EndCheck(string text)
        {
            _name = text;
            isSelected = false;
        }

        public void DeSelect()
        {
            isSelected = false;
        }

        public void Select()
        {
            isSelected = true;
        }
    }
}
