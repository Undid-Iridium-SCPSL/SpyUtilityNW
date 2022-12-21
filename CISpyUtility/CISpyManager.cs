using System;
using System.Collections.Generic;
using CISpyUtilityNW.Events;
using PluginAPI.Core;
using Respawning;
using System.Linq;
using Mirror;
using PlayerRoles;
using PlayerStatsSystem;
using PluginAPI.Core.Attributes;
using PluginAPI.Core.Interfaces;
using PluginAPI.Enums;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CISpyUtilityNW
{
    public class CISpyManager
    {
        
        
        public CISpyManager()
        {
            currentSpies = new HashSet<Player>();
            CISpyUtilityNW.Instance.SpyManager = this;
            pluginInstance = CISpyUtilityNW.Instance;
            Log.Debug("This spawned right", CISpyUtilityNW.Instance.Config.Debug);
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
                $"What is TeamRespawnEvent spawnTeamType {respawningEvent.SpawningTeam == SpawnableTeamType.NineTailedFox} and blah", CISpyUtilityNW.Instance.Config.Debug);

            var potentialSpies = new HashSet<Player>();
            var amountOfRetries = 3;
            int totalPlayers = respawningEvent.RespawningPlayers.Count;

            if (respawningEvent.RespawningPlayers.Count < 1)
            {
                return;
            }
            
            switch (respawningEvent.SpawningTeam)
            {
                case SpawnableTeamType.ChaosInsurgency:
                {
                    var howManyMtfSpies = 0;
                    foreach (var keyValuePair in pluginInstance.Config.probabilityOfMtfSpy)
                    {
                        Log.Debug($"Probability of ci spy {keyValuePair.Key} and {keyValuePair.Value}", CISpyUtilityNW.Instance.Config.Debug);
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
                        Log.Debug($"Probability of ci spy {keyValuePair.Key} and {keyValuePair.Value}", CISpyUtilityNW.Instance.Config.Debug);
                        if (Random.Range(0, 100) < keyValuePair.Value)
                        {
                            Log.Debug($"Probability of spy was lucky, adding spy {howManyCISpies}", CISpyUtilityNW.Instance.Config.Debug);
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
                currentSpies.Add(potentialSpy);
            }

            foreach (Player respawningPlayer in respawningEvent.RespawningPlayers)
            {
                Log.Debug($"Current player in respawning players {respawningPlayer.Nickname}", CISpyUtilityNW.Instance.Config.Debug);
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
                
                Log.Debug($"Found potential spy {curPlayerToMakeSpy}", CISpyUtilityNW.Instance.Config.Debug);
                while (players.Contains(curPlayerToMakeSpy) || currentSpies.Contains(curPlayerToMakeSpy))
                {
                    if (attempt >= 3)
                    {
                        Log.Debug($"After 3 attempts, no spy was found. Skipping sky", CISpyUtilityNW.Instance.Config.Debug);
                        unableToFind = true;
                        break;
                    }
                    
                    Log.Debug($"Player {curPlayerToMakeSpy} was already chosen, looking again", CISpyUtilityNW.Instance.Config.Debug);
                    curPlayerToMakeSpy =
                        respawningEvent.RespawningPlayers.ElementAt(Random.Range(0, totalPlayers));
                    attempt++;
                }

                if (unableToFind)
                {
                    continue;
                }
                Log.Debug($"Adding spy {curPlayerToMakeSpy}", CISpyUtilityNW.Instance.Config.Debug);
                players.Add(curPlayerToMakeSpy);
            }
        }

        [PluginEvent(ServerEventType.PlayerDamage)]
        public bool OnPlayerDamage(IPlayer target, IPlayer attacker, DamageHandlerBase damageHandler)
        {
            Log.Debug($"ON player damaged attacker {attacker.ReferenceHub}, and target {target.ReferenceHub}" +
                     $"and what are the roles? \n" +
                     $"\nattacker.ReferenceHub.roleManager.CurrentRole.Team {attacker.ReferenceHub.roleManager.CurrentRole.Team}" +
                     $"\ntarget.ReferenceHub.roleManager.CurrentRole.Team { target.ReferenceHub.roleManager.CurrentRole.Team}", CISpyUtilityNW.Instance.Config.Debug);

            Team attackerTeam = attacker.ReferenceHub.roleManager.CurrentRole.Team;
            Team targetTeam = target.ReferenceHub.roleManager.CurrentRole.Team;

            
            if (attackerTeam == Team.FoundationForces &&
                targetTeam == Team.FoundationForces)
            {
                Log.Debug($"Well both were foundation forces", CISpyUtilityNW.Instance.Config.Debug);
                Player curAttacker = Player.Get(attacker.ReferenceHub.netId);
        
                if (currentSpies.Contains(curAttacker))
                {
                    //attacker.ReferenceHub.roleManager.CurrentRole.Team = Team.ChaosInsurgency;
                    List<Player> players = Player.GetPlayers();
                    // foreach (Player curTarget in players.Where(x => x != attacker))
                    // {
                    //     // GameObject gameObject = attacker.GameObject;
                    //     // NetworkServer.UnSpawn(gameObject);
                    //     // attacker.ReferenceHub.GetTeam()
                    //     
                    // }

                    // if (PlayerRoleLoader.TryGetRoleTemplate<PlayerRoleBase>(RoleTypeId.ChaosRifleman,
                    //         out PlayerRoleBase result))
                    // {
                    //     
                    // }
                    
                    Log.Debug($"Changing current player role from {curAttacker.ReferenceHub.roleManager.CurrentRole} to {RoleTypeId.ChaosRifleman} ", CISpyUtilityNW.Instance.Config.Debug);
                    curAttacker.ReferenceHub.roleManager.ServerSetRole(RoleTypeId.ChaosRifleman, RoleChangeReason.None);
                    Log.Debug($"Finished changing {curAttacker.ReferenceHub.roleManager.CurrentRole} to {RoleTypeId.ChaosRifleman} ", CISpyUtilityNW.Instance.Config.Debug);
                    currentSpies.Remove(curAttacker);
                    curAttacker.ReceiveHint("<align=center><voffset=28em> <color=#F6511D> You've been revealed!!! </color></voffset></align>");
                }
            }

            return true;
        }
        

        
        [PluginEvent(ServerEventType.PlayerDeath)]
        public void OnPlayerDeath(IPlayer attacker, IPlayer target, DamageHandlerBase damageHandler)
        {
            
        }
        
        [PluginEvent(ServerEventType.PlayerLeft)]
        public void OnPlayerLeave(IPlayer leavingPlayer)
        {
            
        }
        
    }
}