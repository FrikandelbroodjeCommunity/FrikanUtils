using CentralAuth;
using HarmonyLib;
using LabApi.Features.Console;

namespace FrikanUtils.Npc.Patches;

[HarmonyPatch]
internal static class DummyAuthenticationPatch
{
    private static byte _uniqueDummyId;

    [HarmonyPatch(typeof(PlayerAuthenticationManager), nameof(PlayerAuthenticationManager.Start))]
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