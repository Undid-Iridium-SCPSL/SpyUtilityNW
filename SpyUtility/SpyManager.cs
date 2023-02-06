using System.Collections.Generic;
using PluginAPI.Core;
using Respawning;
using System.Linq;
using InventorySystem;
using InventorySystem.Items.Firearms;
using InventorySystem.Items.Firearms.Attachments;
using JetBrains.Annotations;
using MEC;
using PlayerRoles;
using PlayerStatsSystem;
using PluginAPI.Core.Attributes;
using PluginAPI.Core.Interfaces;
using PluginAPI.Enums;
using SpyUtilityNW.Events;
using SpyUtilityNW.Spies;
using UnityEngine;
using Utils.NonAllocLINQ;
using Random = UnityEngine.Random;

namespace SpyUtilityNW
{
    /// <summary>
    /// Spy manager for all spies.
    /// </summary>
    [UsedImplicitly]
    public class SpyManager
    {
        /// <summary>
        /// Setups spy manager instance, and associated structures
        /// </summary>
        public SpyManager()
        {
            SpyUtilityNW.Instance.SpyManager = this;
            PluginInstance = SpyUtilityNW.Instance;
            Log.Debug("This spawned right", SpyUtilityNW.Instance.Config.Debug);
        }

        /// <summary>
        /// TODO Actually implement and use this
        /// </summary>
        public static IDictionary<Utility.CustomTeams, ISet<Player>> AllCurrentSpies { get; set; } =
            new Dictionary<Utility.CustomTeams, ISet<Player>>
            {
                { Utility.CustomTeams.Chaos, new HashSet<Player>() },
                { Utility.CustomTeams.Mtf,  new HashSet<Player>() },
                { Utility.CustomTeams.Guards, new HashSet<Player>() }
            };
        
        private SpyUtilityNW PluginInstance { get; set; }

        /// <summary>
        /// Handles TeamRspawn for CISpy
        /// </summary>
        /// <param name="respawningEvent"></param>
        
