using QT.Core;
using QT.InGame;
using UnityEngine;

namespace QT.InGame
{
    [FSMState((int)JelloLeftHand.States.Return)]
    public class JelloLeftHandReturnState : FSMState<JelloLeftHand>
    {
        private Vector2 _targetPos;
        private Transform _transform;
        private Transform _ballObject;

        private float _targetTime;
        private float _timer;

        private Vector2 _startPos;
        
        public JelloLeftHandReturnState(IFSMEntity owner) : base(owner)
        {
            _transform = _ownerEntity.transform;
            _ballObject = _ownerEntity.BallObject;
        }

        public void InitializeState(Vector2 targetPos, float duration)
        {
            _ownerEntity.SetPhysics(false);
            _ownerEntity.HpIndicator.gameObject.SetActive(false);
            
            _startPos = _transform.position;
            _targetPos = targetPos;

            _targetTime = duration;
            _timer = 0;
        }

        public override void UpdateState()
        {
            _timer += Time.deltaTime;
            
            var progress = _timer / _targetTime;
            
            var inQuad = progress * progress;
            var outQuad = 1 - (1 - progress) * (1 - progress);

            _transform.position = Vector2.Lerp(_startPos, _targetPos, outQuad);
            _ballObject.localPosition = new Vector2(0, Mathf.Sin(Mathf.PI * outQuad));
            _ballObject.localScale = Vector2.one * (1 - inQuad);

            if (_timer >= _targetTime)
            {
                _ownerEntity.ChangeState(JelloLeftHand.States.Normal);
            }
        }
        
        public override void ClearState()
        {
            _ownerEntity.SetPhysics(true);
            
            _ballObject.localPosition = Vector3.zero;
            _ballObject.localScale = Vector2.one;
        }
    }
}
