using System;
using System.Collections;
using System.Collections.Generic;
using QT.Core;
using QT.Core.Map;
using QT.UI;
using QT.Util;
using UnityEngine;
using UnityEngine.Serialization;

namespace QT
{
    public class NpcCasterInteraction : MonoBehaviour, IHitAble
    {
        public int InstanceId => gameObject.GetInstanceID();
        public Vector2 Position => transform.position;
        [field: SerializeField] public float ColliderRad { get; private set; }
        public bool IsClearTarget => false;
        public bool IsDead => false;

        public int TutotialDualogueId;
        public List<int> DefaultDialogueIds;
        public List<int> BossDialogueIds;
        public List<int> ClearDialogueIds;

        private int _dialogueIndex;
        
        
        [SerializeField] private TextBalloon _textBalloon;
        [SerializeField] private GameObject _casterObject;

        private PlayerManager _playerManager;
        private Animator _animator = null;
        
        private static readonly int Talk = Animator.StringToHash("Talk");
        private const string SummonPrefabPath = "Effect/Prefabs/FX_NPC_Ppyong_Dust.prefab";


        private Progress _currentProgress;

        private void Start()
        {
            _animator = GetComponentInChildren<Animator>();
            _playerManager = SystemManager.Instance.PlayerManager;
            
            SystemManager.Instance.ResourceManager.EmitParticle(SummonPrefabPath, (Vector2)transform.position + Vector2.up);
            
            _casterObject.gameObject.SetActive(false);
            StartCoroutine(Util.UnityUtil.WaitForFunc(() =>
            {
                _casterObject.gameObject.SetActive(true);
            }, 0.3f));
            
            SystemManager.Instance.SoundManager.PlayOneShot(SystemManager.Instance.SoundManager.SoundData.Npc_Bat_Appear);
            //_textBalloon.Hide();

            Init();
        }

        private void Init()
        {
            _currentProgress = (Progress) PlayerPrefs.GetInt(Constant.ProgressDataKey, 0);
            
            DefaultDialogueIds.Shuffle();
            ClearDialogueIds.Shuffle();

            if (_currentProgress == Progress.Clear)
            {
                DefaultDialogueIds = ClearDialogueIds;
                _dialogueIndex = 0;
            }
            else
            {
                _dialogueIndex = -1;
            }
        }
        
        private int PickDialogId()
        {
            if (_currentProgress == Progress.None)
            {
                return TutotialDualogueId;
            }
            
            if (_dialogueIndex < 0)
            {
                _dialogueIndex = 0;
                switch (_currentProgress)
                {
                    case Progress.TutorialClear:
                        return BossDialogueIds[0];
                    case Progress.JelloClear:
                        return BossDialogueIds[1];
                    case Progress.SaddyClear:
                        return BossDialogueIds[2];
                }
            }
            
            _dialogueIndex++;
            
            if(_dialogueIndex < DefaultDialogueIds.Count)
            {
                return DefaultDialogueIds[_dialogueIndex];
            }
            
            DefaultDialogueIds.Shuffle();
            _dialogueIndex = 0;

            return DefaultDialogueIds[_dialogueIndex];
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Player") ||
                other.gameObject.layer == LayerMask.NameToLayer("PlayerDodge"))
            {
                _playerManager.PlayerInteraction.AddListener(PlayerInteraction);
                _animator.SetBool(Talk,true);
                SystemManager.Instance.SoundManager.PlayOneShot(SystemManager.Instance.SoundManager.SoundData.Npc_Bat_Dialog);
                _textBalloon.Show(PickDialogId());
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Player") ||
                other.gameObject.layer == LayerMask.NameToLayer("PlayerDodge"))
            {
                _playerManager.PlayerInteraction.RemoveListener(PlayerInteraction);
                _animator.SetBool(Talk,false);
                _textBalloon.Hide();
            }
        }

        private void PlayerInteraction()
        {
            if (!_textBalloon.Skip())
            {
                _textBalloon.Show(PickDialogId());
            }
        }
        
        
        public void Hit(Vector2 dir, float power,AttackType attackType)
        {
        }
    }
}
