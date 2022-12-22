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
    public class SpyManager
    {
        
        
        public SpyManager()
        {
            currentCISpies = new HashSet<Player>();
            SpyUtilityNW.Instance.SpyManager = this;
            pluginInstance = SpyUtilityNW.Instance;
            Log.Debug("This spawned right", SpyUtilityNW.Instance.Config.Debug);
        }

        public static ISet<Player> currentCISpies = new HashSet<Player>();
        public static ISet<Player> currentMtfSpies = new HashSet<Player>();
        
        private SpyUtilityNW pluginInstance { get; set; }

        /// <summary>
        /// Handles TeamRspawn for CISpy
        /// </summary>
        /// <param name="respawningEvent"></param>
        public void OnWaveSpawn(TeamRespawnEvent respawningEvent)
        {
            Log.Debug(
                $"What is TeamRespawnEvent spawnTeamType {respawningEvent.SpawningTeam == SpawnableTeamType.NineTailedFox} and blah", SpyUtilityNW.Instance.Config.Debug);

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
                        Log.Debug($"Probability of ci spy {keyValuePair.Key} and {keyValuePair.Value}", SpyUtilityNW.Instance.Config.Debug);
                        if (Random.Range(0, 100) < keyValuePair.Value)
                        {
                            Log.Debug($"Probability of spy was lucky, adding spy {howManyMtfSpies}");
                            howManyMtfSpies++;
                        }
                    } 
                
                    TryToAddSpies(potentialSpies, respawningEvent, totalPlayers, howManyMtfSpies);
                    foreach (Player potentialSpy in potentialSpies)
                    {
                        currentMtfSpies.Add(potentialSpy);
                    }
                    break;
                }
                case SpawnableTeamType.NineTailedFox:
                {
                    var howManyCISpies = 0;
                    foreach (var keyValuePair in pluginInstance.Config.probabilityOfCISpy) 
                    { 
                        Log.Debug($"Probability of ci spy {keyValuePair.Key} and {keyValuePair.Value}", SpyUtilityNW.Instance.Config.Debug);
                        if (Random.Range(0, 100) < keyValuePair.Value)
                        {
                            Log.Debug($"Probability of spy was lucky, adding spy {howManyCISpies}", SpyUtilityNW.Instance.Config.Debug);
                            howManyCISpies++;
                        }
                    } 
                    TryToAddSpies(potentialSpies, respawningEvent, totalPlayers, howManyCISpies);
                    /// Technically this can be reduced in TryToAddSpies to just add to static but I feel that is not safe.
                    foreach (Player potentialSpy in potentialSpies)
                    {
                        currentCISpies.Add(potentialSpy);
                    }

                    break;
                }
                case SpawnableTeamType.None:
                    break;
                default:
                    return;
            }
            
         
            foreach (Player respawningPlayer in respawningEvent.RespawningPlayers)
            {
                Log.Debug($"Current player in respawning players {respawningPlayer.Nickname}", SpyUtilityNW.Instance.Config.Debug);
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
                
                Log.Debug($"Found potential spy {curPlayerToMakeSpy.Nickname}", SpyUtilityNW.Instance.Config.Debug);
                while (players.Contains(curPlayerToMakeSpy) || currentCISpies.Contains(curPlayerToMakeSpy))
                {
                    if (attempt >= 3)
                    {
                        Log.Debug($"After 3 attempts, no spy was found. Skipping sky", SpyUtilityNW.Instance.Config.Debug);
                        unableToFind = true;
                        break;
                    }
                    
                    Log.Debug($"Player {curPlayerToMakeSpy.Nickname} was already chosen, looking again", SpyUtilityNW.Instance.Config.Debug);
                    curPlayerToMakeSpy =
                        respawningEvent.RespawningPlayers.ElementAt(Random.Range(0, totalPlayers));
                    attempt++;
                }

                if (unableToFind)
                {
                    continue;
                }
                Log.Debug($"Adding spy {curPlayerToMakeSpy.Nickname}", SpyUtilityNW.Instance.Config.Debug);
                players.Add(curPlayerToMakeSpy);
            }
        }

        public static bool ForceAddSpy(Player player, Team team)
        {
            switch (team)
            {
                case Team.ChaosInsurgency:
                    currentCISpies.Add(player);
                    break;
                case Team.FoundationForces:
                    currentMtfSpies.Add(player);
                    break;
                default:
                    return false;
            }

            return true;
        }

        public enum RevealStatus
        {
            SpyWasRevealed,
            SpiesWereRevealed,
            SpyDamagedSpy,
            AllowNormalDamage,
            SpyAttackingTeammate
        }
        [PluginEvent(ServerEventType.PlayerDamage)]
        public bool OnPlayerDamage(IPlayer target, IPlayer attacker, DamageHandlerBase damageHandler)
        {
            Log.Debug($"ON player damaged attacker {attacker?.ReferenceHub}, and target {target?.ReferenceHub}" +
                     $"and what are the roles? \n" +
                     $"\nattacker.ReferenceHub.roleManager.CurrentRole.Team {attacker?.ReferenceHub?.roleManager.CurrentRole.Team}" +
                     $"\ntarget.ReferenceHub.roleManager.CurrentRole.Team { target?.ReferenceHub?.roleManager.CurrentRole.Team}", SpyUtilityNW.Instance.Config.Debug);

            if (target == attacker)
            {
                return true;
            }

            if (target == null || attacker == null)
            {
                return true;
            }
            
            Team attackerTeam = attacker.ReferenceHub.roleManager.CurrentRole.Team;
            Team targetTeam = target.ReferenceHub.roleManager.CurrentRole.Team;
            Player curAttacker = Player.Get(attacker.ReferenceHub.netId);
            Player curTarget = Player.Get(target.ReferenceHub.netId);

           
            RevealStatus wasCISpyRevealed = CheckIfCiSpyReveal(attackerTeam, targetTeam, curAttacker, curTarget, currentCISpies, currentMtfSpies);
            switch (wasCISpyRevealed)
            {
                case RevealStatus.AllowNormalDamage:
                    break;
                case RevealStatus.SpyDamagedSpy:
                case RevealStatus.SpyAttackingTeammate:
                    return false;
                case RevealStatus.SpyWasRevealed:
                case RevealStatus.SpiesWereRevealed:
                    return true;
            }
            RevealStatus wasMtfSpyRevealed = CheckIfMtfSpyReveal(attackerTeam, targetTeam, curAttacker, curTarget, currentMtfSpies, currentCISpies);
            switch (wasMtfSpyRevealed)
            {
                case RevealStatus.AllowNormalDamage:
                    break;
                case RevealStatus.SpyDamagedSpy:
                case RevealStatus.SpyAttackingTeammate:
                    return false;
                case RevealStatus.SpyWasRevealed:
                case RevealStatus.SpiesWereRevealed:
                    return true;
            }
            
            return true;
        }

        private RevealStatus CheckIfMtfSpyReveal(Team attackerTeam, Team targetTeam, Player curAttacker,
            Player curTarget, ISet<Player> mtfSpies, ISet<Player> enemySpies)
        {
            // If both are on the same team, possibly reveals
            if (attackerTeam == Team.ChaosInsurgency &&
                targetTeam == Team.ChaosInsurgency)
            {
                Log.Debug($"Well both were ChaosInsurgency forces", SpyUtilityNW.Instance.Config.Debug);
                return revealRoleIfNeeded(curAttacker, curTarget, mtfSpies, RoleTypeId.NtfSergeant, enemySpies, RoleTypeId.ChaosRifleman);
            }

            // If we're both spies, normal damage
            if ((currentCISpies.Contains(curAttacker) && currentMtfSpies.Contains(curTarget)) || 
                (currentMtfSpies.Contains(curAttacker) && currentCISpies.Contains(curTarget)))
            {
                return RevealStatus.AllowNormalDamage;
            }
            
            // If target is MTF, and Chaos agent is MTF spy, do no damage
            if (targetTeam == Team.FoundationForces && currentMtfSpies.Contains(curAttacker))
            {
                curAttacker.ReceiveHint($"<align=center><voffset=28em> <color=#F6511D> You're on {Team.FoundationForces} team, remember? </color></voffset></align>");
                return RevealStatus.SpyAttackingTeammate;
            }
            
            // If attacker is MTF, and the target is Chaos agent who is MTF spy, do no damage.
            if (attackerTeam == Team.FoundationForces && currentMtfSpies.Contains(curTarget))
            {
                curAttacker.ReceiveHint($"<align=center><voffset=28em> <color=#F6511D> They're on {Team.FoundationForces} team, remember? </color></voffset></align>");
                return RevealStatus.SpyAttackingTeammate;
            }

            
            return RevealStatus.AllowNormalDamage;
        }

        private static RevealStatus CheckIfCiSpyReveal(Team attackerTeam, Team targetTeam, Player curAttacker,
            Player curTarget, ISet<Player> currentCiSpies, ISet<Player> enemySpies)
        {
            //If on same team, identify if we're spies, and whether we can do damage to each other
            if (attackerTeam == Team.FoundationForces &&
                targetTeam == Team.FoundationForces)
            {
                Log.Debug($"Well both were foundation forces", SpyUtilityNW.Instance.Config.Debug);

                return revealRoleIfNeeded(curAttacker, curTarget, currentCiSpies, RoleTypeId.ChaosRifleman, enemySpies, RoleTypeId.NtfSergeant);
            }
            
            if ((currentCISpies.Contains(curAttacker) && currentMtfSpies.Contains(curTarget)) || 
                (currentMtfSpies.Contains(curAttacker) && currentCISpies.Contains(curTarget)))
            {
                return RevealStatus.AllowNormalDamage;
            }

            // If I am attacking a CI Spy attacking Chaos, I do no damage
            if (targetTeam == Team.ChaosInsurgency && currentCISpies.Contains(curAttacker))
            {
                curAttacker.ReceiveHint($"<align=center><voffset=28em> <color=#F6511D> You're on {Team.ChaosInsurgency} team, remember? </color></voffset></align>");
                return RevealStatus.SpyAttackingTeammate;
            }
            
            // If I am chaos attacking another player who is a CI spy, I do no damage
            if (attackerTeam == Team.ChaosInsurgency && currentCISpies.Contains(curTarget))
            {
                curAttacker.ReceiveHint($"<align=center><voffset=28em> <color=#F6511D> They're on {Team.ChaosInsurgency} team, remember? </color></voffset></align>");
                return RevealStatus.SpyAttackingTeammate;
            }
         

            return RevealStatus.AllowNormalDamage;
        }

        private static RevealStatus revealRoleIfNeeded(Player curAttacker, Player curTarget,
            ISet<Player> curTeamSpyList,
            RoleTypeId newRoleToSwapTo,
            ISet<Player> curEnemyTeamList,
            RoleTypeId newEnemyRoleToSwapTo)
        {
            //If the attacker and target are both spies for same team
            if (curTeamSpyList.Contains(curAttacker) && curTeamSpyList.Contains(curTarget))
            {
                curAttacker.ReceiveHint("<align=center><voffset=28em> <color=#F6511D> That's a fellow spy! You can't attack them </color></voffset></align>");
                return RevealStatus.SpyDamagedSpy;
            }

            //If attacker and target are spies for enemy teams (reveal both)
            if (curTeamSpyList.Contains(curAttacker) && curEnemyTeamList.Contains(curTarget)||
                curTeamSpyList.Contains(curTarget) && curEnemyTeamList.Contains(curAttacker))
            {
                curAttacker.ReferenceHub.roleManager.ServerSetRole(newRoleToSwapTo, RoleChangeReason.None);
                curTeamSpyList.Remove(curAttacker);
                curAttacker.ReceiveHint("<align=center><voffset=28em> <color=#F6511D> You've been revealed!!! </color></voffset></align>");
                
                curTarget.ReferenceHub.roleManager.ServerSetRole(newEnemyRoleToSwapTo, RoleChangeReason.None);
                curTeamSpyList.Remove(curAttacker);
                curTarget.ReceiveHint("<align=center><voffset=28em> <color=#F6511D> You've been revealed!!! </color></voffset></align>");
                
                return RevealStatus.SpiesWereRevealed;
            }

            // If the current attacker is spy attacking their "fake teammate", then reveals them.
            if (curTeamSpyList.Contains(curAttacker))
            {
                Log.Debug($"Changing current player role from {curAttacker.ReferenceHub.roleManager.CurrentRole} to {newRoleToSwapTo} ", SpyUtilityNW.Instance.Config.Debug);
                curAttacker.ReferenceHub.roleManager.ServerSetRole(newRoleToSwapTo, RoleChangeReason.None);
                curTeamSpyList.Remove(curAttacker);
                curAttacker.ReceiveHint("<align=center><voffset=28em> <color=#F6511D> You've been revealed!!! </color></voffset></align>");
                return RevealStatus.SpyWasRevealed;
            }
            
            return RevealStatus.AllowNormalDamage;
        }

        [PluginEvent(ServerEventType.PlayerChangeRole)]
        public void OnRoleChange( IPlayer curPlayer, PlayerRoleBase curRoleBase, RoleTypeId curRoleId, RoleChangeReason curRoleChangeReason)
        {
            //Log.Info($"PlayerChangeRole was called curPlayer {curPlayer}, curRoleBase {curRoleBase} curRoleId {curRoleId} curRoleChangeReason {curRoleChangeReason}");
            if (curRoleChangeReason is RoleChangeReason.RemoteAdmin)
            {
                RemoveFromSpies(curPlayer);
            }
        }

        [PluginEvent(ServerEventType.PlayerDeath)]
        public void OnPlayerDeath(IPlayer attacker, IPlayer target, DamageHandlerBase damageHandler)
        {
            Log.Debug($"OnPlayerDeath attacker {attacker?.ReferenceHub}, and target {target?.ReferenceHub}" +
                      $"and what are the roles? \n" +
                      $"\nattacker.ReferenceHub.roleManager.CurrentRole.Team {attacker?.ReferenceHub?.roleManager.CurrentRole.Team}" +
                      $"\ntarget.ReferenceHub.roleManager.CurrentRole.Team { target?.ReferenceHub?.roleManager.CurrentRole.Team}", SpyUtilityNW.Instance.Config.Debug);
            
            RemoveFromSpies(target);
        }

        private static void RemoveFromSpies(IPlayer target)
        {
            Player potentialSpy = Player.Get(target.ReferenceHub.netId);
            currentCISpies.Remove(potentialSpy);
            currentMtfSpies.Remove(potentialSpy);
        }
        
        private static void RemoveFromSpies(IPlayer target, Team team)
        {
            Player potentialSpy = Player.Get(target.ReferenceHub.netId);
            switch (team)
            {
                case Team.ChaosInsurgency:
                    currentCISpies.Remove(potentialSpy);
                    break;
                case Team.FoundationForces:
                    currentMtfSpies.Remove(potentialSpy);
                    break;
            }
        }

        [PluginEvent(ServerEventType.PlayerLeft)]
        public void OnPlayerLeave(IPlayer leavingPlayer)
        {
            RemoveFromSpies(leavingPlayer);
        }

        public static void ForceRemoveSpy(Player player, Team team)
        {
            RemoveFromSpies(player, team);
        }

        public static bool ForceCreateSpy(Player player, Team team, RoleTypeId newRoleID = RoleTypeId.None)
        {
            RemoveFromSpies(player);
            if (newRoleID is RoleTypeId.None)
            {
                newRoleID = team switch
                {
                    Team.ChaosInsurgency => RoleTypeId.NtfSpecialist,
                    Team.FoundationForces => RoleTypeId.ChaosRifleman,
                    _ => newRoleID
                };
            }

            player.ReferenceHub.roleManager.ServerSetRole(newRoleID, RoleChangeReason.None);
            Log.Info($"Settings role {newRoleID} for player {player}, on team {team}");
            return ForceAddSpy(player, team);
        }
    }
}