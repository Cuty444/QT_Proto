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
        [SerializeField] private TextMeshProUGUI _clearTimeText;

        private bool isSelected = false;

        private string _name = string.Empty;

        public override void OnOpen()
        {
            base.OnOpen();
            TimeSpan time = TimeSpan.FromSeconds(SystemManager.Instance.RankingManager.GetRankingTime());
            _clearTimeText.text = "#클리어_시간_" + time.ToString(@"hh\:mm\:ss");
            
            var _playerManager = SystemManager.Instance.PlayerManager;
            SystemManager.Instance.UIManager.GetUIPanel<MinimapCanvas>().CellClear();
            SystemManager.Instance.PlayerManager.AddItemEvent.RemoveAllListeners();
            _playerManager._playerIndexInventory.Clear();
            _playerManager.globalGold = 0;
            SystemManager.Instance.GetSystem<DungeonMapSystem>().SetFloor(0);
        }

        private void Update()
        {
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

        private void OpenReset()
        {
            
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
