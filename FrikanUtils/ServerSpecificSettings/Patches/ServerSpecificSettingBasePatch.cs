using System.Reflection;
using HarmonyLib;
using UserSettings.ServerSpecific;

namespace FrikanUtils.ServerSpecificSettings.Patches;

[HarmonyPatch(typeof(ServerSpecificSettingBase))]
internal static class ServerSpecificSettingBasePatch
{
    [HarmonyPrepare]
    public static bool OnPrepare(MethodBase _)
    {
        return UtilitiesPlugin.PluginConfig.UseServerSpecificSettings;
    }

    [HarmonyPatch(nameof(ServerSpecificSettingBase.OriginalDefinition), MethodType.Getter)]
    [HarmonyPrefix]
    // ReSharper disable once InconsistentNaming
    public static bool OnGetOriginalDefinition(ServerSpecificSettingBase __instance,
        // ReSharper disable once InconsistentNaming
        ref ServerSpecificSettingBase __result)
    {
        __result = __instance;
        return false;
    }
}