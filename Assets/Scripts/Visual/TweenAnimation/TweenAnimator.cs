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

        [field: SerializeField] public List<TweenSequence> Sequences { get; private set; } = new();
      
        private Sequence _sequence;
        
        private void Awake()
        {
            if (PlayOnAwake)
            {
                ReStart();
            }
        }

        private void OnDestroy()
        {
            _sequence?.Kill();
        }

        public void ReStart()
        {
            if (_sequence == null)
            {
                BakeSeqence();
            }
            
            _sequence.Restart();
        }

        public void PlayBackwards()
        {
            if (_sequence == null)
            {
                BakeSeqence();
                _sequence.Rewind();
            }
            
            _sequence.PlayBackwards();
        }

        public void Reset()
        {
            if (_sequence == null)
            {
                BakeSeqence();
                _sequence.Rewind();
            }
            
            _sequence.Restart();
            _sequence.Pause();
        }

        public void Pause()
        {
            if (_sequence == null)
            {
                BakeSeqence();
            }
            
            _sequence.Pause();
        }
        
        public void BakeSeqence()
        {
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