using LeagueSandbox.GameServer.Logging;
using log4net;

namespace GameServerLib;

internal class GameStartData
{
    private ILog _logger = LoggerProvider.GetLogger();

    internal static readonly GameStartData sGameStartData = new();

    internal EGameMode GameMode;
    internal int MapID = -1;
    internal string MisionMode = string.Empty; //Typo
    internal string MissionDir = string.Empty;
    internal string ObjectCFG = string.Empty;
    internal float MissionStartTime;
    internal bool MatchedGame;
    internal List<string> UnprocessedMutators = [];
    internal List<string> ProcessedMutators = [];

    internal static GameStartData GetInstance()
    {
        return sGameStartData;
    }

    internal void SetMapID(int mapID)
    {
        sGameStartData.MapID = mapID;
        string missionDir = "LEVELS/Map" + mapID;
        MissionDir = missionDir;
        ObjectCFG = $"{MissionDir}/Scene/CFG/ObjectCFG.cfg";
    }

    internal void SetMissionMode(string missionMode, List<string>? mutators)
    {
        GameMode = missionMode switch
        {
            "CLASSIC" => EGameMode.CLASSIC,
            "ODIN" => EGameMode.ODIN,
            "TUTORIAL" => EGameMode.TUTORIAL,
            "ARAM" => EGameMode.ARAM,
            "FIRSTBLOOD" => EGameMode.FIRSTBLOOD,
            "ASCENSION" => EGameMode.ASCENSION,
            _ => EGameMode.UNKNOWN
        };
        
        if(mutators?.Count > 0)
        {
            if(UnprocessedMutators != mutators)
            {
                UnprocessedMutators.AddRange(mutators);
                GameModeComponents.Initialize(missionMode, UnprocessedMutators, ProcessedMutators);
            }
        }
    }
    internal int GetMapID()
    {
        return MapID;
    }
    internal string GetMissionDir()
    {
        return MissionDir;
    }
    internal string GetMissionMode()
    {
        if (string.IsNullOrEmpty(MisionMode))
        {
            _logger.Error("GetMissionMode called without initializing");
        }
        return MisionMode ?? "";
    }
    internal bool HasValidMissionMode()
    {
        //Check
        return GameMode != EGameMode.UNKNOWN && !string.IsNullOrEmpty(MisionMode);
    }
    internal bool HasValidMapID()
    {
        return MapID is not -1;
    }
}

internal enum EGameMode
{
    UNKNOWN = -0xFFFFFFF,
    CLASSIC = 0x0,
    ODIN = 0x1,
    TUTORIAL = 0x2,
    ARAM = 0x3,
    FIRSTBLOOD = 0x4,
    ASCENSION = 0x5,
    AMOUNT = 0x6,
};
