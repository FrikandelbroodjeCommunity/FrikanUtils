using CentralAuth;
using HarmonyLib;

namespace FrikanUtils.Npc.Patches;

[HarmonyPatch(typeof(PlayerAuthenticationManager))]
internal static class DummyAuthenticationPatch
{
    private static byte _uniqueDummyId;

    [HarmonyPatch(nameof(PlayerAuthenticationManager.Awake))]
    [HarmonyPrefix]
    // ReSharper disable once InconsistentNaming
    public static bool OnAwakeAuthentication(PlayerAuthenticationManager __instance)
    {
        if (__instance.connectionToClient is NpcSystem.FakeConnection)
        {
            __instance.UserId = $"ID_CDummy_{_uniqueDummyId++}";
            return false;
        }

        return true;
    }
}