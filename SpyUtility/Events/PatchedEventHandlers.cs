using System;
using System.Collections.Generic;

namespace SpyUtilityNW.Events
{
    public class PatchedEventHandlers
    {
        public static PatchedEventHandlers Instance;
  
        public delegate void TeamRespawn(TeamRespawnEvent respawnEvent);
        public static event TeamRespawn BeforeTeamRespawn;

        public static void BeforeTeamRespawnEvent(TeamRespawnEvent curEvent)
        {
            BeforeTeamRespawn?.Invoke(curEvent);
        }
        
        public delegate void FlashBang(FlashBangEvent flashBangEvent);
        public static event FlashBang FlashbangProcessing;

        public static void FlashbangEventProcessing(FlashBangEvent flashBangEvent)
        {
            FlashbangProcessing?.Invoke(flashBangEvent);
        }
        
        public delegate void InitialWaveSpawnEvent(InitalWaveSpawnEvent flashBangEvent);
        public static event InitialWaveSpawnEvent InitialWaveSpawn;

        public static void InitalWavePlayersEvent(InitalWaveSpawnEvent initialWaveSpawnEvent)
        {
            InitialWaveSpawn?.Invoke(initialWaveSpawnEvent);
        }
    }
}