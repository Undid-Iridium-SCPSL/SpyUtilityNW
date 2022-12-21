using System;
using System.Collections.Generic;
using Respawning;

namespace CISpyUtilityNW.Events
{
    public class TeamRespawnEvent : EventArgs
    {
        public SpawnableTeamType SpawningTeam { get; }

        public TeamRespawnEvent(List<ReferenceHub> curPlayers, SpawnableTeamType nextSpawningTeam)
        {
            RespawningPlayers = curPlayers;
            SpawningTeam = nextSpawningTeam;
        }

        public List<ReferenceHub> RespawningPlayers { get; set; }
    }
}