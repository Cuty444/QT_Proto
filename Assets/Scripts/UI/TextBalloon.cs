using System;
using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using FMODUnity;
using QT.Core;
using QT.Sound;
using QT.Util;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace QT
{
    public class TextBalloon : MonoBehaviour
    {
        [SerializeField] private float _playSpeed = 0.05f;
        [SerializeField] private EventReference _dialogSoundEvent;
        
        [SerializeField] private TextMeshProUGUI _desc;
        
        [SerializeField] private TweenAnimator _descAnimation;
        [SerializeField] private TweenAnimator _releaseAnimation;
        
        private SoundManager _soundManager;
        private LocaleGameDataBase _localeGameDataBase;

        
        private List<DialogueGameData> _data;

        private Coroutine _messageCoroutine;
        private int _index;

        private bool _isPlaying = false;
        private bool _isTyping = false;
        
        private float _timer;
        private float _duration;

        private string _message;


        private void Awake()
        {
            _soundManager = SystemManager.Instance.SoundManager;
            _localeGameDataBase = SystemManager.Instance.DataManager.GetDataBase<LocaleGameDataBase>();
        }

        public void Show(int dialogueId)
        {
            _releaseAnimation.Pause();
            _descAnimation.ReStart();

            _data = SystemManager.Instance.DataManager.GetDataBase<DialogueGameDataBase>().GetData(dialogueId);
            
            _index = 0;
            _duration = _timer = 0;
            
            _isPlaying = true;
            _isTyping = false;
            //StartCoroutine(PlayDialogue());
        }

        private void Update()
        {
            if (!_isPlaying || _isTyping)
            {
                return;
            }
            
            _timer += Time.deltaTime;

            if (_timer > _duration)
            {
                if (_index >= _data.Count)
                {
                    _isPlaying = false;
                    return;
                }
                
                if (_messageCoroutine != null)
                {
                    StopCoroutine(_messageCoroutine);
                }
                
                _message = _localeGameDataBase.GetString(_data[_index].LocaleId);
                _messageCoroutine = StartCoroutine(ShowMessage(_message));

                _timer = 0;
                _duration = _data[_index].Duration;
                _index++;
            }
        }
        
        
        private IEnumerator ShowMessage(string msg)
        {
            _isTyping = true;

            _desc.text = "";
            foreach (var c in msg)
            {
                _desc.text += c;

                if (c != ' ' && c != '\n' && c != ',')
                {
                    _soundManager.PlayOneShot(_dialogSoundEvent);
                }

                yield return new WaitForSeconds(_playSpeed);
            }
            
            _isTyping = false;
        }
        

        public bool Skip()
        {
            if (!_isPlaying)
            {
                return false;
            }

            if (_messageCoroutine != null)
            {
                StopCoroutine(_messageCoroutine);
            }
            _isTyping = false;
            
            if (_desc.text != _message)
            {
                _desc.text = _message;
            }
            else
            {
                _duration = 0;
            }

            return true;
        }
        
        public void Hide()
        {
            StopAllCoroutines();
            _isTyping = false;
            
            _descAnimation.Pause();
            _releaseAnimation.ReStart();
        }
        
    }
}
