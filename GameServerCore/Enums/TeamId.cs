namespace GameServerCore.Enums;

public enum TeamId: uint
{
    TEAM_UNKNOWN = 0x0,
    TEAM_ORDER = 0x64, // 100
    TEAM_CHAOS = 0xC8, // 200
    TEAM_NEUTRAL = 0x12C, // 300
    TEAM_MAX = 0x190, // 400
    TEAM_ALL = 0xFFFFFF9C,
}