        public void OnWaveSpawn(InitalWaveSpawnEvent initalWaveSpawnEvent)
        {
            Log.Debug(
                $"InitialWaveSpawnEvent OnWaveSpawn", SpyUtilityNW.Instance.Config.Debug);
            HashSet<Player> potentialSpies = new HashSet<Player>();
            List<ReferenceHub> initialPlayers = initalWaveSpawnEvent.InitalWavePlayers[RoleTypeId.FacilityGuard];
            int totalPlayers = initialPlayers.Count;

            if (totalPlayers < 1)
            {
                return;
            }
            
            if (SpyUtilityNW.Instance.Config.MinimumPlayers > initialPlayers.Count)
            {
                return;
            }
            
            // For the future, maybe
            foreach (RoleTypeId roleTypeId in initalWaveSpawnEvent.rolesAdded)
            {
                switch (roleTypeId)
                {
                    case RoleTypeId.FacilityGuard:
                    {
                        var howManyGuardCISpies = 0;
                        foreach (var keyValuePair in PluginInstance.Config.probabilityOfGuardSpy)
                        {
                            Log.Debug($"Probability of guard ci spy {keyValuePair.Key} and {keyValuePair.Value}", SpyUtilityNW.Instance.Config.Debug);
                            if (Random.Range(0, 100) < keyValuePair.Value)
                            {
                                Log.Debug($"Probability of guard ci spy was lucky, adding spy {howManyGuardCISpies}", SpyUtilityNW.Instance.Config.Debug);
                                howManyGuardCISpies++;
                            }
                        } 
                    
                        TryToAddSpies(potentialSpies, new TeamRespawnEvent(initialPlayers, SpawnableTeamType.None), totalPlayers, howManyGuardCISpies, AllCurrentSpies[Utility.CustomTeams.Guards]);
                        foreach (Player potentialSpy in potentialSpies)
                        {
                            if (AllCurrentSpies[Utility.CustomTeams.Chaos].Add(potentialSpy))
                            {
                                ChangeSpawnInventory(potentialSpy, SpyUtilityNW.Instance.Config.GuardSpyLoadout, Utility.CustomTeams.Chaos);
                                AllCurrentSpies[Utility.CustomTeams.Guards].Add(potentialSpy);
                            }
                        }
                        break;
                    }
                    case RoleTypeId.None:
                    case RoleTypeId.Scp173:
                    case RoleTypeId.ClassD:
                    case RoleTypeId.Spectator:
                    case RoleTypeId.Scp106:
                    case RoleTypeId.NtfSpecialist:
                    case RoleTypeId.Scp049:
                    case RoleTypeId.Scientist:
                    case RoleTypeId.Scp079:
                    case RoleTypeId.ChaosConscript:
                    case RoleTypeId.Scp096:
                    case RoleTypeId.Scp0492:
                    case RoleTypeId.NtfSergeant:
                    case RoleTypeId.NtfCaptain:
                    case RoleTypeId.NtfPrivate:
                    case RoleTypeId.Tutorial:
                    case RoleTypeId.Scp939:
                    case RoleTypeId.CustomRole:
                    case RoleTypeId.ChaosRifleman:
                    case RoleTypeId.ChaosRepressor:
                    case RoleTypeId.ChaosMarauder:
                    case RoleTypeId.Overwatch:
                    default:
                        return;
                }
            }
            

            if (SpyUtilityNW.Instance.Config.HideAllUnitNamesForRespawningTeam)
            {
                Timing.CallDelayed(SpyUtilityNW.Instance.Config.HideAllUnitNamesForRespawningTeamDelay, () =>
                {
                    foreach (ReferenceHub eventRespawningPlayer in initialPlayers)
                    {
                        Player.Get(eventRespawningPlayer).PlayerInfo.IsUnitNameHidden = true;
                    }
                });
            }
        }
        /// <summary>
        /// Handles TeamRespawn for CISpy/Mtf spy
        /// </summary>
        /// <param name="respawningEvent"></param>
        public void OnWaveSpawn(TeamRespawnEvent respawningEvent)
        {
            Log.Debug(
                $"What is TeamRespawnEvent spawnTeamType {respawningEvent.SpawningTeam == SpawnableTeamType.NineTailedFox} and blah", SpyUtilityNW.Instance.Config.Debug);

            var potentialSpies = new HashSet<Player>();
            int totalPlayers = respawningEvent.RespawningPlayers.Count;

            if (respawningEvent.RespawningPlayers.Count < 1)
            {
                return;
            }

            if (SpyUtilityNW.Instance.Config.MinimumPlayers > respawningEvent.RespawningPlayers.Count)
            {
                return;
            }
            
            switch (respawningEvent.SpawningTeam)
            {
                case SpawnableTeamType.ChaosInsurgency:
                {
                    var howManyMtfSpies = 0;
                    foreach (var keyValuePair in PluginInstance.Config.probabilityOfMtfSpy)
                    {
                        Log.Debug($"Probability of ci spy {keyValuePair.Key} and {keyValuePair.Value}", SpyUtilityNW.Instance.Config.Debug);
                        if (Random.Range(0, 100) < keyValuePair.Value)
                        {
                            Log.Debug($"Probability of spy was lucky, adding spy {howManyMtfSpies}", SpyUtilityNW.Instance.Config.Debug);
                            howManyMtfSpies++;
                        }
                    } 
                
                    TryToAddSpies(potentialSpies, respawningEvent, totalPlayers, howManyMtfSpies, AllCurrentSpies[Utility.CustomTeams.Mtf]);
                    foreach (Player potentialSpy in potentialSpies)
                    {
                         if (AllCurrentSpies[Utility.CustomTeams.Mtf].Add(potentialSpy))
                         {
                             ChangeSpawnInventory(potentialSpy, SpyUtilityNW.Instance.Config.MtfSpyLoadout, Utility.CustomTeams.Mtf);
                         }
                    }
                    break;
                }
                case SpawnableTeamType.NineTailedFox:
                {
                    var howManyCiSpies = 0;
                    foreach (var keyValuePair in PluginInstance.Config.probabilityOfCISpy) 
                    { 
                        Log.Debug($"Probability of ci spy {keyValuePair.Key} and {keyValuePair.Value}", SpyUtilityNW.Instance.Config.Debug);
                        if (Random.Range(0, 100) < keyValuePair.Value)
                        {
                            Log.Debug($"Probability of spy was lucky, adding spy {howManyCiSpies} - CI SPY add", SpyUtilityNW.Instance.Config.Debug);
                            howManyCiSpies++;
                        }
                    } 
                    TryToAddSpies(potentialSpies, respawningEvent, totalPlayers, howManyCiSpies, AllCurrentSpies[Utility.CustomTeams.Chaos]);
                    // Technically this can be reduced in TryToAddSpies to just add to static but I feel that is not safe.
                    foreach (Player potentialSpy in potentialSpies)
                    {
                        if (AllCurrentSpies[Utility.CustomTeams.Chaos].Add(potentialSpy))
                        {
                            ChangeSpawnInventory(potentialSpy, SpyUtilityNW.Instance.Config.CiSpyLoadout, Utility.CustomTeams.Chaos);
                        }
                    }

                    break;
                }
                case SpawnableTeamType.None:
                    break;
                default:
                    return;
            }

            if (SpyUtilityNW.Instance.Config.HideAllUnitNamesForRespawningTeam)
            {
                Timing.CallDelayed(SpyUtilityNW.Instance.Config.HideAllUnitNamesForRespawningTeamDelay, () =>
                {
                    foreach (Player eventRespawningPlayer in respawningEvent.RespawningPlayers)
                    {
                        eventRespawningPlayer.PlayerInfo.IsUnitNameHidden = true;
                    }
                });
            }
        }

