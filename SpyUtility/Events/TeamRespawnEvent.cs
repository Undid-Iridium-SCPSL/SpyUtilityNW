using System;
using System.Collections.Generic;
using PluginAPI.Core;
using Respawning;

namespace SpyUtilityNW.Events
{
    /// <summary>
    /// Team respawning event, contains spawning team, and players.
    /// </summary>
    public class TeamRespawnEvent : EventArgs
    {
        public SpawnableTeamType SpawningTeam { get; }

        public TeamRespawnEvent(List<ReferenceHub> curPlayers, SpawnableTeamType nextSpawningTeam)
        {
            RespawningPlayers = new List<Player>();
            foreach (ReferenceHub referenceHub in curPlayers)
            {
                RespawningPlayers.Add(Player.Get(referenceHub));
            }
            SpawningTeam = nextSpawningTeam;
        }

        public List<Player> RespawningPlayers { get; set; }
    }
}