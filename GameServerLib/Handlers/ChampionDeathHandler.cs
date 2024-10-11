using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;
using System.Collections.Generic;
using LeaguePackets.Game.Events;
using LeagueSandbox.GameServer;
using System.Linq;
using System;
using LeagueSandbox.GameServer.Handlers;
using GameServerLib.GameObjects.AttackableUnits;

namespace GameServerLib.Handlers;

internal static class ChampionDeathHandler
{
    private const float EVIL_UNK_CONST_LEVEL_DIFF_BONUS_EXP_MULT = 0.1455f;

    private static Game _game;
    internal static void Init(Game game)
    {
        _game = game;
    }
    /// <summary>
    /// I want to keep the definition on LeaguePackets untouched, so I made this constructor here
    /// </summary>
    /// <param name="deathData"></param>
    /// <param name="marker"></param>
    /// <returns></returns>
    internal static OnDeathAssist OnDeathAssistConstructor(DeathData deathData, AssistMarker marker)
    {
        return new()
        {
            AtTime = marker.StartTime,
            PhysicalDamage = marker.PhysicalDamage,
            MagicalDamage = marker.MagicalDamage,
            TrueDamage = marker.TrueDamage,
            OrginalGoldReward = deathData.GoldReward,
            KillerNetID = deathData.Killer.NetId,
            OtherNetID = deathData.Unit.NetId
        };
    }

    internal static void NotifyAssistEvent(Dictionary<ObjAIBase, AssistMarker> assists, DeathData data)
    {
        float assistPercent = 1.0f / assists.Count;
        foreach (var champion in assists.Keys)
        {
            OnDeathAssist onDeathAssist = OnDeathAssistConstructor(data, assists[champion]);
            onDeathAssist.PercentageOfAssist = assistPercent;
            _game.PacketNotifier.NotifyOnEvent(onDeathAssist, champion);
        }
    }

    internal static void NotifyDeathEvent(DeathData data, ObjAIBase[] assists = null)
    {
        OnChampionDie championDie = new()
        {
            OtherNetID = data.Killer.NetId,
            GoldGiven = data.GoldReward,
            AssistCount = assists.Length,
        };

        if (assists != null)
        {
            for (int i = 0; i < assists.Length && i < 12; i++)
            {
                championDie.Assists[i] = assists[i].NetId;
            }
        }

        _game.PacketNotifier.NotifyOnEvent(championDie, data.Unit);
    }

    internal static void ProcessKill(DeathData data)
    {
        MapScriptHandler map = _game.Map;

        // TODO: Find out if it's possible to unhardcode some of the fractions used here.
        Champion killed = data.Unit as Champion;
        Champion killer = data.Killer as Champion;
        data.GoldReward = map.MapScript.MapScriptMetadata.ChampionBaseGoldValue;
        if (killed.ChampStats.CurrentKillingSpree > 1)
        {
            data.GoldReward = Math.Min(data.GoldReward * MathF.Pow(7f / 6f, killed.ChampStats.CurrentKillingSpree - 1), map.MapScript.MapScriptMetadata.ChampionMaxGoldValue);
        }
        else if (killed.ChampStats.CurrentKillingSpree == 0 && killed.DeathSpree >= 1)
        {
            data.GoldReward *= 11f / 12f;
            if (killed.DeathSpree > 1)
            {
                data.GoldReward = Math.Max(data.GoldReward * MathF.Pow(0.8f, killed.DeathSpree / 2), map.MapScript.MapScriptMetadata.ChampionMinGoldValue);
            }
            killed.DeathSpree++;
        }

        if (!map.MapScript.HasFirstBloodHappened)
        {
            data.GoldReward += map.MapScript.MapScriptMetadata.FirstBloodExtraGold;
            map.MapScript.HasFirstBloodHappened = true;
        }

        Dictionary<ObjAIBase, AssistMarker> assists = new();
        foreach (var assistMarker in killed.EnemyAssistMarkers)
        {
            if (!assists.ContainsKey(assistMarker.Source) && assistMarker.Source is Champion c)
            {
                assists.Add(c, assistMarker);
                RecursiveGetAlliedAssists(assists, c, data);
            }
        }
        assists.Remove(killer);
        assists = assists.OrderBy(x => x.Value.StartTime).ToDictionary(x => x.Key, x => x.Value);
        ObjAIBase[] assistObjArray = assists.Keys.ToArray();

        NotifyAssistEvent(assists, data);
        NotifyDeathEvent(data, assistObjArray);
        NotifyChampionKillEvent(data);
        ProcessKillRewards(killed, killer, assistObjArray, data.GoldReward);
        UpdateKillerStats(killer);
        killed.KillSpree = 0;
        killed.DeathSpree++;
    }

