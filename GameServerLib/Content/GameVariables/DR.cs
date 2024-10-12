namespace GameServerLib.Content.GameVariables;

//"Damage Ratios"
internal static class DR 
{
    internal static CVarFloat HeroToHero = new()
    {
        Name = "dr_HeroToHero",
        Description = " ",
        Value = 1.0f
    };
    internal static CVarFloat BuildingToHero = new()
    {
        Name = "dr_BuildingToHero",
        Description = " ",
        Value = 0.8f
    };
    internal static CVarFloat UnitToHero = new()
    {
        Name = "dr_UnitToHero",
        Description = " ",
        Value = 0.5f
    };
    internal static CVarFloat HeroToUnit = new()
    {
        Name = "dr_HeroToUnit",
        Description = " ",
        Value = 1.0f
    };
    internal static CVarFloat BuildingToUnit = new()
    {
        Name = "dr_BuildingToUnit",
        Description = " ",
        Value = 1.3f
    };
    internal static CVarFloat UnitToUnit = new()
    {
        Name = "dr_UnitToUnit",
        Description = " ",
        Value = 1.0f
    };
    internal static CVarFloat HeroToBuilding = new()
    {
        Name = "dr_HeroToBuilding",
        Description = " ",
        Value = 1.0f
    };
    internal static CVarFloat BuildingToBuilding = new()
    {
        Name = "dr_BuildingToBuilding",
        Description = " ",
        Value = 1.0f
    };
    internal static CVarFloat UnitToBuilding = new()
    {
        Name = "dr_UnitToBuilding",
        Description = " ",
        Value = 0.7f
    };
}
