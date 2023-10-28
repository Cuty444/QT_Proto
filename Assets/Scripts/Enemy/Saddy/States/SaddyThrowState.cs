using System.Collections;
using System.Collections.Generic;
using QT.Core;
using QT.Sound;
using UnityEngine;

namespace QT.InGame
{
    [FSMState((int) Saddy.States.Throw)]
    public class SaddyThrowState : FSMState<Saddy>
    {
        private readonly int AttackAnimHash = Animator.StringToHash("Swing");

        private List<EnemyAtkGameData> _atkList;
        private SaddyData _data;
        
        private SoundManager _soundManager;

        public SaddyThrowState(IFSMEntity owner) : base(owner)
        {
            _atkList = SystemManager.Instance.DataManager.GetDataBase<EnemyAtkGameDataBase>().GetData(_ownerEntity.SaddyData.ThrowAtkId);
            _data = _ownerEntity.SaddyData;
        }

        public override void InitializeState()
        {
            _soundManager = SystemManager.Instance.SoundManager;

            _ownerEntity.Rigidbody.velocity = Vector2.zero;
            _ownerEntity.Rigidbody.constraints = RigidbodyConstraints2D.FreezeAll;
            
            _ownerEntity.StartCoroutine(AttackSequence());
        }

        public override void UpdateState()
        {
            Vector2 dir = (SystemManager.Instance.PlayerManager.Player.transform.position - _ownerEntity.transform.position);
            _ownerEntity.SetDir(dir,4);
        }

        public override void ClearState()
        {
            _ownerEntity.Rigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
        }

        private IEnumerator AttackSequence()
        {
            foreach (var data in _atkList)
            {  
                _ownerEntity.Animator.SetTrigger(AttackAnimHash);
                yield return new WaitForSeconds(data.BeforeDelay);

                _soundManager.PlayOneShot(_soundManager.SoundData.Boss_Throw, _ownerEntity.transform.position);

                _ownerEntity.Shooter.Shoot(data.ShootDataId, data.AimType, ProjectileOwner.Boss);
                
                yield return new WaitForSeconds(data.AfterDelay);
            }

            _ownerEntity.ChangeState(Dullahan.States.Normal);
        }
    }
}