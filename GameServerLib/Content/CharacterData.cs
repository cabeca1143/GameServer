using GameServerCore.Content;
using GameServerLib.Content.GameVariables;
using LeagueSandbox.GameServer.Content;
using LeagueSandbox.GameServer.Logging;
using log4net;
using System.Diagnostics;

namespace GameServerLib.Content;

struct CVarFloat
{
    internal string Name;
    internal string Description;
    internal float Value;
}

internal class CharacterData
{
    private ILog _logger = LoggerProvider.GetLogger();

    internal string CharacterName = string.Empty;
    internal uint CharacterNameHash;
    internal int SkinID;
    //const Riot::PackageInterface* mPackage;
    internal CharacterRecord CharRecord = new();
    internal string CharacterINIPath = string.Empty;
    internal string SkinINIPath = string.Empty;
    internal string SkinName = string.Empty;
    internal bool HasRestructuredData;
    internal int[] RecommendedItems = new int[7];
    internal readonly GlobalCharacterData.DataStruct GlobalData;
    internal uint ExtraAttributeFlags;
    internal float DeathTime;
    internal float OccludedUnitSelectableDistance;
    internal uint JointNameHashForAnimAdjustedSelection;
    internal string ContextualActionRuleConfig = string.Empty;

    internal static void Init(GlobalCharacterData.DataStruct data)
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

        float atkTotalTime = LS.ReadCFG_F(characterINIPath, "Data", "AttackTotalTime", 0.0f);
        float attackCastTime = LS.ReadCFG_F(characterINIPath, "Data", "AttackCastTime", 0.0f);

        if (atkTotalTime > 0)
        {
            float attackTotalTime = float.Min(atkTotalTime, attackCastTime);
            if (attackTotalTime > 0)
            {
                if (CharacterDataManager.GlobalCharacterData.Loaded)
                {
                    CharRecord.AttackDelayOffsetPercent[0] = atkTotalTime / CharacterDataManager.GlobalCharacterData.Data.AttackDelay + -1;
                }
                else
                {
                    _logger.Error("Global Character Data not loaded!");
                    CharRecord.AttackDelayCastOffsetPercent[0] = attackTotalTime / atkTotalTime - CharacterDataManager.GlobalCharacterData.Data.AttackDelayCastPercent;
                    CharRecord.AttackDelayCastOffsetPercentAttackSpeedRatio[0] = 1.0f;
                }
            }
        }

        string defaultAttackName = CharacterName + "BasicAttack";
        CharRecord.AttackNames[0] = defaultAttackName;
        CharRecord.AttackProbability[0] = LS.ReadCFG_F(characterINIPath, "Data", "BaseAttack_Probability", 1.0f);

        for (BasicAttackTypes slot = BasicAttackTypes.NORMAL_SLOT2; (int)slot - 63 < 18; slot++)
        {
            Helper_PopulateDefaultBasicAttackSpellName(ref defaultAttackName, slot);
        }
    }

    private void LoadBasicAttackNames(string characterINIPath)
    {
        string attackName = string.Empty;
        string str;
        for (BasicAttackTypes slot = BasicAttackTypes.FIRST_SLOT; (int)slot - 63 < 18; slot++)
        {
            Helper_PopulateDefaultBasicAttackSpellName(ref attackName, slot);
            str = "BaseAttack";
            switch (slot)
            {
                case BasicAttackTypes.NORMAL_SLOT1:
                    break;
                case BasicAttackTypes.NORMAL_SLOT2:
                    str = "ExtraAttack1";
                    break;
                case BasicAttackTypes.NORMAL_SLOT3:
                    str = "ExtraAttack2";
                    break;
                case BasicAttackTypes.NORMAL_SLOT4:
                    str = "ExtraAttack3";
                    break;
                case BasicAttackTypes.NORMAL_SLOT5:
                    str = "ExtraAttack4";
                    break;
                case BasicAttackTypes.NORMAL_SLOT6:
                    str = "ExtraAttack5";
                    break;
                case BasicAttackTypes.NORMAL_SLOT7:
                    str = "ExtraAttack6";
                    break;
                case BasicAttackTypes.NORMAL_SLOT8:
                    str = "ExtraAttack7";
                    break;
                case BasicAttackTypes.NORMAL_SLOT9:
                    str = "ExtraAttack8";
                    break;
                case BasicAttackTypes.CRITICAL_SLOT1:
                    str = "CritAttack";
                    break;
                case BasicAttackTypes.CRITICAL_SLOT2:
                    str = "ExtraCritAttack1";
                    break;
                case BasicAttackTypes.CRITICAL_SLOT3:
                    str = "ExtraCritAttack2";
                    break;
                case BasicAttackTypes.CRITICAL_SLOT4:
                    str = "ExtraCritAttack3";
                    break;
                case BasicAttackTypes.CRITICAL_SLOT5:
                    str = "ExtraCritAttack4";
                    break;
                case BasicAttackTypes.CRITICAL_SLOT6:
                    str = "ExtraCritAttack5";
                    break;
                case BasicAttackTypes.CRITICAL_SLOT7:
                    str = "ExtraCritAttack6";
                    break;
                case BasicAttackTypes.CRITICAL_SLOT8:
                    str = "ExtraCritAttack7";
                    break;
                case BasicAttackTypes.CRITICAL_SLOT9:
                    str = "ExtraCritAttack8";
                    break;
                default:
                    _logger.Warn("There is an autoattack slot without a load string associated with it in BasicAttackSlotToIniString!");
                    str = "";
                    break;
            }
            string cfg = LS.ReadCFG_S(characterINIPath, "Data", str, attackName);
            CharRecord.AttackNames[(int)slot] = cfg;
        }
    }

    static void Helper_PopulateDefaultBasicAttackSpellName(ref string str, BasicAttackTypes slot)
    {
        //slot == 64 => %sBasicAttack
        //slot > 64 <= 72 => %BasicAttack%d
        if (slot >= BasicAttackTypes.FIRST_SLOT && slot <= BasicAttackTypes.LAST_SLOT)
        {
            str = "BasicAttack";
        }
        //slot == 73 => %sCritAttack
        //slot > 73 <= 81 => %sCritAttack%d
        if (slot >= BasicAttackTypes.CRITICAL_SLOT1 && slot <= BasicAttackTypes.CRITICAL_LAST_SLOT)
        {
            str = "CritAttack";
        }
    }

    enum BasicAttackTypes
    {
        NORMAL_SLOT1 = 0x40,
        NORMAL_SLOT2 = 0x41,
        NORMAL_SLOT3 = 0x42,
        NORMAL_SLOT4 = 0x43,
        NORMAL_SLOT5 = 0x44,
        NORMAL_SLOT6 = 0x45,
        NORMAL_SLOT7 = 0x46,
        NORMAL_SLOT8 = 0x47,
        NORMAL_SLOT9 = 0x48,
        CRITICAL_SLOT1 = 0x49,
        CRITICAL_SLOT2 = 0x4A,
        CRITICAL_SLOT3 = 0x4B,
        CRITICAL_SLOT4 = 0x4C,
        CRITICAL_SLOT5 = 0x4D,
        CRITICAL_SLOT6 = 0x4E,
        CRITICAL_SLOT7 = 0x4F,
        CRITICAL_SLOT8 = 0x50,
        CRITICAL_SLOT9 = 0x51,
        MAX_SLOT = 0x52,
        FIRST_SLOT = 0x40,
        LAST_SLOT = 0x48,
        CRITICAL_LAST_SLOT = 0x51,
    };

}
