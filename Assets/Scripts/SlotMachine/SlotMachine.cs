using System;
using System.Collections;
using System.Collections.Generic;
using QT.Core;
using QT.Util;
using UnityEngine;
using UnityEngine.UI;
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
        [SerializeField] private int _priceGold;

        [Header("확률 테이블")] [SerializeField] private SlotPrizeData[] _slotPrizeData; 
        [Space]
        [SerializeField] private UIItemDesc _uiItemDesc;


        private PlayerManager _playerManager;
        private SlotDropPercentage _slotDropPercentage;
        private SpriteRenderer _sprite;

        private bool isAnimationStay = false;
        
        private void Awake()
        {
            _uiItemDesc.SetGoldCost(string.Format("X {0}", _priceGold));
            _uiItemDesc.Hide();
            _slotDropPercentage = new SlotDropPercentage(_slotPrizeData);
            _sprite = GetComponent<SpriteRenderer>();
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
                _playerManager.PlayerItemInteraction.AddListener(PlaySlotMachine);
                _uiItemDesc.Show();
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Player") ||
                other.gameObject.layer == LayerMask.NameToLayer("PlayerDodge"))
            {
                _playerManager.PlayerItemInteraction.RemoveListener(PlaySlotMachine);
                _uiItemDesc.Hide();
            }
        }

        private void PlaySlotMachine()
        {
            if (isAnimationStay)
            {
                return;
            }
            if (_playerManager.Gold < _priceGold)
            {
                _uiItemDesc.PlayFailButtonAnimation();
                return;
            }
            _playerManager.OnGoldValueChanged.Invoke(-_priceGold);
            isAnimationStay = true;
            _sprite.color = Color.yellow;
            StartCoroutine(UnityUtil.WaitForFunc(() =>
            {
                var data = _slotPrizeData[_slotDropPercentage.RandomSlotPrize()];
                switch (data.SlotPrizeType)
                {
                    case SlotPrizeType.gold:
                        Coin.SpawnDelayStay(data.DropCount, transform.position, Vector2.zero, 20,1f);
                        break;
                    case SlotPrizeType.hp:
                        _playerManager.Player.Heal(data.DropCount);
                        _playerManager.AddItemEvent.Invoke();
                        break;
                    case SlotPrizeType.item:
                        SystemManager.Instance.SoundManager.PlayOneShot(SystemManager.Instance.SoundManager.SoundData.Item_GetSFX);
                        var itemData = SystemManager.Instance.GetSystem<ItemPoolSystem>().GetItemsWithDropPercentage(data.DropCount,DropGameType.Select);
                        _playerManager.Player.AddItem(itemData[0]);
                        _playerManager.AddItemEvent.Invoke();
                        break;
                }

                _sprite.color = Color.white;
                isAnimationStay = false;
            },1f));
        }
    }
}
