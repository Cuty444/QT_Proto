using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;


namespace QT
{
    public enum TweenMode
    {
        DoMove,
        DoRotate,
        DoScale
    }

    [Serializable]
    public class TweenSequenceElement
    {
        public TweenMode Mode;
        public Transform Target;
        public Ease Ease = Ease.Linear;
        public Vector3 TweenTarget;
        public float Duration = 1;
    }

    [Serializable]
    public class TweenSequence
    {
        public List<TweenSequenceElement> Elements;
    }

    public class TweenAnimator : MonoBehaviour
    {
        public bool PlayOnAwake;
        public bool IgnoreTimeScale;
        public bool IsLoop;

        public float SequenceLength { get; private set; }

        public TweenAnimator ChainAnimator;

        [field: SerializeField] public List<TweenSequence> Sequences { get; private set; } = new();
      
        private Sequence _sequence;
        
        private void OnEnable()
        {
            if (PlayOnAwake)
            {
                ReStart();
            }
        }

        // private void OnDisable()
        // {
        //     _sequence?.Pause();
        // }

        private void OnDestroy()
        {
            _sequence?.SetAutoKill(true);
        }

        public void GoToPlay(float time)
        {
            if (_sequence == null)
            {
                BakeSequence();
            }
            
            _sequence.Goto(time, true);
            ChainAnimator?.GoToPlay(time);
        }
        
        public void ReStart()
        {
            if (_sequence == null)
            {
                BakeSequence();
            }
            
            _sequence.Restart();
            ChainAnimator?.ReStart();
        }

        public void PlayBackwards()
        {
            if (_sequence == null)
            {
                BakeSequence();
                _sequence.Rewind();
            }
            
            _sequence.PlayBackwards();
            ChainAnimator?.PlayBackwards();
        }

        public void Reset()
        {
            if (_sequence == null)
            {
                BakeSequence();
                _sequence.Rewind();
            }
            
            _sequence.Restart();
            _sequence.Pause();
            
            ChainAnimator?.Reset();
        }

        public void Pause()
        {
            if (_sequence == null)
            {
                BakeSequence();
            }
            
            _sequence.Pause();
            
            ChainAnimator?.Pause();
        }

        public void BakeSequence(bool bakeChain = false)
        {
            if (bakeChain)
            {
                ChainAnimator?.BakeSequence(bakeChain);
            }
            
            _sequence = DOTween.Sequence().SetUpdate(IgnoreTimeScale).SetRecyclable(true).SetAutoKill(false).Pause();
            SequenceLength = 0;

            if (IsLoop)
            {
                _sequence.SetLoops(-1);
            }
            
            foreach (var sequence in Sequences)
            {
                var seq = DOTween.Sequence();
                float duration = 0;
                
                foreach (var elements in sequence.Elements)
                {
                    switch (elements.Mode)
                    {
                        case TweenMode.DoMove:
                            if (elements.Target is RectTransform rectTransform)
                            {  
                                seq.Join(rectTransform.DOAnchorPos(elements.TweenTarget, elements.Duration)
                                    .SetEase(elements.Ease));
                            }
                            else
                            {
                                seq.Join(elements.Target.DOLocalMove(elements.TweenTarget, elements.Duration)
                                    .SetEase(elements.Ease));
                            }
                            break;
                        
                        case TweenMode.DoRotate:
                            seq.Join(elements.Target.DOLocalRotate(elements.TweenTarget, elements.Duration)
                                .SetEase(elements.Ease));
                            break;
                        
                        case TweenMode.DoScale:
                            seq.Join(elements.Target.DOScale(elements.TweenTarget, elements.Duration)
                                .SetEase(elements.Ease));
                            break;
                    }

                    if (elements.Duration > duration)
                    {
                        duration = elements.Duration;
                    }
                }

                _sequence.Append(seq);
                SequenceLength += duration;
            }
        }
        
    }
}