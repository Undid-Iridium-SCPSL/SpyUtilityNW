﻿using PluginAPI.Core;

namespace SpyUtilityNW.Events
{
    public class TeamRespawn
    {
        public void onWaveSpawn(TeamRespawnEvent respawnEvent)
        {
            Log.Debug($"Called personal TeamRespawn base code", SpyUtilityNW.Instance.Config.Debug);
        }
    }
}