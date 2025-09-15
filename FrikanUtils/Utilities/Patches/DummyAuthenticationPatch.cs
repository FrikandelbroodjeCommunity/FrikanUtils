using CentralAuth;
using HarmonyLib;

namespace FrikanUtils.Utilities.Patches;

[HarmonyPatch]
internal static class DummyAuthenticationPatch
{
    private static byte _uniqueDummyId;

    [HarmonyPatch(typeof(PlayerAuthenticationManager), nameof(PlayerAuthenticationManager.Awake))]
    [HarmonyPrefix]
    // ReSharper disable once InconsistentNaming
    public static bool OnAwakeAuthentication(PlayerAuthenticationManager __instance)
    {
        if (__instance.connectionToClient is DummyUtilities.FakeConnection)
        {
            __instance.UserId = $"ID_CDummy_{_uniqueDummyId++}";
            return false;
        }

        return true;
    }
}