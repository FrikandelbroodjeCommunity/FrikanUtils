using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using FrikanUtils.Utilities;
using HarmonyLib;
using LabApi.Features.Wrappers;
using RemoteAdmin.Communication;

namespace FrikanUtils.CustomDummyActions.Patches;

[HarmonyPatch(typeof(RaDummyActions))]
public static class LoadActionsPatch
{
    private static Player _currentPlayer;

    private static readonly PropertyInfo IsDummyField = typeof(ReferenceHub).GetProperty(nameof(ReferenceHub.IsDummy));
    private static readonly PropertyInfo IsHostField = typeof(ReferenceHub).GetProperty(nameof(ReferenceHub.IsHost));

    [HarmonyPatch(nameof(RaDummyActions.ReceiveData), typeof(CommandSender), typeof(string))]
    [HarmonyPrefix]
    public static void OnReceiveData(CommandSender sender)
    {
        _currentPlayer = sender.TryGetPlayer(out var player, out _) ? player : null;
    }

    [HarmonyPatch(nameof(RaDummyActions.ReceiveData), typeof(CommandSender), typeof(string))]
    [HarmonyTranspiler]
    public static IEnumerable<CodeInstruction> OnReceiveDataTranspiler(IEnumerable<CodeInstruction> instructions)
    {
        var found = false;
        foreach (var instruction in instructions)
        {
            if (found)
            {
                found = false;
                instruction.opcode = OpCodes.Brtrue_S;
            }
            else if (instruction.operand != null && instruction.operand.Equals(IsDummyField.GetMethod))
            {
                found = true;
                instruction.operand = IsHostField.GetMethod;
            }

            yield return instruction;
        }
    }

    [HarmonyPatch(nameof(RaDummyActions.GatherData))]
    [HarmonyPrefix]
    // ReSharper disable once InconsistentNaming
    public static bool OnGatherData(RaDummyActions __instance)
    {
        foreach (var hub in ReferenceHub.AllHubs.Where(hub =>
                     !hub.IsHost &&
                     !RaDummyActions.NonDirtyReceivers.GetOrAddNew(hub.netId).Contains(__instance._senderNetId)))
        {
            __instance.AppendDummy(hub);
        }

        return false;
    }

    [HarmonyPatch(nameof(RaDummyActions.AppendDummy))]
    [HarmonyPostfix]
    // ReSharper disable once InconsistentNaming
    public static void OnGatherDummyActions(RaDummyActions __instance, ReferenceHub dummy)
    {
        if (_currentPlayer == null)
        {
            return;
        }

        var dummyPlayer = Player.Get(dummy);
        if (dummyPlayer == null)
        {
            return;
        }
        
        foreach (var str in RaHandler.Modules
                     .Where(module => module.HasPermission(_currentPlayer))
                     .SelectMany(module => module.GetStrings(_currentPlayer, dummyPlayer)))
        {
            __instance.AppendData(str);
        }
    }
}