    internal static void NotifyChampionKillEvent(DeathData data)
    {
        _game.PacketNotifier.NotifyOnEvent(new OnChampionKill() { OtherNetID = data.Unit.NetId }, data.Killer);
    }

    internal static void UpdateKillerStats(Champion c)
    {
        //Check if GoldFromMinions should be reset
        c.GoldFromMinions = 0;
        c.ChampStats.Kills++;
        c.DeathSpree = 0;
        c.KillSpree++;
        if (c.KillSpree > c.ChampStats.LargestKillingSpree)
        {
            c.ChampStats.LargestKillingSpree = c.KillSpree;
        }
    }

    internal static void RecursiveGetAlliedAssists(Dictionary<ObjAIBase, AssistMarker> assistMarkers, Champion champ, DeathData deathData)
    {
        foreach (var assist in champ.AlliedAssistMarkers)
        {
            if (assist.Source is not Champion c)
            {
                continue;
            }

            if (!assistMarkers.ContainsKey(assist.Source))
            {
                assistMarkers.Add(assist.Source, assist);
                RecursiveGetAlliedAssists(assistMarkers, c, deathData);
            }
            else
            {
                assistMarkers[c].StartTime = assistMarkers[c].StartTime < assist.StartTime ? assist.StartTime : assistMarkers[c].StartTime;
            }
        }
    }

    internal static void ProcessKillRewards(Champion killed, Champion killer, ObjAIBase[] assists, float gold)
    {
        float xpShareFactor = assists.Length + 1;

        killer.AddExperience(GetEXPGrantedFromChampion(killer, killed) / xpShareFactor);
        foreach (ObjAIBase obj in assists)
        {
            if (obj is Champion c)
            {
                c.AddExperience(GetEXPGrantedFromChampion(c, killed) / xpShareFactor);
            }
        }

        killer.AddGold(killer, gold);
        foreach (ObjAIBase obj in assists)
        {
            if (obj is not Champion c)
            {
                continue;
            }

            float assistGold = gold / 2 * (1.0f / assists.Length);
            int killDiff = c.ChampStats.Assists - c.ChampStats.Kills;
            if (killDiff > 0)
            {
                assistGold += 15 + 15 * MathF.Min(killDiff, 3);
            }
            //TODO: Check if the Assist + Bonus can't exeed the original value gained by the killer or just the Bonus alone.
            assistGold = MathF.Min(gold, assistGold);

            c.AddGold(c, assistGold);
            c.ChampStats.Assists++;
        }
    }

    public static float GetEXPGrantedFromChampion(Champion killer, Champion killed)
    {
        int cLevel = killed.Stats.Level;
        float EXP = (_game.Map.MapData.ExpCurve[cLevel] - _game.Map.MapData.ExpCurve[cLevel - 1]) * _game.Map.MapData.BaseExpMultiple;

        float levelDifference = cLevel - killer.Stats.Level;

        if (cLevel < 0)
        {
            EXP -= EXP * MathF.Min(_game.Map.MapData.LevelDifferenceExpMultiple * levelDifference, _game.Map.MapData.MinimumExpMultiple);
        }
        else if (cLevel > 0)
        {
            EXP += EXP * (levelDifference * EVIL_UNK_CONST_LEVEL_DIFF_BONUS_EXP_MULT);
        }

        return EXP;
    }
}
