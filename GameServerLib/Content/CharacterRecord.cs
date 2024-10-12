using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using GameServerCore.Enums;

namespace LeagueSandbox.GameServer.Content
{
    public class PassiveData
    {
        public string PassiveAbilityName { get; set; } = "";
        public int[] PassiveLevels { get; set; } = new int[6];
        public string PassiveLuaName { get; set; } = "";
        public string PassiveNameStr { get; set; } = "";

        //TODO: Extend into handling several passives, when we decide on a format for that case.
    }

    public class CharacterRecord
    {
        internal uint Flags;
        internal uint ParType;
        internal string AssetCategory;
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
        internal string CriticalAttackStr;
        internal string PassiveName;
        internal string PassiveDescription;
        internal string PassiveLuaName;
        internal string PassiveToolTip;
        internal string PassiveSpell;
        internal float PassiveRange;
        internal float HitFxScale;
        internal string[] AttackNames = new string[18];
        internal float OverrideCollisionHeight;
        internal float OverrideCollisionRadius;
        internal float PathfindingCollisionRadius;
        internal float GameplayCollisionRadius;
        internal uint[] SpellMaxLevelsOverride = new uint[4];
        internal uint[,] SpellsUpLevelsOverride = new uint[4, 6];
        internal string FriendlyTooltip;
        internal string EnemyTooltip;
        internal string DisplayName;
        internal string PARName;
        internal float PARIncrements;
        //EvolutionDescription* evolutionData;
        internal string ArmorMaterial;
        internal List<string> WeaponMaterials;
        internal string CharAudioNameOverride;
        internal string MinimapOverride;
        internal string HoverIndicatorTextureName;
        internal string HoverLineIndicatorBaseTextureName;
        internal string HoverLineIndicatorTargetTextureName;
        internal bool RecordAsWard;
        internal bool UseOverrideBoundingBox;
        internal Vector3 OverrideBoundingBox;
        internal float BoundingCylinderRadius;
        internal float BoundingCylinderHeight;
        internal float BoundingSphereRadius;
        internal Dictionary<PerLevelStatType, float> StatsPerLevel;
    }
}

enum PerLevelStatType
{
    kHP = 0x0,
    kPAR = 0x1,
    kHPRegen = 0x2,
    kPARRegen = 0x3,
    kDamage = 0x4,
    kArmor = 0x5,
    kSpellBlock = 0x6,
    kDodge = 0x7,
    kCrit = 0x8,
    kAttackSpeed = 0x9,
    kAbilityPowerInc = 0xA,
    COUNT = 0xB,
};