        private static void ChangeSpawnInventory(Player potentialSpy, SpyBase currentSpyLoadout, Utility.CustomTeams argTeam)
        {
             Timing.CallDelayed(SpyUtilityNW.Instance.Config.RespawnChangeRoleDelay, () => {
                 var potentialSpyPosition = potentialSpy.Position;
                 Timing.CallDelayed(SpyUtilityNW.Instance.Config.RespawnChangeRoleDelay, () =>
                 {
                     potentialSpy.ReferenceHub.roleManager.ServerSetRole(currentSpyLoadout.SpyRealRole,
                         RoleChangeReason.None);
                    
                     Timing.CallDelayed(SpyUtilityNW.Instance.Config.RespawnChangeRoleItemsDelay, () =>
                     {
                         Vector3 roleLoadoutVector = new Vector3(currentSpyLoadout.SpawnPosition[0],
                             currentSpyLoadout.SpawnPosition[1],currentSpyLoadout.SpawnPosition[2]);
                         potentialSpy.Position = roleLoadoutVector == Vector3.zero ? potentialSpyPosition : roleLoadoutVector;
                         potentialSpy.ReferenceHub.inventory.UserInventory.Items.Clear();
                         potentialSpy.ReferenceHub.inventory.UserInventory.ReserveAmmo.Clear();
                         
                         foreach (var playerItem in currentSpyLoadout.SpyStartingItems)
                         {
                             var item = potentialSpy.ReferenceHub.inventory.ServerAddItem(playerItem);
                             if (item is Firearm curFireArm)
                             {
                                 if (AttachmentsServerHandler.PlayerPreferences.TryGetValue(potentialSpy.ReferenceHub, out Dictionary<ItemType, uint> data)
                                     && data.TryGetValue(item.ItemTypeId, out uint num))
                                 {
                                     curFireArm.ApplyAttachmentsCode(num, true);
                                 }
                                 curFireArm.Status = new FirearmStatus(curFireArm.AmmoManagerModule.MaxAmmo, curFireArm.Status.Flags, curFireArm.GetCurrentAttachmentsCode());
                             }
                         }
                         
                         foreach (var pairedAmmoTypeAndQty in currentSpyLoadout.SpyStartingAmmo)
                         {
                             potentialSpy.ReferenceHub.inventory.ServerAddAmmo(pairedAmmoTypeAndQty.Key, pairedAmmoTypeAndQty.Value);
                         }
                         potentialSpy.ReferenceHub.inventory.SendItemsNextFrame = true;
                         ChangeAppearance(potentialSpy, currentSpyLoadout.SpyFakeRole);
                         var newSpyMessage = string.Format(SpyUtilityNW.Instance.Config.OnSpySpawnMessage,
                             argTeam);
                         potentialSpy.ReceiveHint(newSpyMessage,
                             SpyUtilityNW.Instance.Config.OnSpySpawnMessageHintDuration);

                         if (SpyUtilityNW.Instance.Config.HideAllUnitNamesForSpies)
                         {
                             Timing.CallDelayed(.1f, () => { potentialSpy.PlayerInfo.IsUnitNameHidden = true; });
                         }

                         if (SpyUtilityNW.Instance.Config.ExtraChangeAppearance)
                         {
                             Timing.CallDelayed(SpyUtilityNW.Instance.Config.ExtraChangeAppearanceDelay, () =>
                             {
                                 ChangeAppearance(potentialSpy, currentSpyLoadout.SpyFakeRole);
                             });
                         }
                     });
                 });
             });
        }

