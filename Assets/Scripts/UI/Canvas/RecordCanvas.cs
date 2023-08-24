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

        [Space] [SerializeField] private UITweenAnimator _popAnimation;
        
        public override void OnOpen()
        {
            base.OnOpen();
            TimeSpan time = TimeSpan.FromSeconds(SystemManager.Instance.RankingManager.GetRankingTime());
            _clearTimeText.text = "#클리어_시간_" + time.ToString(@"hh\:mm\:ss");
            
            var _playerManager = SystemManager.Instance.PlayerManager;
            SystemManager.Instance.UIManager.GetUIPanel<MinimapCanvas>().CellClear();
            SystemManager.Instance.PlayerManager.AddItemEvent.RemoveAllListeners();
            _playerManager.globalGold = 0;
            
            _playerManager.Player.Inventory.ClearItems();
            _playerManager.PlayerIndexInventory.Clear();
            _playerManager.PlayerActiveItemIndex = -1;
            
            SystemManager.Instance.GetSystem<DungeonMapSystem>().SetFloor(0);
            
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
                        //SystemManager.Instance.UIManager.GetUIPanel<MinimapCanvas>().MinimapSetting(); TODO : 이 부분 로딩 정리하기
                    }, 2f));
                },5f));
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
