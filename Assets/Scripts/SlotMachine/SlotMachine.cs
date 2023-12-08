using System;
using System.Collections;
using System.Collections.Generic;
using QT.Core;
using QT.Util;
using UnityEngine;
using Random = UnityEngine.Random;

namespace QT
{
    [Serializable]
    public enum SlotPrizeType
    {
        gold,
        hp,
        item,
    }
    
    [Serializable]
    public class SlotPrizeData
    {
        public SlotPrizeType SlotPrizeType;
        [Range(0,100)]
        public float Percentage;

        public int DropCount;
    }

    public class SlotMachine : MonoBehaviour
    {
        private static readonly int Run = Animator.StringToHash("Run");
        private static readonly int Result = Animator.StringToHash("Result");
        private static readonly int Value = Animator.StringToHash("Value");

        #region SlotRandom

        public class SlotDropPercentage
        {
            private readonly List<float> _percentagesList = new List<float>();
            private readonly float _max;
            public SlotDropPercentage(SlotPrizeData[] data)
            {
                for (int i = 0; i < data.Length; i++)
                {
                    _percentagesList.Add(data[i].Percentage);
                    _max += data[i].Percentage;
                }
            }

            public int RandomSlotPrize()
            {
                float key = Random.Range(0, _max);
                float target = 0;

                for (int i = 0; i < _percentagesList.Count; ++i)
                {
                    target += _percentagesList[i];
                    if (key < target)
                    {
                        return i;
                    }
                }

                return 0;
            }
        }

        #endregion

        [Header("소모 골드 값")]
        [SerializeField] private int _startPrice;
        [SerializeField] private float _priceIncressPer = 1;

        [Header("확률 테이블")] [SerializeField] private SlotPrizeData[] _slotPrizeData; 
        [Space]
        [SerializeField] private UIItemDesc _uiItemDesc;

        [SerializeField] private GameObject _focusCamera;

        private Animator _animator;

        private PlayerManager _playerManager;
        private SlotDropPercentage _slotDropPercentage;

        private float _currentPrice;

        private bool _isMapClear = false;

        private void Awake()
        {
            _currentPrice = _startPrice;
            
            _animator = GetComponentInChildren<Animator>();
            _uiItemDesc.SetGoldCost($"X {Mathf.RoundToInt(_currentPrice)}");
            _uiItemDesc.Hide();
            _slotDropPercentage = new SlotDropPercentage(_slotPrizeData);
        }

        private void Start()
        {
            _playerManager = SystemManager.Instance.PlayerManager;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Player") ||
                other.gameObject.layer == LayerMask.NameToLayer("PlayerDodge"))
            {
                _playerManager.PlayerInteraction.AddListener(PlaySlotMachine);
                _uiItemDesc.Show();
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Player") ||
                other.gameObject.layer == LayerMask.NameToLayer("PlayerDodge"))
            {
                _playerManager.PlayerInteraction.RemoveListener(PlaySlotMachine);
                _uiItemDesc.Hide();
            }
        }

        public void MapClear()
        {
            _isMapClear = true;
        }

        private void PlaySlotMachine()
        {
            if (!_isMapClear)
                return;
            AnimatorStateInfo stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
            if (!stateInfo.IsName("Idle"))
            {
                return;
            }
            if (_playerManager.Gold < _currentPrice)
            {
                _uiItemDesc.PlayFailButtonAnimation();
                return;
            }
            
            _playerManager.OnGoldValueChanged.Invoke(-Mathf.RoundToInt(_currentPrice));
            
            _currentPrice *= _priceIncressPer;
            _uiItemDesc.SetGoldCost($"X {Mathf.RoundToInt(_currentPrice)}");
            
            SystemManager.Instance.SoundManager.PlayOneShot(SystemManager.Instance.SoundManager.SoundData.Roulette_Insert);
            SystemManager.Instance.SoundManager.PlayOneShot(SystemManager.Instance.SoundManager.SoundData.Roulette_Start);
            
            _animator.SetTrigger(Run);
            _focusCamera.SetActive(true);
            _playerManager.Player.PlayerInputPause(true);
            StartCoroutine(UnityUtil.WaitForFunc(() =>
            {
                var data = _slotPrizeData[_slotDropPercentage.RandomSlotPrize()];
                _animator.SetTrigger(Result);
                switch (data.SlotPrizeType)
                {
                    case SlotPrizeType.gold:
                        Coin.SpawnDelayStay(data.DropCount, transform.position, Vector2.zero, 20,1f);
                        if (data.DropCount < 50)
                        {
                            _animator.SetFloat(Value,0.25f);
                        }
                        else
                        {
                            SystemManager.Instance.SoundManager.PlayOneShot(SystemManager.Instance.SoundManager.SoundData.Roulette_Jackpot);
                            _animator.SetFloat(Value,1f);
                        }
                        break;
                    case SlotPrizeType.hp:
                        _playerManager.Player.Heal(data.DropCount);
                        _playerManager.AddItemEvent.Invoke();
                        _animator.SetFloat(Value,0.5f);
                        break;
                    case SlotPrizeType.item:
                        SystemManager.Instance.SoundManager.PlayOneShot(SystemManager.Instance.SoundManager.SoundData.Item_GetSFX);
                        var itemData = SystemManager.Instance.GetSystem<ItemPoolSystem>().GetItemsWithDropPercentage(data.DropCount,DropGameType.Select);
                        _playerManager.Player.AddItem(itemData[0]);
                        _animator.SetFloat(Value,0.75f);
                        _playerManager.AddItemEvent.Invoke();
                        SystemManager.Instance.SoundManager.PlayOneShot(SystemManager.Instance.SoundManager.SoundData.Roulette_Reward);
                        break;
                }

                StartCoroutine(UnityUtil.WaitForFunc(() =>
                {
                    _focusCamera.SetActive(false);
                    _playerManager.Player.PlayerInputPause(false);
                }, 0.2f));
            },1f));
        }
    }
}
