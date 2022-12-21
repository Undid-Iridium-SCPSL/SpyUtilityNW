using System.Collections.Generic;
using System.Reflection.Emit;
using CISpyUtilityNW.Events;
using HarmonyLib;
using static HarmonyLib.AccessTools;

using NorthwoodLib.Pools;
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
            TeamRespawnEvent respawnEvent = new TeamRespawnEvent(teamRespawnPlayers, respawnManager.NextKnownTeam);

            PatchedEventHandlers.BeforeTeamRespawnEvent(respawnEvent);
        }
    }
}