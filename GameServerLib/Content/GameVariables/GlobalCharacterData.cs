namespace GameServerLib.Content.GameVariables;

internal class GlobalCharacterData
{
    internal DataStruct Data;
    internal bool Loaded;

    internal class DataStruct
    {
        internal float AttackDelay;
        internal float AttackDelayCastPercent;
        internal float AttackMinimumDelay;
        internal float AttackMaximumDelay;
        internal float PercentAttackSpeedModMinimum;
        internal float CooldownMinimum;
        internal float PercentCooldownModMinimum;
        internal float PercentRespawnTimeModMinimum;
        internal float PercentGoldLostOnDeathModMinimum;
        internal float PercentEXPBonusMinimum;
        internal float PercentEXPBonusMaximum;
        internal float HeroToHeroRatio;
        internal float HeroToUnitRatio;
        internal float HeroToBuildingRatio;
        internal float UnitToHeroRatio;
        internal float UnitToUnitRatio;
        internal float UnitToBuildingRatio;
        internal float BuildingToHeroRatio;
        internal float BuildingToUnitRatio;
        internal float BuildingToBuildingRatio;
    };
}
