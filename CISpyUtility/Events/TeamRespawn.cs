using PluginAPI.Core;

namespace CISpyUtilityNW.Events
{
    public class TeamRespawn
    {
        public void onWaveSpawn(TeamRespawnEvent respawnEvent)
        {
            Log.Debug($"Called personal TeamRespawn base code");
        }
    }
}