        /// <summary>
        /// Try to add <see cref="howManySpies"/> spies to the list of spies to be spawned
        /// </summary>
        /// <param name="players">Spawning players <see cref="ReferenceHub"/></param>
        /// <param name="respawningEvent">Respawn Event <see cref="TeamRespawnEvent"/></param>
        /// <param name="totalPlayers">Amount of players available</param>
        /// <param name="howManySpies">How many spies to create, if possible</param>
        private static void TryToAddSpies(ISet<Player> players, TeamRespawnEvent respawningEvent,
            int totalPlayers, int howManySpies, ISet<Player> spyDictionary)
        {
            for (var pos = 0; pos < howManySpies; pos++)
            {
                int attempt = 0;
                bool unableToFind = false;
                    
                Player curPlayerToMakeSpy =
                    respawningEvent.RespawningPlayers.ElementAt(Random.Range(0, totalPlayers));
                
                Log.Debug($"Found potential spy {curPlayerToMakeSpy.Nickname}", SpyUtilityNW.Instance.Config.Debug);
                while (players.Contains(curPlayerToMakeSpy) || spyDictionary.Contains(curPlayerToMakeSpy))
                {
                    if (attempt >= 3)
                    {
                        Log.Debug($"After 3 attempts, no spy was found. Skipping spy", SpyUtilityNW.Instance.Config.Debug);
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

        /// <summary>
        /// Forcefully adds spy to current spy dictionary based on type.
        /// </summary>
        /// <param name="player"></param>
        /// <param name="team"></param>
        /// <returns></returns>
        public static bool ForceAddSpy(Player player, Utility.CustomTeams team)
        {
            switch (team)
            {
                case Utility.CustomTeams.Chaos:
                    AllCurrentSpies[Utility.CustomTeams.Chaos].Add(player);
                    break;
                case Utility.CustomTeams.Guards:
                    AllCurrentSpies[Utility.CustomTeams.Chaos].Add(player);
                    AllCurrentSpies[Utility.CustomTeams.Guards].Add(player);
                    break;
                case Utility.CustomTeams.Mtf:
                    AllCurrentSpies[Utility.CustomTeams.Mtf].Add(player);
                    break;
                default:
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Reveal status enumeration, allows specific behaviors based on type.
        /// </summary>
        // ReSharper disable once MemberCanBePrivate.Global
        public enum RevealStatus
        {
            /// <summary>
            /// If the spy was revealed, we allow damage and change their appearance back.
            /// </summary>
            SpyWasRevealed,
            /// <summary>
            /// If two spies on separate teams attacked each other.
            /// </summary>
            SpiesWereRevealed,
            /// <summary>
            /// If a Spy damaged a fellow team Spy
            /// </summary>
            SpyDamagedFriendlySpy,
            /// <summary>
            /// Allow event damage to go through.
            /// </summary>
            AllowNormalDamage,
            /// <summary>
            /// Prevent event damage due to attacking teammate.
            /// </summary>
            SpyAttackingTeammate,
            /// <summary>
            /// Prevent damage all together.
            /// </summary>
            RejectDamage
        }

        [PluginEvent(ServerEventType.RoundEnd)]
        [UsedImplicitly]
        void OnRoundEnd(RoundSummary.LeadingTeam curLeadingTeam)
        {
            RoundEndAllowAllDamage = true;
            RevealAllSpies();
            AllCurrentSpies.Clear();
        }
        
        [PluginEvent(ServerEventType.PlayerHandcuff)]
        [UsedImplicitly]
        void OnCuffing(IPlayer PlayerCuffing, IPlayer PlayerGettingCuffed)
        {
            if (SpyUtilityNW.Instance.Config.RevealOnCuffActions)
            {
                Log.Debug($"OnCuffing, prior to calling OnPlayerDamage.", SpyUtilityNW.Instance.Config.Debug);
                OnPlayerDamage(PlayerCuffing, PlayerGettingCuffed, null);
            }
        }
        
        [PluginEvent(ServerEventType.PlayerRemoveHandcuffs)]
        [UsedImplicitly]
        void OnRemovingCuff(IPlayer PlayerCuffing, IPlayer PlayerGettingCuffed)
        {
            if (SpyUtilityNW.Instance.Config.RevealOnCuffActions)
            {
                Log.Debug($"OnRemovingCuff, prior to calling OnPlayerDamage.", SpyUtilityNW.Instance.Config.Debug);
                OnPlayerDamage(PlayerCuffing, PlayerGettingCuffed, null);
            }
        }

        private bool RoundEndAllowAllDamage { get; set; }

        [PluginEvent(ServerEventType.PlayerDamage)]
        // ReSharper disable once UnusedMethodReturnValue.Local
        // ReSharper disable once UnusedParameter.Local
        private bool OnPlayerDamage(IPlayer target, IPlayer attacker, DamageHandlerBase damageHandler)
        {
            Log.Debug($"OnPlayerDamage, prior to target/attacker checks.", SpyUtilityNW.Instance.Config.Debug);

            if (target == attacker)
            {
                return true;
            }

            if (target == null || attacker == null)
            {
                return true;
            }
            
            Log.Debug($"\nOn player damaged attacker {attacker.ReferenceHub}, and target {target.ReferenceHub} and what are the roles? " + 
                      $"\nattacker.ReferenceHub.roleManager.CurrentRole.Team {attacker.ReferenceHub.roleManager.CurrentRole.Team}" +
                      $"\ntarget.ReferenceHub.roleManager.CurrentRole.Team { target.ReferenceHub.roleManager.CurrentRole.Team}", SpyUtilityNW.Instance.Config.Debug);

            Player host = Player.Get(ReferenceHub.HostHub);
            if (attacker == host || target == host)
            {
                Log.Debug($"Server host was either attacker {attacker == host} or target {target == host}", SpyUtilityNW.Instance.Config.Debug);
                return true;
            }

            if (RoundEndAllowAllDamage)
            {
                return true;
            }
            
            Team attackerTeam = attacker.ReferenceHub.roleManager.CurrentRole.Team;
            Team targetTeam = target.ReferenceHub.roleManager.CurrentRole.Team;
            Player curAttacker = Player.Get(attacker.ReferenceHub.netId);
            Player curTarget = Player.Get(target.ReferenceHub.netId);

           
            RevealStatus wasCiSpyRevealed = CheckIfCiSpyReveal(attackerTeam, targetTeam, curAttacker, curTarget, AllCurrentSpies[Utility.CustomTeams.Chaos], AllCurrentSpies[Utility.CustomTeams.Mtf]);
            Log.Debug($"wasCISpyRevealed result { wasCiSpyRevealed }", SpyUtilityNW.Instance.Config.Debug);
            switch (wasCiSpyRevealed)
            {
                case RevealStatus.AllowNormalDamage:
                    break;
                case RevealStatus.SpyDamagedFriendlySpy:
                case RevealStatus.SpyAttackingTeammate:
                    return false;
                case RevealStatus.SpyWasRevealed:
                case RevealStatus.SpiesWereRevealed:
                    return true;
                case RevealStatus.RejectDamage:
                    return false;
            }
            RevealStatus wasMtfSpyRevealed = CheckIfMtfSpyReveal(attackerTeam, targetTeam, curAttacker, curTarget, AllCurrentSpies[Utility.CustomTeams.Mtf], AllCurrentSpies[Utility.CustomTeams.Chaos]);
            Log.Debug($"wasMtfSpyRevealed result { wasMtfSpyRevealed }", SpyUtilityNW.Instance.Config.Debug);
            switch (wasMtfSpyRevealed)
            {
                case RevealStatus.AllowNormalDamage:
                    break;
                case RevealStatus.SpyDamagedFriendlySpy:
                case RevealStatus.SpyAttackingTeammate:
                    return false;
                case RevealStatus.SpyWasRevealed:
                case RevealStatus.SpiesWereRevealed:
                    return true;
                case RevealStatus.RejectDamage:
                    return false;
            }
            
            return true;
        }

        private RevealStatus CheckIfMtfSpyReveal(Team attackerTeam, Team targetTeam, Player curAttacker,
            Player curTarget, ISet<Player> mtfSpies, ISet<Player> enemySpies)
        {
            // If spy has not been revealed yet, reject damage.
            if (attackerTeam is Team.FoundationForces or Team.Scientists && AllCurrentSpies[Utility.CustomTeams.Chaos].Contains(curTarget) && !mtfSpies.Contains(curAttacker))
            {
                Log.Debug($"FoundationForces Spy has not been revealed yet, reject damage. CheckIfMtfSpyReveal", SpyUtilityNW.Instance.Config.Debug);
                return RevealStatus.RejectDamage;
            }
            
            // If both are on the same team, possibly reveals
            if (targetTeam is Team.ChaosInsurgency or Team.ClassD)
            {
                Log.Debug($"targetTeam was ChaosInsurgency forces. CheckIfMtfSpyReveal", SpyUtilityNW.Instance.Config.Debug);
                return RevealRoleIfNeeded(curAttacker, curTarget, mtfSpies, 
                    SpyUtilityNW.Instance.Config.MtfSpyLoadout.SpyRealRole,
                    enemySpies, 
                    SpyUtilityNW.Instance.Config.CiSpyLoadout.SpyRealRole);
            }

            // If we're both spies, normal damage
            if ((AllCurrentSpies[Utility.CustomTeams.Chaos].Contains(curAttacker) && mtfSpies.Contains(curTarget)) || 
                (mtfSpies.Contains(curAttacker) && AllCurrentSpies[Utility.CustomTeams.Chaos].Contains(curTarget)))
            {
                Log.Debug($"If we're both spies, normal damage. CheckIfMtfSpyReveal", SpyUtilityNW.Instance.Config.Debug);
                return RevealStatus.AllowNormalDamage;
            }
            
            // If target is MTF, and Chaos agent is MTF spy, do no damage
            if (targetTeam is Team.FoundationForces or Team.Scientists && mtfSpies.Contains(curAttacker))
            {
                Log.Debug($"If target is MTF, and Chaos agent is MTF spy, do no damage. CheckIfMtfSpyReveal", SpyUtilityNW.Instance.Config.Debug);
                var attackingTeammateOnSameTeam = string.Format(SpyUtilityNW.Instance.Config.OnSpyAttackingTeammate,
                    Team.FoundationForces);
                curAttacker.ReceiveHint(attackingTeammateOnSameTeam, SpyUtilityNW.Instance.Config.OnSpyAttackingTeammateHintDuration);
                return RevealStatus.SpyAttackingTeammate;
            }
            
            // If attacker is MTF, and the target is Chaos agent who is MTF spy, do no damage.
            if (attackerTeam is Team.FoundationForces or Team.Scientists && mtfSpies.Contains(curTarget))
            {
                Log.Debug($"If attacker is MTF, and the target is Chaos agent who is MTF spy, do no damage. CheckIfMtfSpyReveal", SpyUtilityNW.Instance.Config.Debug);
                var attackingSpyOnSameTeam = string.Format(SpyUtilityNW.Instance.Config.OnTeammateAttackingSpy,
                    Team.FoundationForces);
                curAttacker.ReceiveHint(attackingSpyOnSameTeam, SpyUtilityNW.Instance.Config.OnTeammateAttackingSpyHintDuration);
                return RevealStatus.SpyAttackingTeammate;
            }

            Log.Debug($"Default path. CheckIfMtfSpyReveal", SpyUtilityNW.Instance.Config.Debug);
            // Normal damage path
            return RevealStatus.AllowNormalDamage;
        }

        private static RevealStatus CheckIfCiSpyReveal(Team attackerTeam, Team targetTeam, Player curAttacker,
            Player curTarget, ISet<Player> currentCiSpies, ISet<Player> enemySpies)
        {
            // If spy has not been revealed yet, reject damage.
            if (attackerTeam is Team.ChaosInsurgency or Team.ClassD && enemySpies.Contains(curTarget)&& !currentCiSpies.Contains(curAttacker))
            {
                Log.Debug($"Spy has not been revealed yet, reject damage. CheckIfCiSpyReveal", SpyUtilityNW.Instance.Config.Debug);
                return RevealStatus.RejectDamage;
            }
            
            //If on same team, identify if we're spies, and whether we can do damage to each other
            if (targetTeam is Team.FoundationForces or Team.Scientists)
            {
                Log.Debug($"targetTeam was foundation forces CheckIfCiSpyReveal", SpyUtilityNW.Instance.Config.Debug);

                return RevealRoleIfNeeded(curAttacker, curTarget, currentCiSpies, 
                    SpyUtilityNW.Instance.Config.CiSpyLoadout.SpyRealRole,
                    enemySpies, 
                    SpyUtilityNW.Instance.Config.MtfSpyLoadout.SpyRealRole);
            }
            
            // Spies do not reveal each other and do normal damage.
            if ((currentCiSpies.Contains(curAttacker) && enemySpies.Contains(curTarget)) || 
                (enemySpies.Contains(curAttacker) && currentCiSpies.Contains(curTarget)))
            {
                Log.Debug($"Spies do not reveal each other and do normal damage.foundation forces CheckIfCiSpyReveal", SpyUtilityNW.Instance.Config.Debug);
                return RevealStatus.AllowNormalDamage;
            }

            // If I am a CI Spy attacking Chaos, I do no damage
            if (targetTeam is Team.ChaosInsurgency or Team.ClassD && currentCiSpies.Contains(curAttacker))
            {
                Log.Debug($"If I am a CI Spy attacking Chaos, I do no damage. CheckIfCiSpyReveal", SpyUtilityNW.Instance.Config.Debug);
                var attackingTeammateOnSameTeam = string.Format(SpyUtilityNW.Instance.Config.OnSpyAttackingTeammate,
                    Team.ChaosInsurgency);
                curAttacker.ReceiveHint(attackingTeammateOnSameTeam, SpyUtilityNW.Instance.Config.OnSpyAttackingTeammateHintDuration);
                return RevealStatus.SpyAttackingTeammate;
            }
            
            // If I am chaos attacking CI spy, I do no damage
            if (attackerTeam is Team.ChaosInsurgency or Team.ClassD && currentCiSpies.Contains(curTarget))
            {
                Log.Debug($"If I am chaos attacking CI spy, I do no damage. CheckIfCiSpyReveal", SpyUtilityNW.Instance.Config.Debug);
                var attackingTeammateOnSameTeam = string.Format(SpyUtilityNW.Instance.Config.OnTeammateAttackingSpy,
                    Team.ChaosInsurgency);
                curAttacker.ReceiveHint(attackingTeammateOnSameTeam, SpyUtilityNW.Instance.Config.OnTeammateAttackingSpyHintDuration);
                return RevealStatus.SpyAttackingTeammate;
            }
         
            Log.Debug($"Default path. CheckIfCiSpyReveal", SpyUtilityNW.Instance.Config.Debug);
            // Normal damage path
            return RevealStatus.AllowNormalDamage;
        }

        private static RevealStatus RevealRoleIfNeeded(Player curAttacker, Player curTarget,
            ICollection<Player> curTeamSpyList,
            RoleTypeId newRoleToSwapTo,
            ICollection<Player> curEnemyTeamList,
            RoleTypeId newEnemyRoleToSwapTo)
        {
            //If the attacker and target are both spies for same team
            if (curTeamSpyList.Contains(curAttacker) && curTeamSpyList.Contains(curTarget))
            {
                Log.Debug($"Both attacker and target were spies for the same team {curAttacker.Nickname} and {curTarget.Nickname}", SpyUtilityNW.Instance.Config.Debug);
                curAttacker.ReceiveHint(SpyUtilityNW.Instance.Config.SameTeamSpyMessage, SpyUtilityNW.Instance.Config.SameTeamSpyMessageHintDuration);
                return RevealStatus.SpyDamagedFriendlySpy;
            }
            //If attacker and target are spies for enemy teams (reveal both)
            if ( (curTeamSpyList.Contains(curAttacker) && curEnemyTeamList.Contains(curTarget)) ||
                 (curTeamSpyList.Contains(curTarget) && curEnemyTeamList.Contains(curAttacker)) )
            {
                
                Log.Debug($"Both attacker and target were spies {curAttacker.Nickname} and {curTarget.Nickname}", SpyUtilityNW.Instance.Config.Debug);
                if (curAttacker.ReferenceHub.roleManager.CurrentRole.Team ==
                    curTarget.ReferenceHub.roleManager.CurrentRole.Team)
                {
                    Log.Debug($"Both attacker and target were spies but on the same role team, not same spy team. {curAttacker.Nickname} and {curTarget.Nickname}", SpyUtilityNW.Instance.Config.Debug);
                    return RevealStatus.RejectDamage;
                }
                // curAttacker.ReferenceHub.roleManager.ServerSetRole(newRoleToSwapTo, RoleChangeReason.None);
                RemoveFromSpies(curAttacker);
                ChangeAppearance(curAttacker, newRoleToSwapTo);
                curAttacker.ReceiveHint(SpyUtilityNW.Instance.Config.SpyHasBeenRevealed, SpyUtilityNW.Instance.Config.SpyHasBeenRevealedHintDuration);
                
                //curTarget.ReferenceHub.roleManager.ServerSetRole(newEnemyRoleToSwapTo, RoleChangeReason.None);
                RemoveFromSpies(curTarget);
                ChangeAppearance(curTarget, newEnemyRoleToSwapTo);
                curAttacker.ReceiveHint(SpyUtilityNW.Instance.Config.SpyHasBeenRevealed, SpyUtilityNW.Instance.Config.SpyHasBeenRevealedHintDuration);
                Log.Debug($"SpiesWereRevealed {curAttacker.Nickname} and {curTarget.Nickname} in both are spies logic", SpyUtilityNW.Instance.Config.Debug);
                return RevealStatus.SpiesWereRevealed;
            }
            // If the current attacker is spy attacking their "fake teammate", then reveals them.
            if (curTeamSpyList.Contains(curAttacker))
            {
                Log.Debug($"Changing current player role from {curAttacker.ReferenceHub.roleManager.CurrentRole} to {newRoleToSwapTo} ", SpyUtilityNW.Instance.Config.Debug);
                //curAttacker.ReferenceHub.roleManager.ServerSetRole(newRoleToSwapTo, RoleChangeReason.None);
                ChangeAppearance(curAttacker, newRoleToSwapTo);
                curTeamSpyList.Remove(curAttacker);
                curAttacker.ReceiveHint(SpyUtilityNW.Instance.Config.SpyHasBeenRevealed, SpyUtilityNW.Instance.Config.SpyHasBeenRevealedHintDuration);
                return RevealStatus.SpyWasRevealed;
            }
            Log.Debug($"RevealRoleIfNeeded allow normal damage: {curAttacker.Nickname} and {curTarget.Nickname}", SpyUtilityNW.Instance.Config.Debug);
            return RevealStatus.AllowNormalDamage;
        }

        /// <summary>
        /// If a player changes role due to remote admin, we will wipe them from spy database.
        /// </summary>
        /// <param name="curPlayer"></param>
        /// <param name="curRoleBase"></param>
        /// <param name="curRoleId"></param>
        /// <param name="curRoleChangeReason"></param>
        [PluginEvent(ServerEventType.PlayerChangeRole)]
        [UsedImplicitly]
        public void OnRoleChange( IPlayer curPlayer, PlayerRoleBase curRoleBase, RoleTypeId curRoleId, RoleChangeReason curRoleChangeReason)
        {
            Log.Info("UH HEY");
            Log.Info($"{curPlayer},");
            Log.Info($", {curPlayer.ReferenceHub.roleManager.CurrentRole}, ");
            Log.Info($",  {RoleChangeReason.LateJoin}, ");
            Log.Info($", {RoleChangeReason.Respawn}, ");
            if (curRoleChangeReason is RoleChangeReason.RemoteAdmin)
            {
                RemoveFromSpies(curPlayer);
            }
        }

        /// <summary>
        /// If Player dies, we remove them from spy database.
        /// </summary>
        /// <param name="attacker"></param>
        /// <param name="target"></param>
        /// <param name="damageHandler"></param>
        [PluginEvent(ServerEventType.PlayerDeath)]
        public void OnPlayerDeath(IPlayer attacker, IPlayer target, DamageHandlerBase damageHandler)
        {
            Log.Debug($"OnPlayerDeath", SpyUtilityNW.Instance.Config.Debug);

            if (target == null)
            {
                return;
            }

            if (attacker != null)
            {
                Log.Debug($"OnPlayerDeath attacker {attacker.ReferenceHub}, and target {target.ReferenceHub}" +
                          $"and what are the roles? \n" +
                          $"\nattacker.ReferenceHub.roleManager.CurrentRole.Team {attacker.ReferenceHub.roleManager.CurrentRole.Team}" +
                          $"\ntarget.ReferenceHub.roleManager.CurrentRole.Team {target.ReferenceHub.roleManager.CurrentRole.Team}",
                    SpyUtilityNW.Instance.Config.Debug);
            }

            if (SpyUtilityNW.Instance.Config.RevealIfSolo)
            {
                bool atLeastOneScp = false;
                HashSet<Player> chaosPlayers = new HashSet<Player>();
                HashSet<Player> mtfPlayers = new HashSet<Player>();
                foreach (Player player in Player.GetPlayers())
                {
                    if (player.IsSCP)
                    {
                        atLeastOneScp = true;
                    }

                    if (player.IsChaos)
                    {
                        if (!AllCurrentSpies[Utility.CustomTeams.Chaos].Contains(player))
                        {
                            chaosPlayers.Append(player);
                        }
                    }

                    if (player.IsNTF)
                    {
                        if (!AllCurrentSpies[Utility.CustomTeams.Mtf].Contains(player))
                        {
                            mtfPlayers.Append(player);
                        }
                    }
                }

                if (!atLeastOneScp)
                {
                    if (!chaosPlayers.IsEmpty())
                    {
                        RevealSpies(Utility.CustomTeams.Chaos);
                    }

                    if (!mtfPlayers.IsEmpty())
                    {
                        RevealSpies(Utility.CustomTeams.Mtf);
                    }
                }
                // if (Player.GetPlayers().Count(player => player.IsSCP) <= 0)
                // {
                //     
                // }
            }

            RemoveFromSpies(target);
        }

        /// <summary>
        /// Reveals all current spies, and changes their appearance. 
        /// </summary>
        public void RevealAllSpies()
        {
            foreach (Player player in Player.GetPlayers())
            {
                if (AllCurrentSpies[Utility.CustomTeams.Chaos].Contains(player))
                {
                    ChangeAppearance(player, SpyUtilityNW.Instance.Config.CiSpyLoadout.SpyRealRole);
                }
                else if (AllCurrentSpies[Utility.CustomTeams.Mtf].Contains(player))
                {
                    ChangeAppearance(player, SpyUtilityNW.Instance.Config.MtfSpyLoadout.SpyRealRole);
                }
                RemoveFromSpies(player);

            }
        }

        /// <summary>
        /// Reveals current spies, and changes their appearance. 
        /// </summary>
        public void RevealSpies(Utility.CustomTeams team)
        {
            foreach (Player player in AllCurrentSpies[team])
            {
                RevealSpies(player, team);
            }
        }
        /// <summary>
        /// Reveals current spies, and changes their appearance. 
        /// </summary>
        public void RevealSpies(Player player, Utility.CustomTeams team)
        {
            if (AllCurrentSpies[Utility.CustomTeams.Chaos].Contains(player) && team == Utility.CustomTeams.Chaos)
            {
                if (AllCurrentSpies[Utility.CustomTeams.Guards].Contains(player))
                {
                    ChangeAppearance(player, SpyUtilityNW.Instance.Config.GuardSpyLoadout.SpyRealRole);
                }
                else{
                    ChangeAppearance(player, SpyUtilityNW.Instance.Config.CiSpyLoadout.SpyRealRole);
                }
            }
            else if (AllCurrentSpies[Utility.CustomTeams.Mtf].Contains(player) && team == Utility.CustomTeams.Mtf)
            {
                ChangeAppearance(player, SpyUtilityNW.Instance.Config.MtfSpyLoadout.SpyRealRole);
            }
            else if (AllCurrentSpies[team].Contains(player))
            {
                ChangeAppearance(player, SpyUtilityNW.Instance.Config.GuardSpyLoadout.SpyRealRole);
            }
            RemoveFromSpies(player);
        }
        
        /// <summary>
        /// Reveals current spies, and changes their appearance. 
        /// </summary>
        public void RevealSpy(Player player)
        {
            if (AllCurrentSpies[Utility.CustomTeams.Chaos].Contains(player))
            {
                if (AllCurrentSpies[Utility.CustomTeams.Guards].Contains(player))
                {
                    ChangeAppearance(player, SpyUtilityNW.Instance.Config.GuardSpyLoadout.SpyRealRole);
                }
                else
                {
                    ChangeAppearance(player, SpyUtilityNW.Instance.Config.CiSpyLoadout.SpyRealRole);
                }
            }
            else if (AllCurrentSpies[Utility.CustomTeams.Mtf].Contains(player))
            {
                ChangeAppearance(player, SpyUtilityNW.Instance.Config.MtfSpyLoadout.SpyRealRole);
            }
            RemoveFromSpies(player);
        }

        private static void RemoveFromSpies(IPlayer target)
        {
            Player potentialSpy = Player.Get(target.ReferenceHub);
            if (potentialSpy == null)
            {
                return;
            }
            
            foreach (KeyValuePair<Utility.CustomTeams,ISet<Player>> allCurrentSpy in AllCurrentSpies)
            {
                allCurrentSpy.Value.Remove(potentialSpy);
            }
        }
        
        private static void RemoveFromSpies(IPlayer target, Utility.CustomTeams team)
        {
            Player potentialSpy = Player.Get(target.ReferenceHub.netId);
            switch (team)
            {
                case Utility.CustomTeams.Chaos:
                case Utility.CustomTeams.Guards:
                    AllCurrentSpies[Utility.CustomTeams.Chaos].Remove(potentialSpy);
                    AllCurrentSpies[Utility.CustomTeams.Guards].Remove(potentialSpy);
                    break;
                case Utility.CustomTeams.Mtf:
                    AllCurrentSpies[Utility.CustomTeams.Mtf].Remove(potentialSpy);
                    break;
            }
        }

        /// <summary>
        /// Player left means we remove from spy database.
        /// </summary>
        /// <param name="leavingPlayer"></param>
        [PluginEvent(ServerEventType.PlayerLeft)]
        [UsedImplicitly]
        public void OnPlayerLeave(IPlayer leavingPlayer)
        {
            RemoveFromSpies(leavingPlayer);
        }

        /// <summary>
        /// Forcefully remove spies from database.
        /// </summary>
        /// <param name="player"></param>
        /// <param name="team"></param>
        public static void ForceRemoveSpy(Player player, Utility.CustomTeams team)
        {
            RemoveFromSpies(player, team);
        }

        /// <summary>
        /// Override and set player role/spy team.
        /// </summary>
        /// <param name="player"></param>
        /// <param name="team"></param>
        /// <param name="newRoleID"></param>
        /// <returns></returns>
        public static bool ForceCreateSpy(Player player, Utility.CustomTeams team, RoleTypeId newRoleID = RoleTypeId.None)
        {
            RemoveFromSpies(player);
            if (newRoleID is RoleTypeId.None)
            {
                newRoleID = team switch
                {
                    Utility.CustomTeams.Chaos => RoleTypeId.ChaosRifleman,
                    Utility.CustomTeams.Mtf => RoleTypeId.NtfSergeant,
                    _ => newRoleID
                };
            }

            // player.ReferenceHub.roleManager.ServerSetRole(newRoleID, RoleChangeReason.None);
            var newSpyMessage = string.Format(SpyUtilityNW.Instance.Config.OnSpySpawnMessage, team);
            player.ReceiveHint(newSpyMessage, SpyUtilityNW.Instance.Config.OnSpySpawnMessageHintDuration);
            Log.Debug($"Settings role {newRoleID} for player {player.Nickname}, on team {team}", SpyUtilityNW.Instance.Config.Debug);
            switch (team)
            {
                case Utility.CustomTeams.Chaos:
                    Log.Debug($"Force Changing appearance to {newRoleID}", SpyUtilityNW.Instance.Config.Debug);
                    ChangeSpawnInventory(player, SpyUtilityNW.Instance.Config.CiSpyLoadout, team);
                    break;
                case Utility.CustomTeams.Mtf:
                    Log.Debug($"Force Changing appearance to {newRoleID}", SpyUtilityNW.Instance.Config.Debug);
                    ChangeSpawnInventory(player, SpyUtilityNW.Instance.Config.MtfSpyLoadout, team);
                    break;
                case Utility.CustomTeams.Guards:
                    Log.Debug($"Force Changing appearance to {newRoleID}", SpyUtilityNW.Instance.Config.Debug);
                    ChangeSpawnInventory(player, SpyUtilityNW.Instance.Config.GuardSpyLoadout, team);
                    break;
            }
            
            return ForceAddSpy(player, team);
        }
        
        /// <summary>
        /// No longer desync's the game
        /// </summary>
        /// <param name="player"></param>
        /// <param name="type"></param>
        public static void ChangeAppearance(Player player, RoleTypeId type)
        {
            Log.Debug($"Changing Appearance of player {player.Nickname} to {type}", SpyUtilityNW.Instance.Config.Debug);
            Timing.CallDelayed(SpyUtilityNW.Instance.Config.ChangeAppearanceDelay, () =>
            {
                foreach (Player target in Player.GetPlayers().Where(x => x != player))
                {
                    target.Connection.Send(new RoleSyncInfo(player.ReferenceHub, type, target.ReferenceHub));
                }
            });
        }

        /// <summary>
        /// Flashbang event, see <see cref="FlashBangEvent"/>
        /// </summary>
        /// <param name="flashEvent"></param>
        public void OnFlashBang(FlashBangEvent flashEvent)
        {
            OnPlayerDamage(Player.Get(flashEvent.Attacker), Player.Get(flashEvent.Attacker), null);
        }

    }

}