using System.Collections.Generic;
using CISpyUtilityNW.Events;
using PluginAPI.Core;
using Respawning;
using UnityEngine;
using System.Linq;
namespace CISpyUtilityNW
{
    public class CISpyManager
    {
        public CISpyManager(CISpyUtilityNW mainPlugin)
        {
            pluginInstance = mainPlugin;
        }

        public CISpyUtilityNW pluginInstance { get; set; }

        public void OnWaveSpawn(TeamRespawnEvent respawningEvent)
        {
            Log.Info(
                $"What is TeamRespawnEvent spawnTeamType {respawningEvent.SpawningTeam == SpawnableTeamType.NineTailedFox} and blah");

           

            var potentialSpies = new HashSet<ReferenceHub>();
            var amountOfRetries = 3;
            int totalPlayers = respawningEvent.RespawningPlayers.Count;
            
            
            if (respawningEvent.SpawningTeam is SpawnableTeamType.ChaosInsurgency)
            {
                var howManyMtfSpys = 0;
                foreach (var keyValuePair in pluginInstance.Config.probabilityOfMtfSpy)
                {
                   
                    if (Random.Range(0, 100) < keyValuePair.Value)
                    {
                        
                        howManyMtfSpys++;
                    }
                } 
                
                TryToAddSpy(potentialSpies, respawningEvent, totalPlayers, howManyMtfSpys);
            }
            
            if (respawningEvent.SpawningTeam is SpawnableTeamType.NineTailedFox)
            {
                var howManyCISpys = 0;
                foreach (var keyValuePair in pluginInstance.Config.probabilityOfCISpy)
                {
                    Log.Info($"Probability of ci spy {keyValuePair.Key} and {keyValuePair.Value}");
                    if (Random.Range(0, 100) < keyValuePair.Value)
                    {
                        Log.Info($"Probability of spy was lucky, adding spy {howManyCISpys}");
                        howManyCISpys++;
                    }
                } 
                TryToAddSpy(potentialSpies, respawningEvent, totalPlayers, howManyCISpys);
            }

            foreach (var respawningPlayer in respawningEvent.RespawningPlayers)
                Log.Info($"Current player in respawning players {respawningPlayer}");


            //if(respawningEvent.RespawningPlayers)
        }

        private void TryToAddSpy(HashSet<ReferenceHub> players, TeamRespawnEvent respawningEvent,
            int totalPlayers, int howManySpies)
        {
            for (var pos = 0; pos < howManySpies; pos++)
            {
                int attempt = 0;
                bool unableToFind = false;
                    
                ReferenceHub curPlayerToMakeSpy =
                    respawningEvent.RespawningPlayers.ElementAt(Random.Range(0, totalPlayers));
                Log.Info($"Found potential spy {curPlayerToMakeSpy}");
                while (players.Contains(curPlayerToMakeSpy))
                {
                    if (attempt >= 3)
                    {
                        Log.Info($"After 3 attempts, no spy was found. Skipping sky");
                        unableToFind = true;
                        break;
                    }
                    
                    Log.Info($"Player {curPlayerToMakeSpy} was already chosen, looking again");
                    curPlayerToMakeSpy =
                        respawningEvent.RespawningPlayers.ElementAt(Random.Range(0, totalPlayers));
                    attempt++;
                }

                if (unableToFind)
                {
                    continue;
                }
                Log.Info($"Adding spy {curPlayerToMakeSpy}");
                players.Add(curPlayerToMakeSpy);
            }
        }
    }
}