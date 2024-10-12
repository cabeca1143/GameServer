using GameServerCore.Content;
using GameServerLib.Content.GameVariables;
using LeagueSandbox.GameServer.Content;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServerLib.Content;

struct CVarFloat
{
    internal string Name;
    internal string Description;
    internal float Value;
}

internal class CharacterData
{
    internal string CharacterName;
    internal uint CharacterNameHash;
    internal int SkinID;
    //const Riot::PackageInterface* mPackage;
    internal CharacterRecord CharRecord;
    internal string CharacterINIPath;
    internal string SkinINIPath;
    internal string SkinName;
    internal bool HasRestructuredData;
    internal int[] RecommendedItems = new int[7];
    internal readonly GlobalCharacterData.DataStruct GlobalData;
    internal uint ExtraAttributeFlags;
    internal float DeathTime;
    internal float OccludedUnitSelectableDistance;
    internal uint JointNameHashForAnimAdjustedSelection;
    internal string ContextualActionRuleConfig;

    void Init(GlobalCharacterData.DataStruct data)
    {
        data.AttackDelay = GCD.AttackDelay.Value;
        data.AttackDelayCastPercent = GCD.AttackDelayCastPercent.Value;
        data.AttackMinimumDelay = GCD.AttackMinDelay.Value;
        data.AttackMaximumDelay = GCD.AttackMaxDelay.Value;
        data.PercentAttackSpeedModMinimum = GCD.PercentAttackSpeedModMinimum.Value;
        data.CooldownMinimum = GCD.CooldownMinimum.Value;
        data.PercentRespawnTimeModMinimum = GCD.PercentRespawnTimeModMinimum.Value;
        data.PercentGoldLostOnDeathModMinimum = GCD.PercentGoldLostOnDeathModMinimum.Value;
        data.PercentEXPBonusMinimum = GCD.PercentEXPBonusMinimum.Value;
        data.PercentEXPBonusMaximum = GCD.PercentEXPBonusMaximum.Value;
        data.BuildingToBuildingRatio = DR.BuildingToBuilding.Value;
        data.BuildingToUnitRatio = DR.BuildingToUnit.Value;
        data.BuildingToHeroRatio = DR.BuildingToHero.Value;
        data.HeroToHeroRatio = DR.HeroToHero.Value;
        data.HeroToUnitRatio = DR.HeroToUnit.Value;
        data.HeroToBuildingRatio = DR.HeroToBuilding.Value;
        data.UnitToBuildingRatio = DR.UnitToBuilding.Value;
        data.UnitToUnitRatio = DR.UnitToUnit.Value;
        data.UnitToHeroRatio = DR.UnitToHero.Value;

        //v3 = 0.0;
        //if (gGetCooldownReductionCap) if function ptr isnt null
        //{
        //    v4 = ((long double(__cdecl *)(int))gGetCooldownReductionCap)(a2); call function
        //    v3 = v4;
        //}
        //*(float*)(a3 + 24) = v3;
        data.PercentCooldownModMinimum = 0;

        CharacterDataManager.GlobalCharacterData.Loaded = true;
    }

    internal void Load(string characterName, int skinId)
    {
        Stopwatch profileTimer = Stopwatch.StartNew();

        CharacterName = characterName;
        CharacterNameHash = HashFunctions.HashString(characterName);
        SkinID = skinId;

        //Riot::PackageManager::GetCharacterPackage(Instance, &name, skinID, 0);
        //Package = CharacterPackage;

        string characterININame = characterName + ".ini";
        characterININame = $"DATA/Characters/{characterName}/{characterININame}"; //Hack

        //Load some Fallback INI stuff

        LoadCharacterINI(characterININame);
    }

