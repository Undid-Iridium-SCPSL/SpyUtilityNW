using CISpyUtilityNW.Events;
using PluginAPI.Core;
using PluginAPI.Core.Attributes;
using PluginAPI.Enums;
using Respawning;

namespace CISpyUtilityNW
{
    public class CISpyManager
    {
        public CISpyManager(CISpyUtilityNW mainPlugin)
        {
            this.pluginInstance = mainPlugin;
        }

        public CISpyUtilityNW pluginInstance { get; set; }

        public void OnWaveSpawn(TeamRespawnEvent spawnTeamType)
        {
            Log.Info($"What is TeamRespawn {spawnTeamType.SpawningTeam == SpawnableTeamType.NineTailedFox} and blah");
            foreach (var respawningPlayer in spawnTeamType.RespawningPlayers)
            {
                Log.Info($"Current player in respawning players {respawningPlayer}");
            }
        }

 
    }
}