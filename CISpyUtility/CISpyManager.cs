using System;
using System.Collections.Generic;
using CISpyUtilityNW.Events;
using PluginAPI.Core;
using Respawning;
using System.Linq;
using PluginAPI.Core.Attributes;
using PluginAPI.Enums;
using Random = UnityEngine.Random;

namespace CISpyUtilityNW
{
    public class CISpyManager
    {
        public CISpyManager(CISpyUtilityNW mainPlugin)
        {
            pluginInstance = mainPlugin;
            currentSpies = new HashSet<Player>();
        }

        public static ISet<Player> currentSpies = new HashSet<Player>();

        private CISpyUtilityNW pluginInstance { get; set; }

        /// <summary>
        /// Handles TeamRspawn for CISpy
        /// </summary>
        /// <param name="respawningEvent"></param>
        public void OnWaveSpawn(TeamRespawnEvent respawningEvent)
        {
            Log.Debug(
                $"What is TeamRespawnEvent spawnTeamType {respawningEvent.SpawningTeam == SpawnableTeamType.NineTailedFox} and blah");

            var potentialSpies = new HashSet<Player>();
            var amountOfRetries = 3;
            int totalPlayers = respawningEvent.RespawningPlayers.Count;
            
            
            switch (respawningEvent.SpawningTeam)
            {
                case SpawnableTeamType.ChaosInsurgency:
                {
                    var howManyMtfSpies = 0;
                    foreach (var keyValuePair in pluginInstance.Config.probabilityOfMtfSpy)
                    {
                        Log.Debug($"Probability of ci spy {keyValuePair.Key} and {keyValuePair.Value}"); 
                        if (Random.Range(0, 100) < keyValuePair.Value)
                        {
                            Log.Debug($"Probability of spy was lucky, adding spy {howManyMtfSpies}");
                            howManyMtfSpies++;
                        }
                    } 
                
                    TryToAddSpies(potentialSpies, respawningEvent, totalPlayers, howManyMtfSpies);
                    break;
                }
                case SpawnableTeamType.NineTailedFox:
                {
                    var howManyCISpies = 0;
                    foreach (var keyValuePair in pluginInstance.Config.probabilityOfCISpy) 
                    { 
                        Log.Debug($"Probability of ci spy {keyValuePair.Key} and {keyValuePair.Value}"); 
                        if (Random.Range(0, 100) < keyValuePair.Value)
                        {
                            Log.Debug($"Probability of spy was lucky, adding spy {howManyCISpies}");
                            howManyCISpies++;
                        }
                    } 
                    TryToAddSpies(potentialSpies, respawningEvent, totalPlayers, howManyCISpies);
                    break;
                }
                case SpawnableTeamType.None:
                    break;
                default:
                    return;
            }
            
            /// Technically this can be reduced in TryToAddSpies to just add to static but I feel that is not safe.
            foreach (Player potentialSpy in potentialSpies)
            {
                potentialSpies.Add(potentialSpy);
            }

            foreach (Player respawningPlayer in respawningEvent.RespawningPlayers)
            {
                Log.Debug($"Current player in respawning players {respawningPlayer}");
            }

        }

        /// <summary>
        /// Try to add <see cref="howManySpies"/> spies to the list of spies to be spawned
        /// </summary>
        /// <param name="players">Spawning players <see cref="ReferenceHub"/></param>
        /// <param name="respawningEvent">Respawn Event <see cref="TeamRespawnEvent"/></param>
        /// <param name="totalPlayers">Amount of players available</param>
        /// <param name="howManySpies">How many spies to create, if possible</param>
        private static void TryToAddSpies(ISet<Player> players, TeamRespawnEvent respawningEvent,
            int totalPlayers, int howManySpies)
        {
            for (var pos = 0; pos < howManySpies; pos++)
            {
                int attempt = 0;
                bool unableToFind = false;
                    
                Player curPlayerToMakeSpy =
                    respawningEvent.RespawningPlayers.ElementAt(Random.Range(0, totalPlayers));
                
                Log.Debug($"Found potential spy {curPlayerToMakeSpy}");
                while (players.Contains(curPlayerToMakeSpy) || currentSpies.Contains(curPlayerToMakeSpy))
                {
                    if (attempt >= 3)
                    {
                        Log.Debug($"After 3 attempts, no spy was found. Skipping sky");
                        unableToFind = true;
                        break;
                    }
                    
                    Log.Debug($"Player {curPlayerToMakeSpy} was already chosen, looking again");
                    curPlayerToMakeSpy =
                        respawningEvent.RespawningPlayers.ElementAt(Random.Range(0, totalPlayers));
                    attempt++;
                }

                if (unableToFind)
                {
                    continue;
                }
                Log.Debug($"Adding spy {curPlayerToMakeSpy}");
                players.Add(curPlayerToMakeSpy);
            }
        }

        [PluginEvent(ServerEventType.PlayerDamage)]
        public void OnDamage()
        {
            
        }
        
    }
}