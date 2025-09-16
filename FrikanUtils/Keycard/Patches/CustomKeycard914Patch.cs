using HarmonyLib;
using Scp914;
using Scp914.Processors;

namespace FrikanUtils.Keycard.Patches;

[HarmonyPatch(typeof(Scp914Upgrader))]
internal static class CustomKeycard914Patch
{
    [HarmonyPatch(typeof(Scp914Upgrader), nameof(Scp914Upgrader.TryGetProcessor))]
    [HarmonyPrefix]
    // ReSharper disable once InconsistentNaming
    public static bool OnTryGetProcessor(ItemType itemType, ref Scp914ItemProcessor processor, ref bool __result)
    {
        // Make custom keycards trigger the 914 system
        if (!CustomKeycardUtilities.CustomKeycards.Contains(itemType)) return true;
        if (!Scp914Upgrader.TryGetProcessor(ItemType.KeycardO5, out processor)) return true;

        __result = true;
        return false;
    }
}