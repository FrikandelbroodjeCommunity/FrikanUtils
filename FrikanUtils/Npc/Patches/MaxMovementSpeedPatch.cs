using System.Collections.Generic;
using HarmonyLib;
using PlayerRoles.FirstPersonControl;

namespace FrikanUtils.Npc.Patches;

[HarmonyPatch]
internal class MaxMovementSpeedPatch
{
    internal static readonly List<ReferenceHub> NpcModules = [];

    [HarmonyPatch(typeof(FirstPersonMovementModule), nameof(FirstPersonMovementModule.MaxMovementSpeed),
        MethodType.Getter)]
    [HarmonyPrefix]
    // ReSharper disable once InconsistentNaming
    public static bool OverrideMaxMovement(FirstPersonMovementModule __instance,
        // ReSharper disable once InconsistentNaming
        ref float __result)
    {
        if (!NpcModules.Contains(__instance.Hub)) return true;
        __result = float.MaxValue;
        return false;
    }
}