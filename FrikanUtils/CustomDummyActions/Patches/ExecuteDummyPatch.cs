using System;
using System.Linq;
using System.Reflection;
using CommandSystem;
using CommandSystem.Commands.RemoteAdmin.Dummies;
using FrikanUtils.Utilities;
using HarmonyLib;
using LabApi.Features.Console;
using LabApi.Features.Wrappers;
using Utils;

namespace FrikanUtils.CustomDummyActions.Patches;

[HarmonyPatch(typeof(ActionDummyCommand))]
internal static class ExecuteDummyPatch
{
    [HarmonyPrepare]
    public static bool OnPrepare(MethodBase _)
    {
        return UtilitiesPlugin.PluginConfig.UseCustomDummyActions;
    }
    
    [HarmonyPatch(nameof(ActionDummyCommand.Execute))]
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
        var moduleName = arguments.At(1);
        var module = RaHandler.Modules
            .FirstOrDefault(x => x.Name.Replace(" ", "_") == moduleName);
        if (module == null || !module.HasPermission(player))
        {
            Logger.Debug($"Could not find the module {moduleName}", UtilitiesPlugin.PluginConfig.Debug);
            return true;
        }

        var executable = module.GetAction(player, ids.Select(Player.Get).ToList(), arguments.At(2));
        if (!executable.IsValid())
        {
            Logger.Debug("Executable is not valid", UtilitiesPlugin.PluginConfig.Debug);
            return true;
        }

        __result = true;
        response = $"[Custom] {executable.Invoke(player)}";
        return false;
    }
}