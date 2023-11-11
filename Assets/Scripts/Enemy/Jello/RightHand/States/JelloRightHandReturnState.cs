using QT.Core;
using QT.InGame;
using UnityEngine;

namespace QT.InGame
{
    [FSMState((int)JelloRightHand.States.Return)]
    public class JelloRightHandReturnState : FSMState<JelloRightHand>
    {
        private Vector2 _targetPos;

        private float _targetTime;
        private float _timer;

        private Vector2 _startPos;
        
        public JelloRightHandReturnState(IFSMEntity owner) : base(owner)
        {
        }

        public void InitializeState(Vector2 targetPos, float duration)
        {
            _ownerEntity.SetPhysics(false);
            
            _startPos = _ownerEntity.transform.position;
            _targetPos = targetPos;

            _targetTime = duration;
            _timer = 0;
        }

        public override void UpdateState()
        {
            _timer += Time.deltaTime;
            
            var progress = 1 - _timer / _targetTime;
            progress = 1 - progress * progress;

            _ownerEntity.transform.position = Vector2.Lerp(_startPos, _targetPos, progress);

            if (_timer >= _targetTime)
            {
                _ownerEntity.ChangeState(JelloRightHand.States.Normal);
            }
        }
        
        public override void ClearState()
        {
            _ownerEntity.SetPhysics(true);
        }
    }
}
