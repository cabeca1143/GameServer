using GameServerCore.Enums;
using LeagueSandbox.GameServer;
using LeagueSandbox.GameServer.Logging;
using log4net;
using System.Numerics;

namespace GameServerLib.Managers;

internal class LocationsManager
{
    ILog _logger = LoggerProvider.GetLogger();
    SpawnLoc[] SpawnLocations = new SpawnLoc[9];
    Dictionary<TeamId, ulong> NumberBotsPerTeam;

    Vector3 GetHeroSpawnLocation(int spawnPos, TeamId team)
    {
        int index = ToIncrementalNumb(team);

        if (index >= 4)
        {
            _logger.Warn("teamIndex >= 0 && teamIndex < MAX_TEAM_ARRAY");
            return Vector3.Zero;
        }

        Vector3 pos = SpawnLocations[(int)SpawnType.SPAWN_LOCATION].Loc[index].FirstOrDefault(Vector3.Zero);

        if (spawnPos >= 6)
        {
            return pos;
        }

        int count = Game.PlayerManager.GetPlayerCountOnTeam(team) + (int)NumberBotsPerTeam[team];
        Vector3 offset = Vector3.Zero;
        
        if (team is TeamId.TEAM_ORDER)
        {
            offset = LS.ReadCFG_V("Characters\\HeroSpawnOffsets.ini", $"Order{count}", $"Pos{spawnPos}", Vector3.Zero);
        }

        if (team is TeamId.TEAM_CHAOS)
        {
            offset = LS.ReadCFG_V("Characters\\HeroSpawnOffsets.ini", $"Chaos{count}", $"Pos{spawnPos}", Vector3.Zero);
        }
        pos += offset;

        return pos;
    }

    Vector3 Get(SpawnType spawnType, TeamId team, int index)
    {
        int teamIndex = ToIncrementalNumb(team);


        if (index >= 4)
        {
            _logger.Warn("teamIndex >= 0 && teamIndex < MAX_TEAM_ARRAY");
            return Vector3.Zero;
        }
        if (spawnType >= SpawnType.SPAWN_Numof)
        {
            _logger.Warn("spawnType >= 0 && spawnType < SPAWN_Numof");
            return Vector3.Zero;
        }

        return SpawnLocations[(int)spawnType].Loc[teamIndex][index];
    }

    void PushBack(SpawnType spawnType, TeamId team, Vector3 pos)
    {
        int index = ToIncrementalNumb(team);

        if(index >= 4)
        {
            _logger.Warn("teamIndex >= 0 && teamIndex < MAX_TEAM_ARRAY");
            return;
        }
        if (spawnType >= SpawnType.SPAWN_Numof) 
        {
            _logger.Warn("spawnType >= 0 && spawnType < SPAWN_Numof");
            return;
        }

        SpawnLocations[(int)spawnType].Loc[index].Add(pos);
    }

    struct SpawnLoc
    {
        public List<Vector3>[] Loc { get; init; } = [[], [], [], []];
        public SpawnLoc()
        {
        }
    }


    int ToIncrementalNumb(TeamId team) => (int)team / 100;
}
