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
        public RectTransform Target;
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

        [field: SerializeField] public List<TweenSequence> Sequences { get; private set; } = new();
      
        
        private Sequence _sequence;
        
        private void Awake()
        {
            if (PlayOnAwake)
            {
                ReStart();
            }
        }

        public void ReStart()
        {
            if (_sequence == null)
            {
                BakeSeqence();
            }
            
            _sequence.Restart();
        }

        public void Rewind()
        {
            if (_sequence == null)
            {
                BakeSeqence();
            }
            
            _sequence.Rewind();
        }

        public void BakeSeqence()
        {
            _sequence = DOTween.Sequence().SetUpdate(IgnoreTimeScale).SetRecyclable(true).SetAutoKill(false).Pause();

            foreach (var sequence in Sequences)
            {
                var seq = DOTween.Sequence();
                foreach (var elements in sequence.Elements)
                {
                    switch (elements.Mode)
                    {
                        case TweenMode.DoMove:
                            seq.Join(elements.Target.DOAnchorPos(elements.TweenTarget, elements.Duration)
                                .SetEase(elements.Ease));
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
                }
                _sequence.Append(seq);
            }
        }
    }
}