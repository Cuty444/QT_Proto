using QT.Core;
using QT.Core.Data;
using UnityEngine;

namespace QT.Enemy
{
    [FSMState((int) Enemy.States.Rigid)]
    public class EnemyRigidState : FSMState<Enemy>
    {
        private float _rigidStartTime = 0f;

        private float _rigidTime;
        
        public EnemyRigidState(IFSMEntity owner) : base(owner)
        {
        }

        public override void InitializeState()
        {
            _rigidStartTime = Time.time;
            _rigidTime = SystemManager.Instance.GetSystem<GlobalDataSystem>().GlobalData.RigidTime;
        }

        public override void UpdateState()
        {
            if (_rigidStartTime + _rigidTime < Time.time)
            {
                _ownerEntity.RevertToPreviousState();
            }
        }
    }
}