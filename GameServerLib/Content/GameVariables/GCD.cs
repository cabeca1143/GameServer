namespace GameServerLib.Content.GameVariables;

//"Global Character Data constants"
internal static class GCD
{
    internal static CVarFloat AttackDelay = new()
    {
        Name = "gcd_AttackDelay",
        Description = "Attack delay coefficient",
        Value = 1.6f
    };
    internal static CVarFloat AttackDelayCastPercent = new()
    {
        Name = "gcd_AttackDelayCastPercent",
        Description = "Attack delay cast percent 0-1",
        Value = 0.3f
    };
    internal static CVarFloat AttackMinDelay = new()
    {
        Name = "gcd_AttackMinDelay",
        Description = "Attack min delay",
        Value = 0.4f
    };
    internal static CVarFloat PercentAttackSpeedModMinimum = new()
    {
        Name = "gcd_PercentAttackSpeedModMinimum",
        Description = "The lowest Attack Speed Percent Mod penalty can go",
        Value = -0.95f
    };
    internal static CVarFloat AttackMaxDelay = new()
    {
        Name = "gcd_AttackMaxDelay",
        Description = "Attack max delay",
        Value = 5.0f
    };
    internal static CVarFloat CooldownMinimum = new()
    {
        Name = "gcd_CooldownMinimum",
        Description = "Minimum cooldown time for a spell.",
        Value = 0
    };
    internal static CVarFloat PercentRespawnTimeModMinimum = new()
    {
        Name = "gcd_PercentRespawnTimeModMinimum",
        Description = "The lowest RespawnTime Percent Mod bonus can go.",
        Value = -0.95f
    };
    internal static CVarFloat PercentGoldLostOnDeathModMinimum = new()
    {
        Name = "gcd_PercentGoldLostOnDeathModMinimum",
        Description = "The lowest GoldLostOnDeath Percent Mod bonus can go.",
        Value = -0.95f
    };
    internal static CVarFloat PercentEXPBonusMinimum = new()
    {
        Name = "gcd_PercentEXPBonusMinimum",
        Description = "The lowest EXPBonus Percent Mod penalty can go.",
        Value = -1.0f
    };
    internal static CVarFloat PercentEXPBonusMaximum = new()
    {
        Name = "gcd_PercentEXPBonusMaximum",
        Description = "The highest EXPBonus Percent Mod bonus can go.",
        Value = 5.0f
    };
}
