using System;
using QT.Core;
using QT.Core.Data;
using UnityEngine;
using QT.Data;

namespace QT.Player
{
    public partial class Player : FSMPlayer<Player>, IFSMEntity
    {
        public enum States : int
        {
            Global,
            Idle,
            Move,
            Swing,
            Dead,
        }

        [SerializeField] private int _characterID = 100;
        [SerializeField] private int _characterAtkID = 200;
        public Rigidbody2D Rigidbody { get; private set; }

        public CharacterGameData Data { get; private set; }
        public CharacterAtkGameData AtkData { get; private set; }
        private void Awake()
        {
            Data = SystemManager.Instance.DataManager.GetDataBase<CharacterGameDataBase>().GetData(_characterID);
            AtkData = SystemManager.Instance.DataManager.GetDataBase<CharacterAtkGameDataBase>().GetData(_characterAtkID);
            SetUp(States.Idle);
            SetGlobalState(new PlayerGlobalState(this));
            Rigidbody = GetComponent<Rigidbody2D>();
            MeshFilter = GetComponent<MeshFilter>();
            MeshRenderer = GetComponent<MeshRenderer>();
            SetUpStats();
        }

        private void Start()
        {
            //StatLoad();
            //SwingAreaCreate();
        }
        
    }
}
