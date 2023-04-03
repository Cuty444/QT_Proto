using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using QT.Data;

namespace QT.Core.Player
{
    public class PlayerSystem : SystemBase
    {
        [SerializeField] private GameObject _playerObject;
        private ChargeAtkPierce _chargeAtkPierce;
        public ChargeAtkPierce ChargeAtkPierce { get => _chargeAtkPierce; set => _chargeAtkPierce = value; }

        private UnityEvent<float> _chargeAtkShootEvent = new UnityEvent<float>();
        public UnityEvent<float> ChargeAtkShootEvent => _chargeAtkShootEvent;

        private UnityEvent<int> _batSwingRigidHitEvent = new UnityEvent<int>();
        public UnityEvent<int> BatSwingRigidHitEvent => _batSwingRigidHitEvent;

        private UnityEvent<int> _chargeBounceValueEvent = new UnityEvent<int>();
        public UnityEvent<int> ChargeBounceValueEvent => _chargeBounceValueEvent;

        private UnityEvent _batSwingEndEvent = new UnityEvent();
        public UnityEvent BatSwingEndEvent => _batSwingEndEvent;

        private UnityEvent<GameObject> _playerCreateEvent = new UnityEvent<GameObject>();
        public UnityEvent<GameObject> PlayerCreateEvent => _playerCreateEvent;

        private UnityEvent<bool> _dodgeEvent = new UnityEvent<bool>();
        public UnityEvent<bool> DodgeEvent => _dodgeEvent;

        private UnityEvent _playerCollisionEnemyEvent = new UnityEvent();
        public UnityEvent PlayerCollisionEnemyEvent => _playerCollisionEnemyEvent;

        private UnityEvent<GameObject> _playerBallAddedEvent = new UnityEvent<GameObject>();

        public UnityEvent<GameObject> PlayerBallAddedEvent => _playerBallAddedEvent;

        private UnityEvent<float> _playerCurrentChargingTimeEvent = new UnityEvent<float>();
        public UnityEvent<float> PlayerCurrentChargingTimeEvent => _playerCurrentChargingTimeEvent;

        private UnityEvent<bool> _batSwingTimeScaleEvent = new UnityEvent<bool>();
        public UnityEvent<bool> BatSwingTimeScaleEvent => _batSwingTimeScaleEvent;

        private UnityEvent _batSwingBallHitEvent = new UnityEvent();
        public UnityEvent BatSwingBallHitEvent => _batSwingBallHitEvent;

        private Transform _playerTransform;
        public Transform PlayerTransform => _playerTransform;
        public void OnPlayerCreate() // 추후 로그라이크맵 절차 생성 SystemManager에서 관리하도록 코드 위치 변경이 필요함
        {
            GameObject playerObject = Instantiate(_playerObject, Vector3.zero, Quaternion.identity);
            _playerCreateEvent.Invoke(playerObject);
            _playerTransform = playerObject.transform;
        }
    }
}