    void LoadCharacterINI(string path)
    {
        //Do a null check on CharacterINIPath(?)

        //Null check on GameStartData::GameStartData.MissionDir(?)

        //Assign v3 to GameStartData::sGameStartData->mMissionDir and then append "_" ?

        string characterINIPath = CharacterINIPath;

        //Read some stuff regarding health bar.
        //Looks to be stuff client-sided, like position offsets and bone attachments

        //Some Idle Particle stuff

        CharRecord.MonsterDataTableID = LS.ReadCFG_I(characterINIPath, "Data", "MonsterDataTableID", 0);

        //ReadCFGOverridePrefix_F
        CharRecord.BaseHP = LS.ReadCFG_F(characterINIPath, "Data", "BaseHP", 100.0f);
        //ReadCFGOverridePrefix_F
        CharRecord.BasePAR = LS.ReadCFG_F(characterINIPath, "Data", "BaseMP", 100.0f);

        CharRecord.StatsPerLevel[PerLevelStatType.kHP] = LS.ReadCFG_F(characterINIPath, "Data", "HPPerLevel", 0f);
        CharRecord.StatsPerLevel[PerLevelStatType.kPAR] = LS.ReadCFG_F(characterINIPath, "Data", "MPPerLevel", 0f);
        CharRecord.StatsPerLevel[PerLevelStatType.kPAR] = LS.ReadCFG_F(characterINIPath, "Data", "HPRegenPerLevel", 0f);
        CharRecord.StatsPerLevel[PerLevelStatType.kPAR] = LS.ReadCFG_F(characterINIPath, "Data", "MPRegenPerLevel", 0f);

        //ReadCFGOverridePrefix_F
        CharRecord.BaseStaticHPRegen = LS.ReadCFG_F(characterINIPath, "Data", "BaseStaticHPRegen", 1f);
        //ReadCFGOverridePrefix_F
        CharRecord.BaseFactorHPRegen = LS.ReadCFG_F(characterINIPath, "Data", "BaseFactorHPRegen", 0f);
        //ReadCFGOverridePrefix_F
        CharRecord.BaseStaticPARRegen = LS.ReadCFG_F(characterINIPath, "Data", "BaseStaticMPRegen", 1f);
        //ReadCFGOverridePrefix_F
        CharRecord.BaseFactorHPRegen = LS.ReadCFG_F(characterINIPath, "Data", "BaseFactorMPRegen", 0f);

        //ReadCFGOverridePrefix_F
        CharRecord.BasePhysicalDamage = LS.ReadCFG_F(characterINIPath, "Data", "BaseDamage", 10);
        CharRecord.StatsPerLevel[PerLevelStatType.kDamage] = LS.ReadCFG_F(characterINIPath, "Data", "DamagePerLevel", 0f);

        CharRecord.BaseArmor = LS.ReadCFG_F(characterINIPath, "Data", "Armor", 1f);
        CharRecord.StatsPerLevel[PerLevelStatType.kArmor] = LS.ReadCFG_F(characterINIPath, "Data", "ArmorPerLevel", 0f);

        CharRecord.BaseSpellBlock = LS.ReadCFG_F(characterINIPath, "Data", "SpellBlock", 0);
        CharRecord.StatsPerLevel[PerLevelStatType.kSpellBlock] = LS.ReadCFG_F(characterINIPath, "Data", "SpellBlockPerLevel", 0f);

        CharRecord.BaseDodge = LS.ReadCFG_F(characterINIPath, "Data", "BaseDodge", 0f);
        CharRecord.StatsPerLevel[PerLevelStatType.kDodge] = LS.ReadCFG_F(characterINIPath, "Data", "DodgePerLevel", 0f);

        CharRecord.BaseMissChance = LS.ReadCFG_F(characterINIPath, "Data", "BaseMissChance", 0f);

        CharRecord.BaseCrit = LS.ReadCFG_F(characterINIPath, "Data", "BaseCritChance", 0f);
        CharRecord.StatsPerLevel[PerLevelStatType.kCrit] = LS.ReadCFG_F(characterINIPath, "Data", "CritPerLevel", 0f);

        CharRecord.CritDamageMultiplier = LS.ReadCFG_F(characterINIPath, "Data", "CritDamageBonus", 2f);

        CharRecord.BaseMoveSpeed = LS.ReadCFG_I(characterINIPath, "Data", "MoveSpeed", 100);
        CharRecord.AttackRange = LS.ReadCFG_F(characterINIPath, "Data", "AttackRange", 100);
        CharRecord.AttackAutoInterruptPercent = LS.ReadCFG_F(characterINIPath, "Data", "AttackAutoInterruptPercent", 0.2f);
        CharRecord.AcquisitionRange = LS.ReadCFG_F(characterINIPath, "Data", "AcquisitionRange", 750.0f);
        CharRecord.AttackDelayCastOffsetPercentAttackSpeedRatio[0] = LS.ReadCFG_F(characterINIPath, "Data", "AttackDelayCastOffsetPercentAttackSpeedRatio", 1.0f);
        CharRecord.AttackDelayCastOffsetPercent[0] = LS.ReadCFG_F(characterINIPath, "Data", "AttackDelayCastOffsetPercent", 0f);
        CharRecord.TowerTargetingPriority = LS.ReadCFG_F(characterINIPath, "Data", "TowerTargetingPriorityBoost", 0f);
        DeathTime = LS.ReadCFG_F(characterINIPath, "Data", "DeathTime", 0f); 
        //v47 = Riot::ReadCFG_S((const char*)characterINIPath, v43, "Metadata", defaultvalue, 0);
        CharRecord.AttackDelayOffsetPercent[0] = LS.ReadCFG_F(characterINIPath, "Data", "AttackDelayOffsetPercent", 0.0f);
    }
}
