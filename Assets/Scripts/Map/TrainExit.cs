using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using QT.Core;
using QT.Core.Map;
using QT.Sound;
using UnityEngine;

namespace QT.Map
{
    public class TrainExit : MonoBehaviour
    {
        [SerializeField] private TweenAnimator _trainDoorOpenAnimation;
        [SerializeField] private TweenAnimator _screenDoorOpenAnimation;
        
        [SerializeField] private TweenAnimator _trainEnterAnimation;
        [SerializeField] private TweenAnimator _trainExitAnimation;

        [SerializeField] private GameObject _collider;
        [SerializeField] private GameObject _camera;

        private SoundManager _soundManager;
        
        private void OnEnable()
        {
            _soundManager = SystemManager.Instance.SoundManager;
            
            StopAllCoroutines();
            StartCoroutine(EnterSequence());
            
            _collider.SetActive(true);
            _camera.SetActive(false);
        }
        
        private void OnTriggerEnter2D(Collider2D col)
        {
            SystemManager.Instance.PlayerManager.Player.Pause(true);
            
            StopAllCoroutines();
            StartCoroutine(ExitSequence());
        }
        
        private IEnumerator EnterSequence()
        {
            _trainDoorOpenAnimation.Reset();
            _screenDoorOpenAnimation.Reset();
            _trainEnterAnimation.ReStart();
            
            _soundManager.PlaySFX(_soundManager.SoundData.Subway, Vector3.zero, _trainEnterAnimation.transform);
            yield return new WaitForSeconds(_trainEnterAnimation.SequenceLength);
            _soundManager.StopSFX(_soundManager.SoundData.Subway);

            _soundManager.PlayOneShot(_soundManager.SoundData.Subway_Production, _trainEnterAnimation.transform.position);
            
            _trainDoorOpenAnimation.ReStart();
            _screenDoorOpenAnimation.ReStart();
            
            yield return new WaitForSeconds(0.5f);
            _collider.SetActive(false);
        }

        private IEnumerator ExitSequence()
        {
            _camera.SetActive(true);
            
            _trainDoorOpenAnimation.PlayBackwards();
            _screenDoorOpenAnimation.PlayBackwards();
            
            _soundManager.PlayOneShot(_soundManager.SoundData.Subway_End, _trainEnterAnimation.transform.position);
            yield return new WaitForSeconds(_trainDoorOpenAnimation.SequenceLength);
            
            _trainExitAnimation.ReStart();
            
            SystemManager.Instance.PlayerManager.Player.transform.position = new Vector2(-99999,-99999);;
            
            yield return new WaitForSeconds(_trainExitAnimation.SequenceLength);

            SystemManager.Instance.GetSystem<DungeonMapSystem>().NextFloor();
        }
    }
}
