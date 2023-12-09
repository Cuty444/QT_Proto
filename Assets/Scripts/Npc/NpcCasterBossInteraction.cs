using System;
using System.Collections;
using System.Collections.Generic;
using QT.Core;
using QT.Core.Map;
using QT.UI;
using UnityEngine;
using UnityEngine.Serialization;

namespace QT
{
    public class NpcCasterBossInteraction : MonoBehaviour, IHitAble
    {
        public int InstanceId => gameObject.GetInstanceID();
        public Vector2 Position => transform.position;
        [field: SerializeField] public float ColliderRad { get; private set; }
        public bool IsClearTarget => false;
        public bool IsDead => false;
        
        public int DialogueId;
        
        [SerializeField] private TextBalloon _textBalloon;
        [SerializeField] private GameObject _casterObject;

        private PlayerManager _playerManager;
        private DungeonMapSystem _dungeonMapSystem;
        private Animator _animator = null;
        
        private static readonly int Talk = Animator.StringToHash("Talk");
        private const string SummonPrefabPath = "Effect/Prefabs/FX_NPC_Ppyong_Dust.prefab";


        private void Start()
        {
            _animator = GetComponentInChildren<Animator>();
            _playerManager = SystemManager.Instance.PlayerManager;
            _dungeonMapSystem = SystemManager.Instance.GetSystem<DungeonMapSystem>();
            
            SystemManager.Instance.ResourceManager.EmitParticle(SummonPrefabPath, (Vector2)transform.position + Vector2.up);
            
            _casterObject.gameObject.SetActive(false);
            StartCoroutine(Util.UnityUtil.WaitForFunc(() =>
            {
                _casterObject.gameObject.SetActive(true);
            }, 0.3f));
            
            SystemManager.Instance.SoundManager.PlayOneShot(SystemManager.Instance.SoundManager.SoundData.Npc_Bat_Appear);
            //_textBalloon.Hide();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Player") ||
                other.gameObject.layer == LayerMask.NameToLayer("PlayerDodge"))
            {
                _playerManager.PlayerInteraction.AddListener(PlayerInteraction);
                _animator.SetBool(Talk,true);
                SystemManager.Instance.SoundManager.PlayOneShot(SystemManager.Instance.SoundManager.SoundData.Npc_Bat_Dialog);
                _textBalloon.Show(DialogueId);
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
                MoveStairMap();
            }
        }
        
        private async void MoveStairMap()
        {
            _dungeonMapSystem.EnterStairMap( _playerManager.Player);
            
            var minimapCanvas = await SystemManager.Instance.UIManager.Get<MinimapCanvasModel>();
            minimapCanvas.SetMiniMap(_dungeonMapSystem.DungeonMapData);
            minimapCanvas.ChangeCenter(_dungeonMapSystem.DungeonMapData.BossRoomPosition);
            
            var phoneCanvas = await SystemManager.Instance.UIManager.Get<PhoneCanvasModel>();
            phoneCanvas.SetMiniMap(_dungeonMapSystem.DungeonMapData);
        }
        
        public void Hit(Vector2 dir, float power,AttackType attackType)
        {
        }
    }
}
