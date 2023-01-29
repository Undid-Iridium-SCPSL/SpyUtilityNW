using System;
using System.Collections.Generic;
using PluginAPI.Core;
using Respawning;

namespace SpyUtilityNW.Events
{
    /// <summary>
    /// Team respawning event, contains spawning team, and players.
    /// </summary>
    public class FlashBangEvent : EventArgs
    {
        public SpawnableTeamType SpawningTeam { get; }

        public FlashBangEvent(ReferenceHub attacker, ReferenceHub target)
        {
            Attacker = attacker;
            Target = target;
        }

        public ReferenceHub Target { get; set; }

        public ReferenceHub Attacker { get; set; }

    }
}