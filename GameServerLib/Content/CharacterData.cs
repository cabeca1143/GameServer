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
    internal CharRecord CharRecord = new();
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
        CharacterINIPath = characterININame;
        //Load some Fallback INI stuff

        LoadCharacterINI(characterININame);
        LoadSpells(characterININame);
        LoadSkinINI(characterININame);
    
        //Log this somewhere?
        //~Riot::ProfileTimer::~ProfileTimer(&loadTimer);
    }

    void LoadSpells(string characterININame)
    {
        //TODO
    }

    void LoadSkinINI(string characterININame)
    {
        //TODO
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

        CharRecord.StatsPerLevel[PerLevelStatType.HP] = LS.ReadCFG_F(characterINIPath, "Data", "HPPerLevel", 0f);
        CharRecord.StatsPerLevel[PerLevelStatType.PAR] = LS.ReadCFG_F(characterINIPath, "Data", "MPPerLevel", 0f);
        CharRecord.StatsPerLevel[PerLevelStatType.HPRegen] = LS.ReadCFG_F(characterINIPath, "Data", "HPRegenPerLevel", 0f);
        CharRecord.StatsPerLevel[PerLevelStatType.PARRegen] = LS.ReadCFG_F(characterINIPath, "Data", "MPRegenPerLevel", 0f);

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
        CharRecord.StatsPerLevel[PerLevelStatType.Damage] = LS.ReadCFG_F(characterINIPath, "Data", "DamagePerLevel", 0f);

        CharRecord.BaseArmor = LS.ReadCFG_F(characterINIPath, "Data", "Armor", 1f);
        CharRecord.StatsPerLevel[PerLevelStatType.Armor] = LS.ReadCFG_F(characterINIPath, "Data", "ArmorPerLevel", 0f);

        CharRecord.BaseSpellBlock = LS.ReadCFG_F(characterINIPath, "Data", "SpellBlock", 0);
        CharRecord.StatsPerLevel[PerLevelStatType.SpellBlock] = LS.ReadCFG_F(characterINIPath, "Data", "SpellBlockPerLevel", 0f);

        CharRecord.BaseDodge = LS.ReadCFG_F(characterINIPath, "Data", "BaseDodge", 0f);
        CharRecord.StatsPerLevel[PerLevelStatType.Dodge] = LS.ReadCFG_F(characterINIPath, "Data", "DodgePerLevel", 0f);

        CharRecord.BaseMissChance = LS.ReadCFG_F(characterINIPath, "Data", "BaseMissChance", 0f);

        CharRecord.BaseCrit = LS.ReadCFG_F(characterINIPath, "Data", "BaseCritChance", 0f);
        CharRecord.StatsPerLevel[PerLevelStatType.Crit] = LS.ReadCFG_F(characterINIPath, "Data", "CritPerLevel", 0f);

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
            string baseAttackName = "BaseAttack";
            switch (slot)
            {
                case BasicAttackTypes.NORMAL_SLOT1:
                    break;
                case BasicAttackTypes.NORMAL_SLOT2:
                    baseAttackName = "ExtraAttack1";
                    break;
                case BasicAttackTypes.NORMAL_SLOT3:
                    baseAttackName = "ExtraAttack2";
                    break;
                case BasicAttackTypes.NORMAL_SLOT4:
                    baseAttackName = "ExtraAttack3";
                    break;
                case BasicAttackTypes.NORMAL_SLOT5:
                    baseAttackName = "ExtraAttack4";
                    break;
                case BasicAttackTypes.NORMAL_SLOT6:
                    baseAttackName = "ExtraAttack5";
                    break;
                case BasicAttackTypes.NORMAL_SLOT7:
                    baseAttackName = "ExtraAttack6";
                    break;
                case BasicAttackTypes.NORMAL_SLOT8:
                    baseAttackName = "ExtraAttack7";
                    break;
                case BasicAttackTypes.NORMAL_SLOT9:
                    baseAttackName = "ExtraAttack8";
                    break;
                case BasicAttackTypes.CRITICAL_SLOT1:
                    baseAttackName = "CritAttack";
                    break;
                case BasicAttackTypes.CRITICAL_SLOT2:
                    baseAttackName = "ExtraCritAttack1";
                    break;
                case BasicAttackTypes.CRITICAL_SLOT3:
                    baseAttackName = "ExtraCritAttack2";
                    break;
                case BasicAttackTypes.CRITICAL_SLOT4:
                    baseAttackName = "ExtraCritAttack3";
                    break;
                case BasicAttackTypes.CRITICAL_SLOT5:
                    baseAttackName = "ExtraCritAttack4";
                    break;
                case BasicAttackTypes.CRITICAL_SLOT6:
                    baseAttackName = "ExtraCritAttack5";
                    break;
                case BasicAttackTypes.CRITICAL_SLOT7:
                    baseAttackName = "ExtraCritAttack6";
                    break;
                case BasicAttackTypes.CRITICAL_SLOT8:
                    baseAttackName = "ExtraCritAttack7";
                    break;
                case BasicAttackTypes.CRITICAL_SLOT9:
                    baseAttackName = "ExtraCritAttack8";
                    break;
            }

            int arraySlot = (int)slot - 63;

            CharRecord.AttackDelayCastOffsetPercentAttackSpeedRatio[arraySlot] = LS.ReadCFG_F(characterINIPath, "Data", baseAttackName + "_AttackDelayCastOffsetPercentAttackSpeedRatio", CharRecord.AttackDelayCastOffsetPercentAttackSpeedRatio[0]);
            CharRecord.AttackDelayCastOffsetPercent[arraySlot] = LS.ReadCFG_F(characterINIPath, "Data", baseAttackName + "_AttackDelayCastOffsetPercent", CharRecord.AttackDelayCastOffsetPercent[0]);
            CharRecord.AttackDelayOffsetPercent[arraySlot] = LS.ReadCFG_F(characterINIPath, "Data", baseAttackName + "_AttackDelayOffsetPercent", CharRecord.AttackDelayOffsetPercent[0]);

            atkTotalTime = LS.ReadCFG_F(characterINIPath, "Data", baseAttackName + "_AttackTotalTime", 0.0f);
            attackCastTime = LS.ReadCFG_F(characterINIPath, "Data", baseAttackName + "_AttackTotalTime", 0.0f);

            if (atkTotalTime > 0 && attackCastTime > 0)
            {
                if (CharacterDataManager.GlobalCharacterData.Loaded)
                {
                    CharRecord.AttackDelayOffsetPercent[arraySlot] = atkTotalTime / CharacterDataManager.GlobalCharacterData.Data.AttackDelay + -1;
                }
                else
                {
                    _logger.Error("Global Character Data not loaded!");
                    CharRecord.AttackDelayCastOffsetPercent[arraySlot] = attackCastTime / atkTotalTime - CharacterDataManager.GlobalCharacterData.Data.AttackDelayCastPercent;
                    CharRecord.AttackDelayCastOffsetPercentAttackSpeedRatio[arraySlot] = 1.0f;
                }
            }

            CharRecord.AttackProbability[arraySlot] = LS.ReadCFG_F(characterINIPath, "Data", baseAttackName + "_Probability", 2.0f);
            CharRecord.AttackNames[arraySlot] = LS.ReadCFG_S(characterINIPath, "Data", baseAttackName, defaultAttackName);
        }

        LoadBasicAttackNames(characterINIPath);
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
            CharRecord.AttackNames[(int)slot - 63] = cfg;

            CharRecord.StatsPerLevel[PerLevelStatType.AttackSpeed] = LS.ReadCFG_F(characterINIPath, "Data", "AttackSpeedPerLevel", 0f);
            CharRecord.ExpGivenOnDeath = LS.ReadCFG_F(characterINIPath, "Data", "ExpGivenOnDeath", 48.0f);
            CharRecord.GoldGivenOnDeath = LS.ReadCFG_F(characterINIPath, "Data", "GoldGivenOnDeath", 25.0f);
            CharRecord.GoldRadius = LS.ReadCFG_F(characterINIPath, "Data", "GoldRadius", 0);
            CharRecord.ExperienceRadius = LS.ReadCFG_F(characterINIPath, "Data", "ExperienceRadius", 0);
            CharRecord.DeathEventListeningRadius = LS.ReadCFG_F(characterINIPath, "Data", "DeathEventListeningRadius", 1000);
            CharRecord.LocalGoldSplitWithLastHitter = LS.ReadCFG_B(characterINIPath, "Data", "LocalGoldSplitWithLastHitter", false);
            CharRecord.LocalGoldGivenOnDeath = LS.ReadCFG_F(characterINIPath, "Data", "LocalGoldGivenOnDeath", 0);
            CharRecord.LocalExpGivenOnDeath = LS.ReadCFG_F(characterINIPath, "Data", "LocalExpGivenOnDeath", 0);
            CharRecord.GlobalGoldGivenOnDeath = LS.ReadCFG_F(characterINIPath, "Data", "GlobalGoldGivenOnDeath", 0);
            CharRecord.GlobalExpGivenOnDeath = LS.ReadCFG_F(characterINIPath, "Data", "GlobalExpGivenOnDeath", 0);
            CharRecord.Significance = LS.ReadCFG_F(characterINIPath, "Data", "LocalGoldSplitWithLastHitter", 0);
            CharRecord.AbilityPower = LS.ReadCFG_F(characterINIPath, "Data", "BaseAbilityPower", 0);
            CharRecord.StatsPerLevel[PerLevelStatType.AbilityPowerInc] = LS.ReadCFG_F(characterINIPath, "Data", "AbilityPowerIncPerLevel", 0);

            float defaultValue = LS.ReadCFG_F("DATA/CFG/defaults/GamePermanent.cfg", "Vision", "PerceptionBubbleRadius", 1350);
            CharRecord.PerceptionBubbleRadius = LS.ReadCFG_F("DATA/CFG/defaults/GamePermanent.cfg", "Data", "PerceptionBubbleRadius", defaultValue);

            CharRecord.SpellNames[0] = LS.ReadCFG_S(characterINIPath, "Data", "Spell1", "BaseSpell");
            CharRecord.SpellNames[1] = LS.ReadCFG_S(characterINIPath, "Data", "Spell2", "BaseSpell");
            CharRecord.SpellNames[2] = LS.ReadCFG_S(characterINIPath, "Data", "Spell3", "BaseSpell");
            CharRecord.SpellNames[3] = LS.ReadCFG_S(characterINIPath, "Data", "Spell4", "BaseSpell");

            CharRecord.ExtraSpells[0] = LS.ReadCFG_S(characterINIPath, "Data", "ExtraSpell1", "BaseSpell");
            CharRecord.ExtraSpells[1] = LS.ReadCFG_S(characterINIPath, "Data", "ExtraSpell2", "BaseSpell");
            CharRecord.ExtraSpells[2] = LS.ReadCFG_S(characterINIPath, "Data", "ExtraSpell3", "BaseSpell");
            CharRecord.ExtraSpells[3] = LS.ReadCFG_S(characterINIPath, "Data", "ExtraSpell4", "BaseSpell");
            CharRecord.ExtraSpells[4] = LS.ReadCFG_S(characterINIPath, "Data", "ExtraSpell5", "BaseSpell");
            CharRecord.ExtraSpells[5] = LS.ReadCFG_S(characterINIPath, "Data", "ExtraSpell6", "BaseSpell");
            CharRecord.ExtraSpells[6] = LS.ReadCFG_S(characterINIPath, "Data", "ExtraSpell7", "BaseSpell");
            CharRecord.ExtraSpells[7] = LS.ReadCFG_S(characterINIPath, "Data", "ExtraSpell8", "BaseSpell");
            CharRecord.ExtraSpells[8] = LS.ReadCFG_S(characterINIPath, "Data", "ExtraSpell9", "BaseSpell");
            CharRecord.ExtraSpells[9] = LS.ReadCFG_S(characterINIPath, "Data", "ExtraSpell10", "BaseSpell");
            CharRecord.ExtraSpells[10] = LS.ReadCFG_S(characterINIPath, "Data", "ExtraSpell11", "BaseSpell");
            CharRecord.ExtraSpells[11] = LS.ReadCFG_S(characterINIPath, "Data", "ExtraSpell12", "BaseSpell");
            CharRecord.ExtraSpells[12] = LS.ReadCFG_S(characterINIPath, "Data", "ExtraSpell13", "BaseSpell");
            CharRecord.ExtraSpells[13] = LS.ReadCFG_S(characterINIPath, "Data", "ExtraSpell14", "BaseSpell");
            CharRecord.ExtraSpells[14] = LS.ReadCFG_S(characterINIPath, "Data", "ExtraSpell15", "BaseSpell");
            CharRecord.ExtraSpells[15] = LS.ReadCFG_S(characterINIPath, "Data", "ExtraSpell16", "BaseSpell");

            CharRecord.PassiveName = LS.ReadCFG_S(characterINIPath, "Data", "Passive1Name", "BadPassive");
            CharRecord.PassiveLuaName = LS.ReadCFG_S(characterINIPath, "Data", "Passive1LuaName", "BadPassive");

            //Necessary?
            //CharRecord.PassiveDescription = LS.ReadCFG_S(characterINIPath, "Data", "Passive1Desc", "BadDesc");
            //CharRecord.PassiveToolTip = LS.ReadCFG_S(characterINIPath, "Data", "PassLev1Desc1", "BadDesc");
            //CharRecord.PassiveSpell = LS.ReadCFG_S(characterINIPath, "Data", "PassiveSpell", "BadDesc");

            CharRecord.PassiveRange = LS.ReadCFG_F(characterINIPath, "Data", "Passive1Range", 0);
            CharRecord.RecordAsWard = LS.ReadCFG_B(characterINIPath, "Data", "RecordAsWard", false);

            //CharRecord.RecordAsWard = LS.ReadCFG_S(characterINIPath, "Data", "Passive1Icon", false);

            //Load Passive Icon

            //Load Lore

            //Load Tips

            //Load Friend and Enemy ToolTips

            CharRecord.DisplayName = LS.ReadCFG_S(characterINIPath, "Data", "Name", "");

            CharRecord.PARName = LS.ReadCFG_S(characterINIPath, "Data", "PARType", "MP");

            CharRecord.PARName = LS.ReadCFG_S(characterINIPath, "Data", "PARNameString", "0");
            CharRecord.PARIncrements = LS.ReadCFG_F(characterINIPath, "Data", "PARIncrements", 0);
            CharRecord.MinimapOverride = LS.ReadCFG_S(characterINIPath, "Minimap", "MinimapIconOverride", "");

            CharRecord.HitFxScale = LS.ReadCFG_F(characterINIPath, "Data", "HitFxScale", 1);
            CharRecord.OverrideCollisionHeight = LS.ReadCFG_F(characterINIPath, "Data", "SelectionHeight", -1);
            CharRecord.OverrideCollisionRadius = LS.ReadCFG_F(characterINIPath, "Data", "SelectionRadius", -1);
            CharRecord.PathfindingCollisionRadius = LS.ReadCFG_F(characterINIPath, "Data", "PathfindingCollisionRadius", -1);

            float defaultColisionRadius = LS.ReadCFG_F("Data/Characters/GeneralCharacterData.ini", "GeneralDataHero", "DefaultChampionCollisionRadius", 65);
            CharRecord.GameplayCollisionRadius = LS.ReadCFG_F(characterINIPath, "Data", "GameplayCollisionRadius", 65);

            CharRecord.SpellMaxLevelsOverride[0] = 5;
            CharRecord.SpellsUpLevelsOverride[0] = new uint[6];
            CharRecord.SpellsUpLevelsOverride[0][0] = 1;
            CharRecord.SpellsUpLevelsOverride[0][1] = 3;
            CharRecord.SpellsUpLevelsOverride[0][2] = 5;
            CharRecord.SpellsUpLevelsOverride[0][3] = 7;
            CharRecord.SpellsUpLevelsOverride[0][4] = 9;
            CharRecord.SpellsUpLevelsOverride[0][5] = 99;
            CharRecord.SpellMaxLevelsOverride[1] = 5;
            CharRecord.SpellsUpLevelsOverride[1] = new uint[6];
            CharRecord.SpellsUpLevelsOverride[1][0] = 1;
            CharRecord.SpellsUpLevelsOverride[1][1] = 3;
            CharRecord.SpellsUpLevelsOverride[1][2] = 5;
            CharRecord.SpellsUpLevelsOverride[1][3] = 7;
            CharRecord.SpellsUpLevelsOverride[1][4] = 9;
            CharRecord.SpellsUpLevelsOverride[1][5] = 99;
            CharRecord.SpellMaxLevelsOverride[2] = 5;
            CharRecord.SpellsUpLevelsOverride[2] = new uint[6];
            CharRecord.SpellsUpLevelsOverride[2][0] = 1;
            CharRecord.SpellsUpLevelsOverride[2][1] = 3;
            CharRecord.SpellsUpLevelsOverride[2][2] = 5;
            CharRecord.SpellsUpLevelsOverride[2][3] = 7;
            CharRecord.SpellsUpLevelsOverride[2][4] = 9;
            CharRecord.SpellsUpLevelsOverride[2][5] = 99;
            CharRecord.SpellMaxLevelsOverride[3] = 3;
            CharRecord.SpellsUpLevelsOverride[3] = new uint[6];
            CharRecord.SpellsUpLevelsOverride[3][0] = 6;
            CharRecord.SpellsUpLevelsOverride[3][1] = 11;
            CharRecord.SpellsUpLevelsOverride[3][2] = 16;
            CharRecord.SpellsUpLevelsOverride[3][3] = 99;
            CharRecord.SpellsUpLevelsOverride[3][4] = 99;
            CharRecord.SpellsUpLevelsOverride[3][5] = 99;
        }

        if (LS.ReadCFG_4UI(characterINIPath, "Data", "MaxLevels", ref CharRecord.SpellMaxLevelsOverride))
        {
            LS.ReadCFG_6UI(characterINIPath, "Data", "SpellsUpLevels1", ref CharRecord.SpellsUpLevelsOverride[0]);
            LS.ReadCFG_6UI(characterINIPath, "Data", "SpellsUpLevels2", ref CharRecord.SpellsUpLevelsOverride[1]);
            LS.ReadCFG_6UI(characterINIPath, "Data", "SpellsUpLevels3", ref CharRecord.SpellsUpLevelsOverride[2]);
            LS.ReadCFG_6UI(characterINIPath, "Data", "SpellsUpLevels4", ref CharRecord.SpellsUpLevelsOverride[3]);
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
