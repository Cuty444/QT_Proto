using System;
using System.Collections;
using System.Collections.Generic;
using QT.Core;
using QT.UI;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

namespace QT.Tutorial
{
    [Serializable]
    public class VideoData
    {
        public GameObject Pool;
        public VideoPlayer Video;

        public VideoData(GameObject p, VideoPlayer v)
        {
            Pool = p;
            Video = v;
        }
    }
    public class TutorialCanvas : UIPanel
    {
        [SerializeField] private GameObject _leftButton;
        [SerializeField] private GameObject _rightButton;
        [SerializeField] private Image[] _circleImages;
        [SerializeField] private Sprite[] _circleSprite;
        [SerializeField] private GameObject _battle;
        [SerializeField] private GameObject _control;

        [SerializeField] private VideoData[] _battleData;
        [SerializeField] private VideoData[] _controlData;

        private Dictionary<bool, VideoData[]> dataDictionary = new Dictionary<bool, VideoData[]>();
        private int circleIndex = 0;
        private bool isBC = false;

        public override void PostSystemInitialize()
        {
            base.PostSystemInitialize();
            dataDictionary.Add(false,_controlData);
            dataDictionary.Add(true,_battleData);
        }

        public override void OnOpen()
        {
            base.OnOpen();
            ControlOpen();
        }

        public override void OnClose()
        {
            base.OnClose();
            if (SystemManager.Instance.UIManager.GetUIPanel<UIDiaryCanvas>()._isTutorial)
            {
                SystemManager.Instance.UIManager.GetUIPanel<UIDiaryCanvas>().TutorialClose();
            }
            else
            {
                SystemManager.Instance.UIManager.GetUIPanel<TitleCanvas>().TutorialClose();
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                OnClose();
            }
        }

        public void BattleOpen()
        {
            isBC = true;
            _battle.SetActive(true);
            _control.SetActive(false);
            IndexChange(0);
            _leftButton.SetActive(false);
            _rightButton.SetActive(true);
        }

        public void ControlOpen()
        {
            isBC = false;
            _battle.SetActive(false);
            _control.SetActive(true);
            IndexChange(0);
            _leftButton.SetActive(false);
            _rightButton.SetActive(true);
        }

        public void IndexPlus()
        {
            circleIndex++;
            if (circleIndex >= _battleData.Length-1)
            {
                _rightButton.SetActive(false);
            }
            _leftButton.SetActive(true);
            IndexChange(circleIndex);
        }

        public void IndexMinus()
        {
            circleIndex--;
            if (circleIndex <= 0)
            {
                _leftButton.SetActive(false);
            }
            _rightButton.SetActive(true);
            IndexChange(circleIndex);
        }

        private void IndexChange(int index)
        {
            circleIndex = index;
            for (int i = 0; i < dataDictionary[isBC].Length; i++)
            {
                dataDictionary[isBC][i].Pool.SetActive(false);
            }
            dataDictionary[isBC][index].Pool.SetActive(true);
            dataDictionary[isBC][index].Video.time = 0f;
            dataDictionary[isBC][index].Video.Play();
            for (int i = 0; i < _circleImages.Length; i++)
            {
                if (circleIndex == i)
                {
                    _circleImages[i].sprite = _circleSprite[1];
                }
                else
                {
                    _circleImages[i].sprite = _circleSprite[0];
                }
            }
        }
        
    }
}
