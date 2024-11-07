namespace GameServerCore.Scripting.CSharp;

public class ChainMissileParameters
{
    public int[] MaximumHits { get; init; } = []; 
    public bool CanHitCaster { get; init; }
    public bool CanHitSameTarget { get; init; }
    public bool CanHitSameTargetConsecutively { get; init; }
    public bool CanHitEnemies { get; init; }
    public bool CanHitFriends { get; init; }
}

public interface ISpellScript
{
    ChainMissileParameters ChainMissileParameters { get; }
    //SpellScriptMetadata ScriptMetadata { get; }

    int[] AutoCooldownByLevel { get; }
    int[] AutoTargetDamageByLevel { get; }
    string[] SpellFXOverrideSkins { get; }
    string[] SpellVOOverrideSkins { get; }

    string BuffName { get; }
    string BuffTextureName { get; }
    string AutoAuraBuffName { get; }
    string AutoBuffActivateEvent { get; }
    string AutoBuffActivateEffect { get; }
    string AutoBuffActivateAttachBoneName { get; }
    string AutoBuffActivateAttachBoneName2 { get; }
    string AutoBuffActivateAttachBoneName3 { get; }
    string AutoBuffActivateAttachBoneName4 { get; }

    float CastTime { get; }
    float ChannelDuration { get; }
    float SetSpellDamageRatio { get; }

    int SpellDamageRatio { get; }
    int SpellToggleSlot { get; }
    int OnPreDamagePriority { get; }

    bool NotSingleTargetSpell { get; }
    bool DoesntBreakShields { get; }
    bool DoesntTriggerSpellCasts { get; }
    bool CastingBreaksStealth { get; }
    bool IsDamagingSpell { get; }
    bool NonDispellable { get; }
    bool TriggersSpellCasts { get; }
    bool PersistsThroughDeath { get; }
    bool DoOnPreDamageInExpirationOrder { get; }
    bool IsPetDurationBuff { get; }
    bool PermeatesThroughDeath { get; }
    bool IsDebugMode { get; }

    void TargetExecuteBuildingBlocks() { }
    void PreLoadBuildingBlocks() { }
    void CanCastBuildingBlocks() { }
    void AdjustCooldownBuildingBlocks() { }
    void AdjustCastInfoBuildingBlocks() { }
    void SelfExecuteBuildingBlocks() { }
    void CharOnLaunchAttackBuildingBlocks() { }
    void SpellOnMissileUpdateBuildingBlocks() { }
    void SpellOnMissileEndBuildingBlocks() { }

    void OnBuffActivateBuildingBlocks() { }
    void OnBuffDeactivateBuildingBlocks() { }
    void UpdateBuffsBuildingBlocks() { }

    void BuffOnAllowAddBuildingBlocks() { }

    void BuffOnLaunchMissileBuildingBlocks() { }
    void BuffOnMissileEndBuildingBlocks() { }

    void BuffOnDisconnectBuildingBlocks() { }
    void BuffOnReconnectBuildingBlocks() { }

    void BuffOnHitUnitBuildingBlocks() { }
    void BuffOnBeingHitBuildingBlocks() { }
    void BuffOnMissBuildingBlocks() { }
    void BuffBeingDodgedBuildingBlocks() { }
    void BuffOnLaunchAttackBuildingBlocks() { }

    void BuffOnSpellHitBuildingBlocks() { }
    void BuffOnBeingSpellHitBuildingBlocks() { }
    void BuffOnSpellCastBuildingBlocks() { }
    void BuffOnPreAttackBuildingBlocks() { }

    void BuffOnMoveEndBuildingBlocks() { }
    void BuffOnMoveSuccessBuildingBlocks() { }
    void BuffOnMoveFailureBuildingBlocks() { }
    void BuffOnCollisionBuildingBlocks() { }
    void BuffOnCollisionTerrainBuildingBlocks() { }

    void BuffOnDeathBuildingBlocks() { }
    void BuffOnZombieBuildingBlocks() { }
    void BuffOnKillBuildingBlocks() { }
    void BuffOnAssistBuildingBlocks() { }

    void BuffOnPreDamageBuildingBlocks() { }
    void BuffOnPreMitigationDamageBuildingBlocks() { }
    void BuffOnDealDamageBuildingBlocks() { }
    void BuffOnPreDealDamageBuildingBlocks() { }
    void BuffOnHealBuildingBlocks() { }

    void BuffOnLevelUpSpellBuildingBlocks() { }
    void BuffOnLevelUpBuildingBlocks() { }

    void BuffOnUpdateActionsBuildingBlocks() { }
    void BuffOnUpdateStatsBuildingBlocks() { }
    void BuffOnUpdateAmmoBuildingBlocks() { }

    void ChannelingStartBuildingBlocks() { }
    void ChannelingStopBuildingBlocks() { }
    void ChannelingSuccessStopBuildingBlocks() { }
    void ChannelingCancelStopBuildingBlocks() { }
    void ChannelingUpdateActionsBuildingBlocks() { }
    void ChannelingUpdateStatsBuildingBlocks() { }
}
