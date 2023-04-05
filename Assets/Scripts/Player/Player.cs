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
        public Rigidbody2D Rigidbody { get; private set; }

        private void Awake()
        {
            SetUp(States.Idle);
            SetGlobalState(new PlayerGlobalState(this));
            Rigidbody = GetComponent<Rigidbody2D>();
            MeshFilter = GetComponent<MeshFilter>();
            MeshRenderer = GetComponent<MeshRenderer>();
        }

        private void Start()
        {
            DataLoad();
            MeshFilter.mesh = SwingAreaCreateMesh(AtkRadius, AtkCentralAngle, 32);
            MeshRenderer.material = new Material(Shader.Find("Sprites/Default"));
            MeshRenderer.material.color = new Color(0f, 0f, 1f, 0.2f);
            MeshRenderer.enabled = false;
        }

        private void DataLoad()
        {
            GlobalDataSystem globalDataSystem = SystemManager.Instance.GetSystem<GlobalDataSystem>();
            CharacterAtkTable characterAtkTable = globalDataSystem.CharacterAtkTable;
            CharacterTable characterTable = globalDataSystem.CharacterTable;
            AtkRadius = characterAtkTable.ATKRad;
            AtkCentralAngle = characterAtkTable.AtkCentralAngle;
            AtkShootSpeed = characterAtkTable.AtkShootSpd;
            AtkCoolTime = characterAtkTable.AtkCooldown;
            ChargingMaxTimes = characterAtkTable.ChargingMaxTimes;
            SwingRigidDmg = characterAtkTable.SwingRigidDmg;
            ChargeBounceValues = characterAtkTable.ChargeBounceValue;
            
            BallStackMax = characterTable.BallStackMax;
            MercyInvincibleTime = characterTable.MercyInvincibleTime;
            DodgeInvincibleTime = characterTable.DodgeInvincibleTime;

            HPMax = characterTable.HPMax;
            HP = HPMax;
        }
    }
}
