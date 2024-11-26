using GameServerLib;
using GameServerLib.Content;
using LeagueSandbox.GameServer.Logging;
using log4net;
using System.Numerics;

namespace LeagueSandbox.GameServer.Content;

public class CharRecord
{
    private static ILog _logger = LoggerProvider.GetLogger();
    private static float[] PerLevelStatsFactor = new float[18];
    private static float[] AccumulatedPerLevelStatsFactor = new float[18];

    internal RecordFlagValues Flags;
    internal uint ParType;
    //internal string AssetCategory;
    internal int MonsterDataTableID;
    internal float BaseHP;
    internal float BasePAR;
    internal float BaseStaticHPRegen;
    internal float BaseFactorHPRegen;
    internal float BaseStaticPARRegen;
    internal float BaseFactorPARRegen;
    internal float BasePhysicalDamage;
    internal float BaseArmor;
    internal float BaseSpellBlock;
    internal float BaseDodge;
    internal float BaseMissChance;
    internal float BaseCrit;
    internal float CritDamageMultiplier;
    internal float BaseMoveSpeed;
    internal float AttackRange;
    internal float[] AttackDelayCastOffsetPercentAttackSpeedRatio = new float[18];
    internal float[] AttackDelayCastOffsetPercent = new float[18];
    internal float[] AttackDelayOffsetPercent = new float[18];
    internal float[] AttackProbability = new float[18];
    internal float AcquisitionRange;
    internal float AttackAutoInterruptPercent;
    internal float TowerTargetingPriority;
    internal float GoldGivenOnDeath;
    internal float ExpGivenOnDeath;
    internal float GoldRadius;
    internal float ExperienceRadius;
    internal float DeathEventListeningRadius;
    internal float LocalGoldGivenOnDeath;
    internal float LocalExpGivenOnDeath;
    internal bool LocalGoldSplitWithLastHitter;
    internal float GlobalGoldGivenOnDeath;
    internal float GlobalExpGivenOnDeath;
    internal float PerceptionBubbleRadius;
    internal float Significance;
    internal float AbilityPower;
    internal string[] SpellNames = new string[4];
    internal string[] ExtraSpells = new string[16];
    internal string CriticalAttackStr = "";
    internal string PassiveName = "";
    internal string PassiveDescription = "";
    internal string PassiveLuaName = "";
    internal string PassiveToolTip = "";
    internal string PassiveSpell = "";
    internal float PassiveRange;
    internal float HitFxScale;
    internal string[] AttackNames = new string[18];
    internal float OverrideCollisionHeight;
    internal float OverrideCollisionRadius;
    internal float PathfindingCollisionRadius;
    internal float GameplayCollisionRadius;
    internal uint[] SpellMaxLevelsOverride = new uint[4];
    internal uint[][] SpellsUpLevelsOverride = new uint[4][];
    //internal string FriendlyTooltip;
    //internal string EnemyTooltip;
    internal string DisplayName = "";
    internal string PARName = "";
    internal float PARIncrements;
    //EvolutionDescription* evolutionData;
    //internal string ArmorMaterial;
    //internal List<string> WeaponMaterials;
    //internal string CharAudioNameOverride;
    internal string MinimapOverride = "";
    //internal string HoverIndicatorTextureName;
    //internal string HoverLineIndicatorBaseTextureName;
    //internal string HoverLineIndicatorTargetTextureName;
    internal bool RecordAsWard;
    internal bool UseOverrideBoundingBox;
    internal Vector3 OverrideBoundingBox;
    internal float BoundingCylinderRadius;
    internal float BoundingCylinderHeight;
    internal float BoundingSphereRadius;
    internal Dictionary<PerLevelStatType, float> StatsPerLevel = [];

    internal double GetPerLevelStat(PerLevelStatType stat)
    {
        return StatsPerLevel[stat];
    }

    internal double GetStatForLevel(PerLevelStatType stat, int level)
    {
        if (level == 0)
        {
            return 0;
        }

        float factor = level < 18 ? PerLevelStatsFactor[level] : PerLevelStatsFactor.Last();
        return StatsPerLevel[stat] * factor;
    }

    internal static void StaticInitialize()
    {
        string statsProgressionPath = Path.Join(GameStartData.sGameStartData.GetMissionDir(), "StatsProgession.ini");

        if (Cache.Instance.GetFile(statsProgressionPath) is null)
        {
            _logger.Error($"No StatProgression file found at `{statsProgressionPath}`!");
            return;
        }

        float accumulatedStatsFactor = 0.0f;
        for (int i = 0; i + 1 < 18; i++)
        {
            string name = "Level" + (i + 1);
            float value = LS.ReadCFG_F(statsProgressionPath, "PerLevelStatsFactor", name, 0);
            PerLevelStatsFactor[i] = value;
            accumulatedStatsFactor += value;
            AccumulatedPerLevelStatsFactor[i] = accumulatedStatsFactor;
        }
    }
}

enum PerLevelStatType
{
    HP = 0x0,
    PAR = 0x1,
    HPRegen = 0x2,
    PARRegen = 0x3,
    Damage = 0x4,
    Armor = 0x5,
    SpellBlock = 0x6,
    Dodge = 0x7,
    Crit = 0x8,
    AttackSpeed = 0x9,
    AbilityPowerInc = 0xA,
    COUNT = 0xB,
};

[Flags]
internal enum RecordFlagValues
{
    None = 0x0,
    IsEpic = 0x1,
    IsElite = 0x2,
    DrawPARLikeHealth = 0x4,
    IsMelee = 0x8,
    NeverRender = 0x10,
    ServerOnly = 0x20,
    NoAutoAttack = 0x40,
    NoHealthBar = 0x80,
    ShouldFaceTarget = 0x100,
    PARDisplayThroughDeath = 0x200,
    AllowPetControl = 0x400,
    SkipDrawOutline = 0x800,
    UseChampionVisibility = 0x1000,
    TriggersOrderAcknowledgementVO = 0x2000,
    HasExpandedVO = 0x4000,
    DisableUltReadySounds = 0x8000,
    IsImportantBotTarget = 0x10000,
    HasContextualEmote = 0x20000,
    DisableGlobalDeathEffect = 0x40000,
    DisableAggroIndicator = 0x80000,
    SequentialAutoAttacks = 0x100000,
    DisableContinuousTargetFacing = 0x200000,
    Immobile = 0x400000,
    UseRingIconForKillCallout = 0x800000,
}