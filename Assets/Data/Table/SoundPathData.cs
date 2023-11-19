using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using UnityEngine;
using EventReference = FMODUnity.EventReference;

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
        
        [field: SerializeField] public EventReference Player_Heal { get; private set; }

        [field: SerializeField] public EventReference ActiveOneKiOkStartSFX{ get; private set; }
        [field: SerializeField] public EventReference ActiveOneKiOkThrowSFX{ get; private set; }
        
        [field: SerializeField] public EventReference ActiveDashStartSFX{ get; private set; }
        [field: SerializeField] public EventReference ActiveDashingSFX{ get; private set; }
        [field: SerializeField] public EventReference ActiveDashExplosionSFX{ get; private set; }
        [field: SerializeField] public EventReference ActiveDashAttackSFX{ get; private set; }
        
        [field: SerializeField] public EventReference ActiveTimeStopSFX{ get; private set; }
        [field: SerializeField] public EventReference ActiveTimeStopPauseSFX{ get; private set; }
        
        [field: SerializeField] public EventReference WarpMapSFX{ get; private set; }

        [field: SerializeField] public EventReference BallBounceSFX{ get; private set; }
        
        [field: SerializeField] public EventReference GameOverSFX{ get; private set; }
        
        [field: SerializeField] public EventReference MonsterStun{ get; private set; }
        [field: SerializeField] public EventReference MonsterFly{ get; private set; }
        
        [field: SerializeField] public EventReference Stage1BGM{ get; private set; }
        [field: SerializeField] public EventReference LoadingBGM{ get; private set; }
        [field: SerializeField] public EventReference BossStageBGM { get; private set; }
        [field: SerializeField] public EventReference JelloStageBGM { get; private set; }
        [field: SerializeField] public EventReference SaddyStageBGM { get; private set; }
        
        [field: SerializeField] public EventReference ShopStageBGM { get; private set; }
        
        
        [field: SerializeField] public EventReference ClearBGM{ get; private set; }
        [field: SerializeField] public EventReference GameOverBGM{ get; private set; }
        
        [field: SerializeField] public EventReference MainBGM { get; private set; }

        [field: SerializeField] public EventReference UITabSFX { get; private set; }
        [field: SerializeField] public EventReference UISkipSFX { get; private set; }
        [field: SerializeField] public EventReference UIMouseOverSFX { get; private set; }
        [field: SerializeField] public EventReference UIGameStartSFX { get; private set; }
        
        
        [field: SerializeField] public EventReference Monster_Spawn { get; private set; }
        [field: SerializeField] public EventReference Monster_AwaySFX { get; private set; }
        [field: SerializeField] public EventReference Monster_AwayWallHitSFX { get; private set; }
        [field: SerializeField] public EventReference Monster_AwayMonsterHitSFX { get; private set; }
        
        [field: SerializeField] public EventReference Slime_WalkSFX { get; private set; }
        [field: SerializeField] public EventReference Slime_DeadSFX { get; private set; }
        
        [field: SerializeField] public EventReference Catcher_WalkSFX { get; private set; }
        [field: SerializeField] public EventReference Catcher_DeadSFX { get; private set; }
        
        [field: SerializeField] public EventReference Bat_WalkSFX { get; private set; }
        [field: SerializeField] public EventReference Bat_DeadSFX { get; private set; }
        
        [field: SerializeField] public EventReference Ghost_WalkSFX { get; private set; }
        [field: SerializeField] public EventReference Ghost_DeadSFX { get; private set; }
        
        [field: SerializeField] public EventReference Telekinesisz_WalkSFX { get; private set; }
        [field: SerializeField] public EventReference Telekinesisz_DeadSFX { get; private set; }
        [field: SerializeField] public EventReference Door_OpenSFX { get; private set; }
        [field: SerializeField] public EventReference Altar_AppearSFX { get; private set; }
        
        [field: SerializeField] public EventReference Item_GetSFX { get; private set; }
        [field: SerializeField] public EventReference Shop_BuySFX { get; private set; }
        [field: SerializeField] public EventReference Shop_BuyErrorSFX { get; private set; }
        [field: SerializeField] public EventReference Coin_GetSFX { get; private set; }
        
        [field: SerializeField] public EventReference Player_TeleportAttackSFX { get; private set; }
        [field: SerializeField] public EventReference Subway { get; private set; }
        [field: SerializeField] public EventReference Subway_Production { get; private set; }
        [field: SerializeField] public EventReference Subway_End { get; private set; }
        
        [field: SerializeField] public EventReference Monster_WaterDrop { get; private set; }
        
        
        [field: SerializeField] public EventReference Boss_Walk { get; private set; }
        
        [field: SerializeField] public EventReference Boss_BatAttack { get; private set; }
        [field: SerializeField] public EventReference Boss_Throw { get; private set; }
        [field: SerializeField] public EventReference Boss_Roar { get; private set; }
        
        [field: SerializeField] public EventReference Boss_RushReady { get; private set; }
        [field: SerializeField] public EventReference Boss_Rush { get; private set; }
        [field: SerializeField] public EventReference Boss_Rush_Crash { get; private set; }
        
        
        [field: SerializeField] public EventReference Boss_Motorcycle_Start { get; private set; }
        [field: SerializeField] public EventReference Boss_Motorcycle_Ing { get; private set; }
        [field: SerializeField] public EventReference Boss_Motorcycle_End { get; private set; }

        [field: SerializeField] public EventReference Boss_JumpReady { get; private set; }
        [field: SerializeField] public EventReference Boss_Jump { get; private set; }
        [field: SerializeField] public EventReference Boss_Landing { get; private set; }
        
        [field: SerializeField] public EventReference Boss_Dead { get; private set; }
        
        
        [field: SerializeField] public EventReference Light_TurnOn { get; private set; }
        [field: SerializeField] public EventReference Doctor_Heal_Mark { get; private set; }
        
        [field: SerializeField] public EventReference Roulette_Insert { get; private set; }
        [field: SerializeField] public EventReference Roulette_Start { get; private set; }
        [field: SerializeField] public EventReference Roulette_Reward { get; private set; }
        [field: SerializeField] public EventReference Roulette_Jackpot { get; private set; }
        
        
        [field: SerializeField] public EventReference Store_Reroll { get; private set; }
        [field: SerializeField] public EventReference Npc_Bat_Dialog { get; private set; }
        [field: SerializeField] public EventReference Npc_Bat_Appear { get; private set; }

        [field: SerializeField] public string[] Bank{ get; private set; }
        
    }
}
