using System;
using System.Collections.Generic;
using System.Security.Policy;
using PlayerRoles;
using PluginAPI.Core;
using Respawning;

namespace SpyUtilityNW.Events
{
    /// <summary>
    /// Team respawning event, contains spawning team, and players.
    /// </summary>
    public class InitalWaveSpawnEvent : EventArgs
    {
        public Dictionary<RoleTypeId, List<ReferenceHub>> InitalWavePlayers { get; }
        
        public HashSet<RoleTypeId> rolesAdded { get; }

        public InitalWaveSpawnEvent()
        {
            this.InitalWavePlayers = InitalWaveSpawnPatch.InitalWavePlayers;
            this.rolesAdded = InitalWaveSpawnPatch.rolesAdded;
        }
    }
}