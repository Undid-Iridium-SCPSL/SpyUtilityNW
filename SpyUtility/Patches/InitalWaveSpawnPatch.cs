using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using InventorySystem.Items.ThrowableProjectiles;
using static HarmonyLib.AccessTools;

using NorthwoodLib.Pools;
using PlayerRoles;
using PlayerRoles.RoleAssign;
using PluginAPI.Core;
using Respawning;
using SpyUtilityNW.Events;

namespace SpyUtilityNW
{
    
    [HarmonyPatch]
    public class InitalWaveSpawnPatch
    {
        [HarmonyPatch(typeof(HumanSpawner), nameof(HumanSpawner.SpawnHumans))]
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> WaveSpawnPatch(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);
            
            newInstructions.InsertRange(
                newInstructions.Count -1,
                new[]
                {
                    new CodeInstruction(OpCodes.Call, Method(typeof(InitalWaveSpawnPatch), nameof(InitalWaveSpawnPatch.HandleProcessingPlayers))),
                });

            for (int pos = 0; pos < newInstructions.Count; pos++)
            {
                yield return newInstructions[pos];
            }
        }
        
        [HarmonyPatch(typeof(HumanSpawner), "AssignHumanRoleToRandomPlayer")]
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> HumanRolePatch(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);
            
            newInstructions.InsertRange(
                newInstructions.Count -1,
                new[]
                {
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new(OpCodes.Ldloc_1),
                    new(OpCodes.Call, Method(typeof(InitalWaveSpawnPatch), nameof(InitalWaveSpawnPatch.AddPlayerToWaveSpawn))),
                });

            for (int pos = 0; pos < newInstructions.Count; pos++)
            {
                yield return newInstructions[pos];
            }
        }

        internal static Dictionary<RoleTypeId, List<ReferenceHub>> InitalWavePlayers = new();

        internal static HashSet<RoleTypeId> rolesAdded = new();

        public static void AddPlayerToWaveSpawn(RoleTypeId role, ReferenceHub player)
        {
            InitalWavePlayers.GetOrAdd(role, () => new List<ReferenceHub> {player});
            rolesAdded.Add(role);
        }

        public static void HandleProcessingPlayers()
        {
            try
            {
                Log.Debug("Handle initialWaveSpawnEvent HandleProcessingPlayers", SpyUtilityNW.Instance.Config.Debug);
                InitalWaveSpawnEvent initialWaveSpawnEvent = new InitalWaveSpawnEvent();
                PatchedEventHandlers.InitalWavePlayersEvent(initialWaveSpawnEvent);
            }
            catch (Exception failedToCreateEvent)
            {
                Log.Debug($"FlashBangEvent failed because {failedToCreateEvent}", SpyUtilityNW.Instance.Config.Debug);
            }
        }
    }
}