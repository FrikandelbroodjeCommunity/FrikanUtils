using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using NetworkManagerUtils.Dummies;

namespace FrikanUtils.CustomDummyActions.Patches;

[HarmonyPatch(typeof(DummyActionCollector))]
internal static class CachePatch
{
    [HarmonyPrepare]
    public static bool OnPrepare(MethodBase _)
    {
        return UtilitiesPlugin.PluginConfig.UseCustomDummyActions;
    }

    [HarmonyPatch(nameof(DummyActionCollector.ServerGetActions))]
    [HarmonyPrefix]
    // ReSharper disable once InconsistentNaming
    public static bool OnGetActions(ReferenceHub hub, ref List<DummyAction> __result)
    {
        if (hub.IsDummy || hub.IsHost) return true;

        __result = [];
        return false;
    }

    [HarmonyPatch(nameof(DummyActionCollector.GetCache))]
    [HarmonyPrefix]
    // ReSharper disable once InconsistentNaming
    public static bool OnGetActions(ReferenceHub hub, ref DummyActionCollector.CachedActions __result)
    {
        if (hub.IsDummy || hub.IsHost) return true;

        if (!DummyActionCollector.CollectionCache.TryGetValue(hub, out var cache))
        {
            cache = new DummyActionCollector.CachedActions(hub);
            DummyActionCollector.CollectionCache[hub] = cache;
        }

        __result = cache;
        return false;
    }
}