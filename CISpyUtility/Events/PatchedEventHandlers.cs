using System;
using System.Collections.Generic;

namespace CISpyUtilityNW.Events
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
    }
}