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

        [field: SerializeField] public List<TweenSequence> Sequences { get; private set; } = new();
      
        
        private Sequence _sequence;
        public Sequence Sequence
        {
            get
            {
                if (_sequence == null)
                {
                    MakeSeqence();
                }

                return _sequence;
            }
        }
        
        private void Awake()
        {
            if (PlayOnAwake)
            {
                Sequence.Play();
            }
        }

        public void ReStart()
        {
            Sequence.Restart();
        }

        public void Rewind()
        {
            Sequence.Rewind();
        }

        private void MakeSeqence()
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
                            seq.Join(elements.Target.DOMove(elements.TweenTarget, elements.Duration)
                                .SetEase(elements.Ease));
                            break;
                        case TweenMode.DoRotate:
                            seq.Join(elements.Target.DORotate(elements.TweenTarget, elements.Duration)
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