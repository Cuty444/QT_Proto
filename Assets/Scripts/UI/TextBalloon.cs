using System;
using System.Collections;
using System.Collections.Generic;
using QT.Core;
using QT.Util;
using TMPro;
using UnityEngine;

namespace QT
{
    public class TextBalloon : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _desc;
        
        [SerializeField] private TweenAnimator _descAnimation;
        [SerializeField] private TweenAnimator _releaseAnimation;

        private LocaleGameDataBase _localeGameDataBase;

        private List<DialogueGameData> _data;

        private void Awake()
        {
            _localeGameDataBase = SystemManager.Instance.DataManager.GetDataBase<LocaleGameDataBase>();
        }

        public void Show(int dialogueId)
        {
            _releaseAnimation.Pause();
            _descAnimation.ReStart();

            _data = SystemManager.Instance.DataManager.GetDataBase<DialogueGameDataBase>().GetData(dialogueId);

            StartCoroutine(PlayDialogue());
        }
        
        
        private IEnumerator PlayDialogue()
        {
            foreach (var data in _data)
            {
                _desc.text = "";
                _desc.DOTMP(_localeGameDataBase.GetString(data.LocaleId), data.Duration);
                yield return new WaitForSeconds(data.Duration);
            }
        }
        
        public void Hide()
        {
            StopAllCoroutines();
            
            _descAnimation.Pause();
            _releaseAnimation.ReStart();
        }
        
    }
}
