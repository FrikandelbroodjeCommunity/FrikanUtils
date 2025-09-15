using System;
using System.Linq;
using CommandSystem;
using CommandSystem.Commands.RemoteAdmin.Dummies;
using FrikanUtils.Utilities;
using HarmonyLib;
using LabApi.Features.Console;
using LabApi.Features.Wrappers;
using Utils;

namespace FrikanUtils.CustomDummyActions.Patches;

[HarmonyPatch]
internal static class ExecuteDummyPatch
{
    [HarmonyPatch(typeof(ActionDummyCommand), nameof(ActionDummyCommand.Execute))]
    [HarmonyPrefix]
    public static bool OnExecuteDummy(ArraySegment<string> arguments, ICommandSender sender, ref string response,
        // ReSharper disable once InconsistentNaming
        ref bool __result)
    {
        // If the command is not valid for custom execution
        if (!sender.CheckPermission(PlayerPermissions.FacilityManagement) || arguments.Count < 3 ||
            !sender.TryGetPlayer(out var player, out _))
        {
            return true;
        }

        // Get the ids of the selected players
        var ids = RAUtils.ProcessPlayerIdOrNamesList(arguments, 0, out _);
        if (ids == null)
        {
            return true;
        }

        // Get the targeted module
        var moduleName = arguments.At(1).Substring(1);
        var module = RaHandler.Modules
            .FirstOrDefault(x => x.Name.Replace(" ", "_") == arguments.At(1).Substring(1));
        if (module == null || !module.HasPermission(player))
        {
            Logger.Debug($"Could not find the module {moduleName}", UtilitiesPlugin.Debug);
            return true;
        }

        var executable = module.GetAction(player, ids.Select(Player.Get).ToList(), arguments.At(2));
        if (!executable.IsValid())
        {
            Logger.Debug("Executable is not valid", UtilitiesPlugin.Debug);
            return true;
        }

        __result = true;
        response = $"[Custom] {executable.Invoke(player)}";
        return false;
    }
}