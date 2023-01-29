using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using InventorySystem.Items.ThrowableProjectiles;
using static HarmonyLib.AccessTools;

using NorthwoodLib.Pools;
using PluginAPI.Core;
using Respawning;
using SpyUtilityNW.Events;

namespace SpyUtilityNW
{
    [HarmonyPatch(typeof(FlashbangGrenade), "ProcessPlayer")]
    public class FlashbangPatch
    {
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> TeamRespawnPatch(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);
            
            newInstructions.InsertRange(
                newInstructions.Count -1,
                new[]
                {
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new(OpCodes.Ldarg_1),
                    new(OpCodes.Call, Method(typeof(FlashbangPatch), nameof(FlashbangPatch.HandleFlashbangProcessing))),
                });

            for (int pos = 0; pos < newInstructions.Count; pos++)
            {
                yield return newInstructions[pos];
            }
        }

        public static void HandleFlashbangProcessing(FlashbangGrenade instance, ReferenceHub target)
        {
            try
            {
                ReferenceHub attacker = instance.PreviousOwner.Hub;
                FlashBangEvent flashBangEvent = new FlashBangEvent(attacker, target);

                Log.Debug("Handle FlashBangEvent", SpyUtilityNW.Instance.Config.Debug);
                PatchedEventHandlers.FlashbangEventProcessing(flashBangEvent);
            }
            catch (Exception failedToCreateEvent)
            {
                Log.Debug($"FlashBangEvent failed because {failedToCreateEvent}", SpyUtilityNW.Instance.Config.Debug);
            }
        }
    }
}