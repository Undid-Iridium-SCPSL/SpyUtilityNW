using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using CISpyUtilityNW.Events;
using HarmonyLib;
using static HarmonyLib.AccessTools;

using NorthwoodLib.Pools;
using PluginAPI.Core;
using Respawning;

namespace CISpyUtilityNW
{
    [HarmonyPatch(typeof(RespawnManager), nameof(RespawnManager.Spawn))]
    public class TeamRespawnPatcher
    {
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> TeamRespawnPatch(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            const int offset = 2;
            int curWaveIndex = newInstructions.FindIndex(instruction => instruction.Calls(PropertyGetter(typeof(SpawnableTeamHandlerBase), nameof(SpawnableTeamHandlerBase.MaxWaveSize)))) + offset;

            LocalBuilder teamRespawnEvent = generator.DeclareLocal(typeof(TeamRespawnEvent));

            newInstructions.InsertRange(
                curWaveIndex,
                new[]
                {
                    new CodeInstruction(OpCodes.Ldloc_1),
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Call, Method(typeof(TeamRespawnPatcher), nameof(TeamRespawnPatcher.HandleTeamRespawn))),
                });

            for (int pos = 0; pos < newInstructions.Count; pos++)
            {
                yield return newInstructions[pos];
            }
        }

        public static void HandleTeamRespawn(List<ReferenceHub> teamRespawnPlayers, RespawnManager respawnManager)
        {
            try
            {
                TeamRespawnEvent respawnEvent = new TeamRespawnEvent(teamRespawnPlayers, respawnManager.NextKnownTeam);

                Log.Debug("Handle team respawn", SpyUtilityNW.Instance.Config.Debug);
                PatchedEventHandlers.BeforeTeamRespawnEvent(respawnEvent);
            }
            catch (Exception failedToCreateEvent)
            {
                Log.Debug($"HandleTeamRespawn failed because {failedToCreateEvent}", SpyUtilityNW.Instance.Config.Debug);
            }
        }
    }
}