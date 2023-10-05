using System;
using System.Collections.Generic;
using QT.Core;
using QT.UI;
using TMPro;
using UnityEngine;

namespace QT.Ranking
{
    public class RankingCanvas : UIPanel
    {
        [SerializeField] private TextMeshProUGUI _time;
        [SerializeField] private GameObject _rankingNot;
        [SerializeField] private Transform _rankingPool;
        [SerializeField] private GameObject _rankingObject;
        private List<GameObject> _rankingObjectList = new List<GameObject>();
        
        [Space] [SerializeField] private TweenAnimator _popAnimation;
        
        public override void OnOpen()
        {
            base.OnOpen();
            var dataArray = SystemManager.Instance.RankingManager._rankingAllData.Data.ToArray();
            if (dataArray.Length > 0)
            {
                for (int i = 0; i < _rankingObjectList.Count; i++)
                {
                    Destroy(_rankingObjectList[i]);
                }
                _rankingObjectList.Clear();
                for (int i = 0; i < dataArray.Length; i++)
                {
                    for (int j = i + 1; j < dataArray.Length; j++)
                    {
                        if (dataArray[i].TotalTime > dataArray[j].TotalTime)
                        {
                            (dataArray[i], dataArray[j]) = (dataArray[j], dataArray[i]);
                        }
                    }
                }

                for (int i = 0; i < dataArray.Length; i++)
                {
                    var board = Instantiate(_rankingObject, _rankingPool).GetComponent<RankingBox>();
                    board.NickName.text = dataArray[i].Name;
                    TimeSpan time = TimeSpan.FromSeconds(dataArray[i].TotalTime);
                    board.gameObject.SetActive(true);
                    board.GetComponent<RectTransform>().sizeDelta = new Vector2(700f, 92f);
                    board.TotalTime.text = "#클리어_시간_" + time.ToString(@"hh\:mm\:ss");
                    _rankingObjectList.Add(board.gameObject);
                }
                _rankingNot.SetActive(false);
            }
            else
            {
                _rankingNot.SetActive(true);
            }
            
            _popAnimation.ReStart();
        }

        private void Update()
        {
            _time.text = DateTime.Now.ToString(@"hh:mm");
        }

        public override void OnClose()
        {
            base.OnClose();
            SystemManager.Instance.UIManager.GetUIPanel<TitleCanvas>();
        }
    }
}
