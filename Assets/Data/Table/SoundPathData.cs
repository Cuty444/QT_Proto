using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

namespace QT.Sound
{
    public class SoundPathData : ScriptableObject
    {
        [field: SerializeField] public EventReference WalkSFX{ get; private set; }
        [field: SerializeField] public EventReference WalkFlowerSFX{ get; private set; }
        [field: SerializeField] public EventReference SwingSFX{ get; private set; }
        [field: SerializeField] public EventReference PlayerSwingHitSFX{ get; private set; }
        [field: SerializeField] public EventReference BallAttackSFX{ get; private set; }
        [field: SerializeField] public EventReference PlayerHitSFX{ get; private set; }
        [field: SerializeField] public EventReference ChargeSFX{ get; private set; }
        [field: SerializeField] public EventReference ChargeEndSFX{ get; private set; }
        [field: SerializeField] public EventReference PlayerThrowHitSFX{ get; private set; }
        [field: SerializeField] public EventReference PlayerDashSFX{ get; private set; }
        
        [field: SerializeField] public EventReference BallBounceSFX{ get; private set; }
        
        [field: SerializeField] public EventReference GameOverSFX{ get; private set; }
        
        [field: SerializeField] public EventReference MonsterStun{ get; private set; }
        [field: SerializeField] public EventReference MonsterFly{ get; private set; }
        
        [field: SerializeField] public EventReference Stage1BGM{ get; private set; }
        [field: SerializeField] public EventReference LoadingBGM{ get; private set; }
        [field: SerializeField] public EventReference BossStageBGM { get; private set; }
        
        [field: SerializeField] public EventReference ShopStageBGM { get; private set; }
        
        [field: SerializeField] public EventReference MainBGM { get; private set; }

        [field: SerializeField] public EventReference UITabSFX { get; private set; }
        [field: SerializeField] public EventReference UISkipSFX { get; private set; }
        [field: SerializeField] public EventReference UIMouseOverSFX { get; private set; }
        [field: SerializeField] public EventReference UIGameStartSFX { get; private set; }
        
        
        [field: SerializeField] public EventReference Monster_AwaySFX { get; private set; }
        [field: SerializeField] public EventReference Monster_AwayWallHitSFX { get; private set; }
        [field: SerializeField] public EventReference Monster_AwayMonsterHitSFX { get; private set; }
        
        
        [field: SerializeField] public EventReference Door_OpenSFX { get; private set; }
        [field: SerializeField] public EventReference Altar_AppearSFX { get; private set; }
        
        [field: SerializeField] public EventReference Item_GetSFX { get; private set; }
        [field: SerializeField] public EventReference Shop_BuySFX { get; private set; }
        [field: SerializeField] public EventReference Shop_BuyErrorSFX { get; private set; }
        [field: SerializeField] public EventReference Coin_GetSFX { get; private set; }
        
        [field: SerializeField] public EventReference Player_TeleportAttackSFX { get; private set; }
        [field: SerializeField] public EventReference Player_Walk_StairSFX { get; private set; }
        
        [field: SerializeField] public EventReference Monster_WaterDrop { get; private set; }
        
        
        
        [field: SerializeField] public string[] Bank{ get; private set; }
        
    }
}
