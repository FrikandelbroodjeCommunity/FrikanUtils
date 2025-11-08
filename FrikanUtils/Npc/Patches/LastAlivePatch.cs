using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using LabApi.Features.Console;
using LabApi.Features.Wrappers;
using PlayerRoles.PlayableScps.HumanTracker;

namespace FrikanUtils.Npc.Patches;

internal static class LastAlivePatch
{
    private static readonly MethodInfo IgnoredMethodInfo = typeof(TargetPatches).GetMethod(nameof(IsIgnored));

    [HarmonyPrepare]
    public static bool OnPrepare(MethodBase _)
    {
        return UtilitiesPlugin.PluginConfig.UseLastAlive;
    }

    [HarmonyPatch(typeof(LastHumanTracker), nameof(LastHumanTracker.TryGetLastTarget))]
    [HarmonyTranspiler]
    public static IEnumerable<CodeInstruction> IgnoreTargets(IEnumerable<CodeInstruction> instructions)
    {
        var instructionList = instructions.ToArray();

        var passedOne = false;
        object reference = null;
        foreach (var instruction in instructionList.Where(x => x.opcode == OpCodes.Brfalse_S))
        {
            if (passedOne)
            {
                Logger.Warn($"Operand: {instruction.operand.GetType()} | {instruction.operand}");
                reference = instruction.operand;
                break;
            }

            passedOne = true;
        }

        if (reference == null)
        {
            Logger.Warn("Failed to find reference for LastHumanTracker");
        }

        var found = false;
        foreach (var instruction in instructionList)
        {
            if (reference != null && instruction.opcode == OpCodes.Ldloc_3 && !found)
            {
                yield return new CodeInstruction(OpCodes.Ldloc_3);
                yield return new CodeInstruction(OpCodes.Call, IgnoredMethodInfo);
                yield return new CodeInstruction(OpCodes.Brtrue_S, reference);
                found = true;
            }

            yield return instruction;
        }
    }

    public static bool IsIgnored(ReferenceHub hub)
    {
        var player = Player.Get(hub);
        return player != null && NpcSystem.IgnoreHumanTarget.Contains(player);
    }
}